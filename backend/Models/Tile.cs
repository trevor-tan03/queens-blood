using System.Text.Json.Serialization;
using Newtonsoft.Json.Bson;
using static backend.Models.TileConstants;

namespace backend.Models
{
    public class Tile
    {
        public Player? Owner { get; set; }
        public int Rank { get; set; }
        public Card? Card { get; set; }
        public int TileBonusPower { get; set; } = 0; // Bonus Power from other cards with the "While in play" (*) condition
        public int CardBonusPower {  get; set; } = 0; // Bonus Power that only affects the card on the tile, not the tile itself
        public int SelfBonusPower { get; set; } = 0; // Bonus Power from this card's ability

        public void RankUp(int amount)
        {
            Rank = Rank + amount > 3 ? 3 : Rank + amount;
        }

        public void InitAbility(Game game)
        {
            game.OnCardPlaced += HandleCardPlaced;

            if (OnPlaceConditions.Contains(Card!.Ability.Condition))
                game.OnCardPlaced += HandleAnotherCardPlaced;
            else if (OnDestroyConditions.Contains(Card!.Ability.Condition))
                game.OnCardDestroyed += HandleCardDestroyed;
            else if (OnEnhanceConditions.Contains(Card!.Ability.Condition))
                game.OnCardEnhanced += HandleCardEnhanced;

            if (OnEnfeebleConditions.Contains(Card!.Ability.Condition))
                game.OnCardEnfeebled += HandleCardEnfeebled;

            if (OnGameEndCondition == Card!.Ability.Action)
                game.OnGameEnd += HandleAddLaneLoserScoreToVictor;
        }

        private void UninitAbility(Game game, Tile[,] grid, int row, int col)
        {
            // Unsubscribe the destroyed card from all events
            if (this == grid[row, col])
            {
                game.OnCardPlaced -= HandleCardPlaced;
                game.OnCardDestroyed -= HandleCardDestroyed;
                game.OnCardEnhanced -= HandleCardEnhanced;
                game.OnCardEnfeebled -= HandleCardEnfeebled;
                game.OnGameEnd -= HandleAddLaneLoserScoreToVictor;
            }

            /*
			 * Undo the effects of the ability on other cards if applicable. This includes abilities with:
			 *  - "While in play" condition (*)
			 */
            var abilityCondition = Card!.Ability.Condition;
            var abilityAction = Card!.Ability.Action;

            if (abilityCondition == "*" && abilityAction != "L+V")
            {
                var operation = Card!.Ability.Action!.Contains("+") ? 1 : -1;

                foreach (RangeCell rangeCell in Card!.Range)
                {
                    var dx = col + rangeCell.Offset.x;
                    var dy = row + rangeCell.Offset.y;
                    var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                    if (rangeCell.Colour.Contains("R") && isIndexInBounds && Card!.Ability.Value != null)
                        grid[dy, dx].TileBonusPower -= (int) Card!.Ability.Value * operation;
                }
            }
            else if (abilityCondition == "W")
            {
                int winBonusFromCard = (int)Card!.Ability.Value!;
                // Subtract the destroyed card's win bonus score
                Owner!.Scores[row].winBonus -= winBonusFromCard;
            }
        }

        private bool IsTileTargettable(Card card, Tile tile)
        {
            var target = Card!.Ability.Target;
            var condition = Card!.Ability.Condition;

            return condition == "*" || 
                (target == "a" && tile.Owner == Owner) ||
                (target == "e" && tile.Owner != Owner) ||
                target == "ae" 
                && tile.Card != null;
        }

        private void HandleTargetingAbilties(Tile tile, Game game, int row, int col)
        {
            var abilityCondition = Card!.Ability.Condition;
            var abilityValue = Card!.Ability.Value;
            var abilityAction = Card!.Ability.Action;
            var abilityTarget = Card!.Ability.Target;

            if (!IsTileTargettable(Card, tile)) return;

            int operation;
            if (abilityCondition == "EE")
                operation = game.EnhancedCards.Contains(this) ? 1 : -1;
            else
                operation = abilityAction!.Contains("+") ? 1 : -1;

            bool isTilePowerBonus = Card.Ability.Condition == "*";

            if (abilityValue != null)
            {
                game.ChangePower(Owner, tile, row, col, (int) abilityValue * operation, isTilePowerBonus);
            }
            else if (abilityAction == "destroy")
            {
                var currPlayerIndex = game.Players.FindIndex(p => p == Owner);
                var grid = currPlayerIndex == 0 ? game.Player1Grid : game.Player2Grid;
                game.DestroyCard(Owner, row, col);
            }
        }

        private void HandleAddCardsToHandAbility(Game game, Tile[,] grid, int row, int col)
        {
            var children = grid[row, col].Card!.Children;
            game.CurrentPlayer!.Hand.AddRange(children);
        }

        private void HandleSpawnCardsAbility(Game game, Tile[,] grid)
        {
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < NUM_COLS; j++)
                {
                    var tile = grid[i, j];
                    if (tile.Card == null && tile.Owner == Owner)
                    {
                        var childIndex = tile.Rank - 1;
                        var childCard = Card!.Children[childIndex];

                        var hand = game.CurrentPlayer!.Hand;
                        hand.Add(childCard);

                        game.PlaceCard(hand.Count - 1, i, j);
                    }
                }
            }
        }

        private void HandleWinLaneBonusScore(Player player, int row)
        {
            var newWinBonus = player.Scores[row].winBonus + (int) Card!.Ability.Value!;
            player.Scores[row].winBonus = newWinBonus;
        }

        private void HandleAddLaneLoserScoreToVictor(Game game)
        {
            // Look at the lanes the owner is winning and subtract the enemy's points in that lane and subtract winBonus
            for (int i = 0; i < NUM_ROWS; i++)
            {
                var enemy = game.Players.Find(p => p != Owner);
                var laneWinner = game.GetLaneWinner(i);

                // Owner is the victor. Add enemy's score to owner's
                if (laneWinner == Owner)
                    Owner!.Scores[i].winBonus = Owner.Scores[i].winBonus + enemy!.Scores[i].score;
                // Enemy is the victor. Add owner's score to enemy's
                else if (laneWinner == enemy)
                    enemy!.Scores[i].winBonus = enemy.Scores[i].winBonus + Owner!.Scores[i].score;
            }
        }

        private void ExecuteAbility(Game game, Tile[,] grid, int row, int col)
        {
            // Targets allied or enemy cards means it uses the red tiles
            if (Card!.Ability.Target != null)
            {
                // Enhance/enfeeble/destroy cards on affected tiles
                if (Card!.Ability.Target.Contains("a") || Card!.Ability.Target.Contains("e"))
                {
                    HandleTargetingAbilties(grid[row,col], game, row, col);
                }
                // Raise this card's power by one
                else if (Card!.Ability.Target == "s")
                {
                    CalculateSelfBoostFromPowerModifiedCards(game, grid, row, col);
                }
            }
            else
            {
                switch (Card!.Ability.Action)
                {
                    case "add":
                        HandleAddCardsToHandAbility(game, grid, row, col);
                        break;
                    case "spawn":
                        HandleSpawnCardsAbility(game, grid);
                        break;
                    case "+Score":
                        HandleWinLaneBonusScore(game.CurrentPlayer!, row);
                        break;
                    case "L+V":
                        HandleAddLaneLoserScoreToVictor(game);
                        break;
                    default:
                        break;
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
            var rankUpAmount = Card!.RankUpAmount;

            foreach (RangeCell rangeCell in Card!.Range)
            {
                var dx = col + rangeCell.Offset.x;
                var dy = row + rangeCell.Offset.y;
                var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                if (!isIndexInBounds) continue;
                var offsetTile = grid[dy, dx];

                if (rangeCell.Colour.Contains("O") && isIndexInBounds && offsetTile.Card == null)
                {
                    offsetTile.Owner = game.CurrentPlayer;
                    offsetTile.RankUp(rankUpAmount);
                }

                if (rangeCell.Colour.Contains("R") && isIndexInBounds && executeAbilityImmediately)
                {
                    ExecuteAbility(game, grid, dy, dx);
                }
            }

            var target = Card!.Ability.Target;
            var action = Card!.Ability.Action;
            var triggerCondition = Card!.Ability.Condition;

            // If the card doesn't need to listen for other cards being placed, unsubscribe
            if (!OnPlaceConditions.Contains(triggerCondition))
                game.OnCardPlaced -= HandleCardPlaced;

            // Cards that boost their own power when other cards are enhanced/enfeebled need to look at the board's status and update
            if ((target == "s") && (triggerCondition.Contains("A") || triggerCondition.Contains("E")))
                CalculateSelfBoostFromPowerModifiedCards(game, grid, row, col);

            if (action == "add" || action == "spawn" || action == "+Score")
                ExecuteAbility(game, grid, row, col);

            //
            if (game.EnhancedCards.Contains(this))
                game.EnqueueOnPowerChange("enhance", grid, row, col);
            else if (game.EnfeebledCards.Contains(this))
                game.EnqueueOnPowerChange("enfeeble", grid, row, col);
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

            if (Card!.Ability.Condition == "AD" && grid[row, col].Owner == Owner ||
                Card!.Ability.Condition == "ED" && grid[row, col].Owner != Owner ||
                Card!.Ability.Condition == "AED")
                SelfBonusPower += (int) Card!.Ability.Value!;
            
            if (this == grid[row, col])
                UninitAbility(game, grid, row, col);
        }

        private void HandleCardEnhanced(Game game, Tile[,] grid, int row, int col)
        {
            var triggerCondition = Card!.Ability.Condition;
            var target = Card!.Ability.Target;

            // Handle self-enhancing ability
            if (Card!.Ability.Target == "s")
            {
                ExecuteAbility(game, grid, row, col);
                return;
            }

            // Cloud's ability should only execute once it reaches >= 7 power
            if (Card!.Ability.Condition == "P1R" && GetCumulativePower() < 7) return;

            // Unsubscribe so that the ability doesn't trigger again
            if (
                triggerCondition.Contains("1") || 
                (triggerCondition == "P1R" && GetCumulativePower() >= 7) ||
                triggerCondition == "EE")
                game.OnCardEnhanced -= HandleCardEnhanced;
            if (triggerCondition == "EE")
                game.OnCardEnfeebled -= HandleCardEnfeebled;

            // Cards that boost their own power when other cards are enhanced/enfeebled need to look at the board's status and update
            if ((target == "s") && (triggerCondition.Contains("A") || triggerCondition.Contains("E")))
                CalculateSelfBoostFromPowerModifiedCards(game, grid, row, col);

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
        }

        private void HandleCardEnfeebled(Game game, Tile[,] grid, int row, int col)
        {
            if (Card!.Ability.Target == "s")
            {
                ExecuteAbility(game, grid, row, col);
                return;
            }

            if (Card!.Ability.Condition.Contains("1") ||
                Card!.Ability.Condition == "EE")
                game.OnCardEnfeebled -= HandleCardEnfeebled;
            if (Card!.Ability.Condition == "EE")
                game.OnCardEnhanced -= HandleCardEnhanced;

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
        }

        public int GetCumulativePower()
        {
            return (Card?.Power ?? 0) + TileBonusPower + CardBonusPower + SelfBonusPower;
        }
    }
}
