namespace backend.Models
{
    public class Tile
    {
        public Player? Owner { get; set; }
        public int Rank { get; set; }
        public Card? Card { get; set; }
        public int BonusPower { get; set; } // Bonus Power from other cards
        public int SelfBonusPower { get; set; } // Bonus Power from this card's ability

        // Private variables
        private List<string> onPlaceConditions = new List<string> { "AP", "EP" };
        private List<string> onDestroyConditions = new List<string> { "D", "AD", "ED", "*" };
        private List<string> onEnhanceConditions = new List<string> { "P1R", "1+", "+A", "+E", "+AE" };
        private List<string> onEnfeebleConditions = new List<string> { "1-", "-A", "-E", "-AE" };
        private List<string> onWinLane = new List<string> { "+Score", "L+V" };

        public void RankUp(int amount)
        {
            Rank = Rank + amount > 3 ? 3 : Rank + amount;
        }

        public void InitAbility(Game game)
        {
            game.OnCardPlaced += HandleCardPlaced;

            if (onPlaceConditions.Contains(Card!.Ability.Condition))
            {
                game.OnCardPlaced += HandleAnotherCardPlaced;
            }
            else if (onDestroyConditions.Contains(Card!.Ability.Condition))
            {
                game.OnCardDestroyed += HandleCardDestroyed;
            }
            else if (onEnhanceConditions.Contains(Card!.Ability.Condition))
            {
                game.OnCardEnhanced += HandleCardEnhanced;
            }
            else if (onEnfeebleConditions.Contains(Card!.Ability.Condition))
            {
                game.OnCardEnfeebled += HandleCardEnfeebled;
            }
        }

        private void UninitAbility(Game game, Tile[,] grid, int row, int col)
        {
            // Unsubscribe the destroyed card from all events
            game.OnCardPlaced -= HandleCardPlaced;
            game.OnCardDestroyed -= HandleCardDestroyed;
            game.OnCardEnhanced -= HandleCardEnhanced;
            game.OnCardEnfeebled -= HandleCardEnfeebled;

            /*
			 * Undo the effects of the ability on other cards if applicable. This includes abilities with:
			 *  - "While in play" condition (*)
			 */
            if (Card!.Ability.Condition == "*")
            {
                var operation = Card!.Ability.Action!.Contains("+") ? 1 : -1;

                foreach (RangeCell rangeCell in Card!.Range)
                {
                    var dx = col + rangeCell.Offset.x;
                    var dy = row + rangeCell.Offset.y;
                    var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                    if (rangeCell.Colour.Contains("R") && isIndexInBounds && Card!.Ability.Value != null)
                        grid[dy, dx].BonusPower -= (int) Card!.Ability.Value * operation;
                }
            }
        }

        private void HandleTargetingAbilties(Tile tile, Game game, int row, int col)
        {
            if (Card!.Ability.Value == null) return;
            var operation = Card!.Ability.Action!.Contains("+") ? 1 : -1;

            if (
                // Check if we're allowed to use the ability on the tile on target
                ((Card!.Ability.Target == "a" && tile.Owner == Owner) ||
                (Card!.Ability.Target == "e" && tile.Owner != Owner) ||
                Card!.Ability.Target == "ae")
                &&
                // If card doesn't have "While in play" (*) condition, don't target empty tiles
                (tile.Card != null || Card!.Ability.Condition == "*")
                &&
                Card!.Ability.Value != null
            )
            {
                game.ChangePower(tile, row, col, (int) Card!.Ability.Value * operation);
            }
        }

        private void ExecuteAbility(Game game, Tile[,] grid, int row, int col)
        {
            // Targets allied or enemy cards means it uses the red tiles
            if (Card!.Ability.Target != null)
            {
                if (Card!.Ability.Target.Contains("a") || Card!.Ability.Target.Contains("e"))
                {
                    HandleTargetingAbilties(grid[row,col], game, row, col);
                }
                // Raise this card's power by one
                else if (Card!.Ability.Target == "s")
                {
                    CalculateSelfBoostFromPowerModifiedCards(game, grid, row, col);
                }
                // This card's ability doesn't affect other cards directly
                else
                {

                }
            }
        }

        private void HandleAnotherCardPlaced(Game game, Tile[,] grid, int row, int col)
        {
            if (
                grid[row, col] != this &&
                grid[row, col].Owner == Owner && Card!.Ability.Condition == "AP" || 
                grid[row, col].Owner != Owner && Card!.Ability.Condition == "EP"
            )
            {
                SelfBonusPower++;
            }
        }

        private void CalculateSelfBoostFromPowerModifiedCards(Game game, Tile[,] grid, int row, int col)
        {
            // Prevent out of index error for retrieving the modifier
            string modifier = Card!.Ability.Condition != null ? Card!.Ability.Condition[0].ToString() : "0";
            var modifiers = new string[] { "+", "-" };
            if (!modifiers.Contains(modifier)) return;

            var alliesModified = 0;
            var enemiesModified = 0;
            var triggerCondition = Card!.Ability.Condition;
            var cards = modifier == "+" ? game.EnhancedCards : game.EnfeebledCards;

            foreach (Tile enhancedTile in cards)
            {
                if (enhancedTile.Card == null) 
                    continue;
                if (enhancedTile.Owner == Owner && triggerCondition!.Contains(modifier) && enhancedTile != this)
                    alliesModified++;
                if (enhancedTile.Owner != Owner && triggerCondition!.Contains(modifier) && enhancedTile != this)
                    enemiesModified++;
            }

            if (triggerCondition == "+A" || triggerCondition == "-A")
                SelfBonusPower = (int)Card!.Ability.Value! * alliesModified;
            else if (triggerCondition == "+E" || triggerCondition == "-E")
                SelfBonusPower = (int)Card!.Ability.Value! * enemiesModified;
            else if (triggerCondition == "+AE" || triggerCondition == "-AE")
                SelfBonusPower = (int)Card!.Ability.Value! * (alliesModified + enemiesModified);
        }

        private void HandleCardPlaced(Game game, Tile[,] grid, int row, int col)
        {

            /* Orange tiles in range:
             * - Rank up
             * - Change owner
             */
            var executeAbilityImmediately = Card!.Ability.Condition == "P" || Card!.Ability.Condition == "*";

            foreach (RangeCell rangeCell in Card!.Range)
            {
                var dx = col + rangeCell.Offset.x;
                var dy = row + rangeCell.Offset.y;
                var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                if (!isIndexInBounds) continue;
                var offsetTile = grid[dy, dx];

                if (rangeCell.Colour.Contains("O") && isIndexInBounds && offsetTile.Card == null)
                {
                    offsetTile.Owner = game.currentPlayer;
                    offsetTile.RankUp(Card!.RankUpAmount);
                }

                if (rangeCell.Colour.Contains("R") && isIndexInBounds && executeAbilityImmediately)
                {
                    ExecuteAbility(game, grid, dy, dx);
                }
            }

            var target = Card!.Ability.Target;
            var triggerCondition = Card!.Ability.Condition;

            // If the card doesn't need to listen for other cards being placed, unsubscribe
            if (!onPlaceConditions.Contains(triggerCondition))
                game.OnCardPlaced -= HandleCardPlaced;
            // Cards that boost their own power when other cards are enhanced/enfeebled need to look at the board's status and update
            if ((target == "s") && (triggerCondition.Contains("A") || triggerCondition.Contains("E")))
                CalculateSelfBoostFromPowerModifiedCards(game, grid, row, col);
        }

        private void HandleCardDestroyed(Game game, Tile[,] grid, int row, int col)
        {
            // Execute post-mortem ability
            if (Card!.Ability!.Condition == "D")
            {
                foreach (RangeCell rangeCell in Card!.Range)
                {
                    var dx = col + rangeCell.Offset.x;
                    var dy = row + rangeCell.Offset.y;
                    var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                    if (!isIndexInBounds) continue;
                    var offsetTile = grid[dy, dx];

                    if (rangeCell.Colour.Contains("R") && isIndexInBounds)
                    {
                        ExecuteAbility(game, grid, dy, dx);
                    }
                }
            }

            UninitAbility(game, grid, row, col);
        }

        private void HandleCardEnhanced(Game game, Tile[,] grid, int row, int col)
        {
            // Handle self-enhancing ability
            if (Card!.Ability.Target == "s")
            {
                ExecuteAbility(game, grid, row, col);
                return;
            }

            // Cloud's ability should only execute once it reaches >= 7 power
            if (Card!.Ability.Condition == "P1R" && GetCumulativePower() < 7) return;

            // Handle targetting ability
            foreach (RangeCell rangeCell in Card!.Range)
            {
                var dx = col + rangeCell.Offset.x;
                var dy = row + rangeCell.Offset.y;
                var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                if (!isIndexInBounds) continue;
                var offsetTile = grid[dy, dx];

                if (rangeCell.Colour.Contains("R") && isIndexInBounds)
                {
                    ExecuteAbility(game, grid, dy, dx);
                }
            }

            if (Card!.Ability.Condition.Contains("1"))
                game.OnCardEnhanced -= HandleCardEnhanced;
        }

        private void HandleCardEnfeebled(Game game, Tile[,] grid, int row, int col)
        {
            if (Card!.Ability.Target == "s")
            {
                ExecuteAbility(game, grid, row, col);
                return;
            }

            // Handle ability trigger when first enfeebled
            foreach (RangeCell rangeCell in Card!.Range)
            {
                var dx = col + rangeCell.Offset.x;
                var dy = row + rangeCell.Offset.y;
                var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                if (!isIndexInBounds) continue;
                var offsetTile = grid[dy, dx];

                if (rangeCell.Colour.Contains("R") && isIndexInBounds)
                {
                    ExecuteAbility(game, grid, dy, dx);
                }
            }

            if (Card!.Ability.Condition.Contains("1"))
                game.OnCardEnfeebled -= HandleCardEnfeebled;
        }

        public int GetCumulativePower()
        {
            return Card!.Power + BonusPower + SelfBonusPower;
        }
    }
}
