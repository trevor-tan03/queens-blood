namespace backend.Models
{
    public class Tile
    {
        public Player? Owner { get; set; }
        public int Rank { get; set; }
        public Card? Card { get; set; }
        public int BonusPower { get; set; }

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
                foreach (RangeCell rangeCell in Card!.Range)
                {
                    var dx = col + rangeCell.Offset.x;
                    var dy = row + rangeCell.Offset.y;
                    var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                    if (rangeCell.Colour.Contains("R") && isIndexInBounds && Card!.Ability.Value != null)
                        grid[dy, dx].BonusPower -= (int) Card!.Ability.Value;
                }
            }
        }

        private void HandleTargetingAbilties(Tile tile, Game game)
        {
            if (Card!.Ability.Value == null) return;
            var operation = Card!.Ability.Action!.Contains("+") ? 1 : -1;

            if (operation == 1)
                game.EnhancedCards.Add(tile);
            else
                game.EnfeebledCards.Add(tile);

            if (
                // Check if we're allowed to use the ability on the tile on target
                ((Card!.Ability.Target == "a" && tile.Owner == Owner) ||
                (Card!.Ability.Target == "e" && tile.Owner != Owner) ||
                (Card!.Ability.Target == "ae"))
                &&
                // Check if we're allowed to use the ability on an empty tile
                ((Card!.Ability.Condition == "P" && tile.Card != null) || 
                Card!.Ability.Condition == "*")
            )
            {
                tile.BonusPower += operation * (int) Card!.Ability.Value;
            }
        }

        private void HandleSelfEnhancingAbilities()
        {

        }

        private void ExecuteAbility(Game game, Tile[,] grid, int row, int col)
        {
            // Targets allied or enemy cards means it uses the red tiles
            if (Card!.Ability.Target != null)
            {
                if (Card!.Ability.Target.Contains("a") || Card!.Ability.Target.Contains("e"))
                {
                    HandleTargetingAbilties(grid[row,col], game);
                }
                // Raise this card's power by one
                else if (Card!.Ability.Target == "s")
                {

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
                (grid[row, col] != this) &&
                (grid[row, col].Owner == Owner && Card!.Ability.Condition == "AP") || 
                (grid[row, col].Owner != Owner && Card!.Ability.Condition == "EP"))
            {
                BonusPower++;
            }
        }

        private void HandleCardPlaced(Game game, Tile[,] grid, int row, int col)
        {

            /* Orange tiles in range:
             * - Rank up
             * - Change owner
             */
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

                if (rangeCell.Colour.Contains("R") && isIndexInBounds)
                {
                    ExecuteAbility(game, grid, dy, dx);
                }
            }

            // If the card doesn't need to listen for other cards being placed, unsubscribe
            if (!onPlaceConditions.Contains(Card!.Ability.Condition))
            {
                game.OnCardPlaced -= HandleCardPlaced;
            }
        }

        private void HandleCardDestroyed(Game game, Tile[,] grid, int row, int col)
        {
            UninitAbility(game, grid, row, col);

        }

        private void HandleCardEnhanced(Game game, Tile[,] grid, int row, int col)
        {
            
        }

        private void HandleCardEnfeebled(Game game, Tile[,] grid, int row, int col)
        {
            var cardOnTile = grid[row, col].Card;

            // Destroy card if the power is less than or equal to 0
            if (grid[row, col].BonusPower + grid[row, col]!.Card!.Power <= 0)
                grid[row, col].Card = null;

            // Hand
        }
    }
}
