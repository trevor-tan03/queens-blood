using System;
using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Bson;
using static backend.Models.TileConstants;

namespace backend.Models
{
    public class Tile
    {
        public string Id {  get; set; }
        public Player? Owner { get; set; }
        public int Rank { get; set; }
        public Card? Card { get; set; }
        // Bonus Power from other cards with the "While in play" (*) condition. Different for each player due to cards having different target for their abilities
        public int[] PlayerTileBonusPower { get; set; } = new int[2];
        public int CardBonusPower { get; set; } = 0; // Bonus Power that only affects the card on the tile, not the tile itself
        public int SelfBonusPower { get; set; } = 0; // Bonus Power from this card's ability
        public List<string> _subscribedEvents = new List<string>();

        public Tile(string id)
        {
            Id = id;
        }

        public void RankUp(int amount)
        {
            Rank = Rank + amount > 3 ? 3 : Rank + amount;
        }

        public void InitAbility(Game game)
        {
            game.OnCardPlaced += HandleCardPlaced;

            if (OnPlaceConditions.Contains(Card!.Ability.Condition))
            {
                _subscribedEvents.Add("P");
                game.OnCardPlaced += HandleAnotherCardPlaced;
            }
            else if (OnDestroyConditions.Contains(Card!.Ability.Condition))
            {
                _subscribedEvents.Add("D");
                game.OnCardDestroyed += HandleCardDestroyed;
            }
            else if (OnEnhanceConditions.Contains(Card!.Ability.Condition))
            {
                _subscribedEvents.Add("+P");
                game.OnCardEnhanced += HandleCardEnhanced;
            }

            if (OnEnfeebleConditions.Contains(Card!.Ability.Condition))
            {
                _subscribedEvents.Add("-P");
                game.OnCardEnfeebled += HandleCardEnfeebled;
            }

            if (OnRoundEndCondition == Card!.Ability.Action)
            {
                _subscribedEvents.Add("L+V");
                game.OnRoundEnd += HandleAddLaneLoserScoreToVictor;
            }

            if (OnEnhancedCardsChangedConditions.Contains(Card!.Ability.Condition))
            {
                _subscribedEvents.Add("+C");
                game.OnEnhancedCardsChanged += HandleEnhancedCardsChanged;
            }
            
            if (OnEnfeebledCardsChangedConditions.Contains(Card!.Ability.Condition))
            {
                _subscribedEvents.Add("-C");
                game.OnEnfeebledCardsChanged += HandleEnfeebledCardsChanged;
            }
        }

        private void HandleEnhancedCardsChanged(Game game)
        {
            var triggerCondition = Card!.Ability.Condition;
            var alliesEnhanced = game.EnhancedCards.Exists(c => c.Owner != null && c.Owner.Id == Owner!.Id);
            var enemiesEnhanced = game.EnhancedCards.Exists(c => c.Owner != null && c.Owner.Id != Owner!.Id);

            if ((triggerCondition.Contains("AE") && (alliesEnhanced || enemiesEnhanced)) ||
                (triggerCondition.Contains("A") && alliesEnhanced)||
                (triggerCondition.Contains("E") && enemiesEnhanced))
            {
                if (!game.EnhancedCards.Contains(this))
                    game.AddToEnhancedCards(this);
            }
            else
            {
                game.RemoveFromEnhancedCards(this);
            }

            RecalculateSelfBonusPower(game);
        }
        
        private void HandleEnfeebledCardsChanged(Game game)
        {
            var triggerCondition = Card!.Ability.Condition;
            var alliesEnfeebled = game.EnfeebledCards.Exists(c => c.Owner != null && c.Owner.Id == Owner!.Id);
            var enemiesEnfeebled = game.EnfeebledCards.Exists(c => c.Owner != null && c.Owner.Id != Owner!.Id);

            if ((triggerCondition.Contains("AE") && (alliesEnfeebled || enemiesEnfeebled)) ||
                (triggerCondition.Contains("A") && alliesEnfeebled)||
                (triggerCondition.Contains("E") && enemiesEnfeebled))
            {
                if (!game.EnhancedCards.Contains(this))
                    game.EnhancedCards.Add(this);
            }
            else
            {
                game.EnhancedCards.Remove(this);
            }

            RecalculateSelfBonusPower(game);
        }

        private void RecalculateSelfBonusPower(Game game)
        {
            game.SelfEnhanceQueue.Enqueue(() => CalculateSelfBoostFromPowerModifiedCards(game));
        }
        
        public void ReInitAbilities(Game game)
        {
            game.OnCardPlaced -= HandleCardPlaced;
            game.OnCardDestroyed -= HandleCardDestroyed;
            game.OnCardEnhanced -= HandleCardEnhanced;
            game.OnCardEnfeebled -= HandleCardEnfeebled;
            game.OnRoundEnd -= HandleAddLaneLoserScoreToVictor;

            foreach (var subscribedEvent in _subscribedEvents)
            {
                switch (subscribedEvent)
                {
                    case "P":
                        game.OnCardPlaced += HandleAnotherCardPlaced;
                        break;
                    case "D":
                        game.OnCardDestroyed += HandleCardDestroyed;
                        break;
                    case "+P":
                        game.OnCardEnhanced += HandleCardEnhanced;
                        break;
                    case "-P":
                        game.OnCardEnfeebled += HandleCardEnfeebled;
                        break;
                    case "L+V":
                        game.OnRoundEnd += HandleAddLaneLoserScoreToVictor;
                        break;
                    case "+C":
                        game.OnEnhancedCardsChanged += HandleEnhancedCardsChanged;
                        break;
                    case "-C":
                        game.OnEnfeebledCardsChanged += HandleEnfeebledCardsChanged;
                        break;
                    default:
                        break;
                }
            }
        }

        private void UninitAbility(Player instigator, Game game, Tile[,] grid, int row, int col)
        {
            // Unsubscribe the destroyed card from all events
            if (this == grid[row, col])
            {
                game.OnCardPlaced -= HandleCardPlaced;
                game.OnCardDestroyed -= HandleCardDestroyed;
                game.OnCardEnhanced -= HandleCardEnhanced;
                game.OnCardEnfeebled -= HandleCardEnfeebled;
                game.OnRoundEnd -= HandleAddLaneLoserScoreToVictor;

                _subscribedEvents.Clear();
            }

            /*
			 * Undo the effects of the ability on other cards if applicable. This includes abilities with:
			 *  - "While in play" condition (*)
			 */
            var abilityCondition = Card!.Ability.Condition;
            var abilityAction = Card!.Ability.Action;
            var target = Card!.Ability.Target;

            if (abilityCondition == "*" && abilityAction != "L+V" && target != null && Owner != null)
            {
                var operation = Card!.Ability.Action!.Contains("+") ? 1 : -1;

                foreach (RangeCell rangeCell in Card!.Range)
                {
                    var dx = col + rangeCell.Offset.x;
                    var dy = row + rangeCell.Offset.y;
                    var isIndexInBounds = dy >= 0 && dy <= 2 && dx >= 0 && dx <= 4;

                    if (rangeCell.Colour.Contains("R") && isIndexInBounds && Card!.Ability.Value != null)
                    {
                        var ownerGrid = Owner.playerIndex == 0 ? game.Player1Grid : game.Player2Grid;
                        var tile = ownerGrid[dy, dx];

                        if (target.Contains("a"))
                        {
                            var amount = -(int)Card!.Ability.Value * operation;
                            game.ChangePower(instigator, this, tile, dy, dx, amount, true, "a");
                        }
                        if (target.Contains("e"))
                        {
                            var amount = -(int)Card!.Ability.Value * operation;
                            game.ChangePower(instigator, this, tile, dy, dx, amount, true, "e");
                        }

                        if (tile.GetCumulativePower() <= 0)
                            game.DestroyCard(instigator, dy, dx, false);
                    }
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
                (target == "a" && tile.Owner?.Id == Owner?.Id) ||
                (target == "e" && tile.Owner?.Id != Owner?.Id) ||
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

            if ((abilityAction == null || abilityAction!.Contains("P")) && Owner != null)
            {
                abilityValue = Card!.Ability.Value == null ? game.PowerTransferQueue.Dequeue() : Card!.Ability.Value;
                game.ChangePower(Owner, this, tile, row, col, (int)abilityValue * operation, isTilePowerBonus, Card.Ability.Target!);
            }
            else if (abilityAction == "destroy" && Owner != null)
            {
                var currPlayerIndex = game.Players.FindIndex(p => p == Owner);
                var grid = currPlayerIndex == 0 ? game.Player1Grid : game.Player2Grid;
                game.DestroyCard(Owner, row, col, false);
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
                    if (tile.Card == null && tile.Owner != null && tile.Owner?.Id == Owner?.Id)
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
            var newWinBonus = player.Scores[row].winBonus + (int)Card!.Ability.Value!;
            player.Scores[row].winBonus = newWinBonus;
        }

        private void HandleAddLaneLoserScoreToVictor(Game game)
        {
            // Look at the lanes the owner is winning and subtract the enemy's points in that lane and subtract winBonus
            for (int i = 0; i < NUM_ROWS; i++)
            {
                var enemy = game.Players.Find(p => p != Owner);
                var laneWinner = game.GetLaneWinner(i);
                if (laneWinner == null) continue;

                // Owner is the victor. Add enemy's score to owner's
                if (laneWinner!.Id == Owner!.Id)
                {
                    Owner!.Scores[i].loserBonus = enemy!.Scores[i].score;
                    enemy!.Scores[i].loserBonus = 0;
                }
                // Enemy is the victor. Add owner's score to enemy's
                else if (laneWinner!.Id == enemy!.Id)
                {
                    enemy!.Scores[i].loserBonus = Owner!.Scores[i].score;
                    Owner!.Scores[i].loserBonus = 0;
                }
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
                    HandleTargetingAbilties(grid[row, col], game, row, col);
                }
                // Raise this card's power by one
                else if (Card!.Ability.Target == "s")
                {
                    CalculateSelfBoostFromPowerModifiedCards(game);
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

        private void CalculateSelfBoostFromPowerModifiedCards(Game game)
        {
            // Prevent out of index error for retrieving the modifier
            string modifier = Card!.Ability.Condition.FirstOrDefault().ToString() ?? "";
            var modifiers = new string[] { "+", "-" };
            if (!modifiers.Contains(modifier)) return;

            var alliesModified = 0;
            var enemiesModified = 0;
            string triggerCondition = Card!.Ability.Condition!;
            var cards = modifier == "+" ? game.EnhancedCards : game.EnfeebledCards;

            foreach (Tile enhancedTile in cards)
            {
                if (enhancedTile.Card == null || Owner == null || !triggerCondition!.Contains(modifier) || enhancedTile.Id == this.Id || enhancedTile.GetBonusPower() == 0)
                    continue;
                if (enhancedTile.Owner!.Id == Owner!.Id)
                    alliesModified++;
                if (enhancedTile.Owner!.Id != Owner.Id)
                    enemiesModified++;
            }

            // This card no longer has any dependencies
            if (alliesModified == 0 && enemiesModified == 0)
            {
                if (game.EnhancedCards.Contains(this))
                {
                    SelfBonusPower = 0;
                    game.RemoveFromEnhancedCards(this);
                }
                return;
            }
                    
            if (triggerCondition.Contains("AE"))
            {
                SelfBonusPower = (int)Card!.Ability.Value! * (alliesModified + enemiesModified);
                SelfEnhanceCleanup(game);
            }
            else if (triggerCondition.Contains("A"))
            {
                SelfBonusPower = (int)Card!.Ability.Value! * alliesModified;
                SelfEnhanceCleanup(game);
            }
            else if (triggerCondition.Contains("E"))
            {
                SelfBonusPower = (int)Card!.Ability.Value! * enemiesModified;
                SelfEnhanceCleanup(game);
            }
        }

        public int GetBonusPower()
        {
            var tileBonusPower = Owner != null ? PlayerTileBonusPower[Owner.playerIndex] : 0;
            return tileBonusPower + CardBonusPower + SelfBonusPower;
        }

        private void SelfEnhanceCleanup(Game game)
        {
            if (!game.EnhancedCards.Contains(this) && SelfBonusPower > 0)
                game.AddToEnhancedCards(this);
        }

        private bool AbilityExecutionThresholdMet(Game game)
        {
            if (Card!.Ability.Condition == "P1R" && GetCumulativePower() >= 7)
            {
                _subscribedEvents.Remove("+P");
                game.OnCardEnhanced -= HandleCardEnhanced;
                return true;
            }
            return false;
        }

        private void HandleCardPlaced(Game game, Tile[,] grid, int row, int col)
        {

            /* Orange tiles in range:
             * - Rank up
             * - Change owner
             */
            var executeAbilityImmediately = Card!.Ability.Condition == "P" || Card!.Ability.Condition == "*" || Card!.Ability.Condition == "R";
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

                if (rangeCell.Colour.Contains("R") && isIndexInBounds && (executeAbilityImmediately || AbilityExecutionThresholdMet(game)))
                {
                    ExecuteAbility(game, grid, dy, dx);
                }


            }

            var target = Card!.Ability.Target;
            var action = Card!.Ability.Action;
            var triggerCondition = Card!.Ability.Condition;

            // If the card doesn't need to listen for other cards being placed, unsubscribe
            if (!OnPlaceConditions.Contains(triggerCondition))
            {
                _subscribedEvents.Remove("P");
                game.OnCardPlaced -= HandleCardPlaced;
            }

            // Cards that boost their own power when other cards are enhanced/enfeebled need to look at the board's status and update
            if ((target == "s") && (triggerCondition.Contains("A") || triggerCondition.Contains("E")))
                CalculateSelfBoostFromPowerModifiedCards(game);

            if (GetCumulativePower() < 0)
                game.DestroyCard(Owner!, row, col, false);

            // Only execute the ability if it has conditions
            if ((action == "add" || action == "spawn" || action == "+Score") && Card!.Ability.Condition != "D")
                ExecuteAbility(game, grid, row, col);

            // If this tile is enhanced or enfeebled prior to this card's placement, invoke the card enhanced/enfeebled event
            if (game.EnhancedCards.Contains(this))
                game.EnqueueOnPowerChange("enhance", grid, row, col);
            else if (game.EnfeebledCards.Contains(this))
                game.EnqueueOnPowerChange("enfeeble", grid, row, col);
        }

        private void HandleCardDestroyed(Player instigator, Game game, Tile[,] grid, int row, int col)
        {
            var action = Card!.Ability.Action;
            // Execute post-mortem ability IFF it is the card being destroyed
            if (Card!.Ability!.Condition == "D" && grid[row, col].Id == this.Id)
            {

                if (Card!.Ability.Target != null)
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

                else if (action == "add")
                    ExecuteAbility(game, grid, row, col);
            }

            if (Card!.Ability.Condition == "AD" && grid[row, col].Owner == Owner || // Ally destroyed
                Card!.Ability.Condition == "ED" && grid[row, col].Owner != Owner || // Enemy destroyed
                Card!.Ability.Condition == "AED") // Either destroyed
            {
                SelfBonusPower += (int)Card!.Ability.Value!;

                if (!game.EnhancedCards.Contains(this))
                    game.AddToEnhancedCards(this);
            }

            if (this == grid[row, col])
                UninitAbility(instigator, game, grid, row, col);
        }

        private void HandleCardEnhanced(Game game, Tile[,] grid, int row, int col)
        {
            var triggerCondition = Card!.Ability.Condition;
            var target = Card!.Ability.Target;

            // Cloud's ability should only execute once it reaches >= 7 power
            if (Card!.Ability.Condition == "P1R" && GetCumulativePower() < 7) return;

            // Unsubscribe so that the ability doesn't trigger again
            if (
                triggerCondition.Contains("1") ||
                (triggerCondition == "P1R" && GetCumulativePower() >= 7) ||
                triggerCondition == "EE")
            {
                _subscribedEvents.Remove("+P");
                _subscribedEvents.Remove("-P");
                game.OnCardEnhanced -= HandleCardEnhanced;
                game.OnCardEnfeebled -= HandleCardEnfeebled;
            }

            // Cards that boost their own power when other cards are enhanced/enfeebled need to look at the board's status and update
            if ((target == "s") && (triggerCondition.Contains("A") || triggerCondition.Contains("E")))
                CalculateSelfBoostFromPowerModifiedCards(game);

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
            var triggerCondition = Card!.Ability.Condition;
            var target = Card!.Ability.Target;

            if (triggerCondition.Contains("1") ||
                triggerCondition == "EE")
            {
                _subscribedEvents.Remove("+P");
                _subscribedEvents.Remove("-P");
                game.OnCardEnfeebled -= HandleCardEnfeebled;
                game.OnCardEnhanced -= HandleCardEnhanced;
            }

            // Cards that boost their own power when other cards are enhanced/enfeebled need to look at the board's status and update
            if ((target == "s") && (triggerCondition.Contains("A") || triggerCondition.Contains("E")))
                CalculateSelfBoostFromPowerModifiedCards(game);


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
            var tileBonusPower = Owner != null ? PlayerTileBonusPower[Owner.playerIndex] : 0;
            return (Card?.Power ?? 0) + tileBonusPower + CardBonusPower + SelfBonusPower;
        }
    }
}
