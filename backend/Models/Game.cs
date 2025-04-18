﻿using System.Collections;
using System.Numerics;
using static backend.Models.TileConstants;

namespace backend.Models
{
    public class Game
    {
        public string Id { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public Tile[,] Player1Grid = new Tile[3, 5];
        public Tile[,] Player2Grid = new Tile[3, 5];
        public Player? CurrentPlayer { get; set; }
        public Random _random { get; set; }
        public bool GameOver { get; set; }
        public Queue<Action> ActionQueue { get; set; } = new Queue<Action>();
        public Queue<Action> SelfEnhanceQueue { get; set; } = new Queue<Action>();
        public List<Tile> EnhancedCards { get; set; } = new List<Tile>();
        public List<Tile> EnfeebledCards { get; set; } = new List<Tile>();
        private int _consecutivePasses { get; set; } = 0;
        public int _currentPlayerIndex = 0;
        public int PowerTransferAmount { get; set; } = 0;
        public int UPAsPlaced { get; set; } = 0;

        // Events
        public event Action<Game, Tile[,], int, int> OnCardPlaced;
        public event Action<Game, Tile[,], int, int> OnCardDestroyed;
        public event Action<Game, Tile[,], int, int> OnCardEnhanced;
        public event Action<Game, Tile[,], int, int> OnCardEnfeebled;
        public event Action<Game> OnRoundEnd;
        /*
         * Cards which self-enhance need to know when cards are added or removed
         * from the EnhancedCards/EnfeebledCards list
         */
        public event Action<Game> OnEnhancedCardsChanged;
        public event Action<Game> OnEnfeebledCardsChanged;

        public Game(string id, int? seed = null)
        {
            Id = id;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public void AddPlayer(string playerId, string playerName)
        {
            if (Players.Count >= 2 || Players.Any(p => p.Id == playerId)) return;

            if (Players.Exists(p => p.Name == playerName))
                playerName = $"{playerName} (1)";

            var player = new Player(playerId, playerName) { IsHost = Players.Count == 0 };
            Players.Add(player);
        }
        public void MakePlayerHost(string playerId)
        {
            var player = Players.Find(p => p.Id == playerId);
            if (player != null) player.IsHost = true;
        }

        public bool PlayersReady()
        {
            // Returns true if all players in the game are ready
            return Players.Count == 2 && Players[0].IsReady && Players[1].IsReady;
        }

        private void ShuffleDeck(List<Card> deck)
        {
            int n = deck.Count;

            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                Card card = deck[k];
                deck[k] = deck[n];
                deck[n] = card;
            }
        }

        private void InitializeBoard()
        {
            // Populate player 1 board and set initial owner
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < NUM_COLS; j++)
                {
                    Player1Grid[i, j] = new Tile($"{i}{j}");
                }
                Player1Grid[i, 0].Owner = Players[0];
                Player1Grid[i, 4].Owner = Players[1]; // Setting owner for player 2's view
            }

            // Mirror player 1's board for player 2
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < NUM_COLS; j++)
                {
                    Player2Grid[i, j] = Player1Grid[i, 4 - j];
                }
            }

            // Each player will be the owner of the first column on their respective boards
            for (int i = 0; i < 3; i++)
            {
                Player1Grid[i, 0].Owner = Players[0];
                Player1Grid[i, 0].Rank = 1;
                Player2Grid[i, 0].Owner = Players[1];
                Player2Grid[i, 0].Rank = 1;
            }
        }

        private void PickStartingPlayer()
        {
            _currentPlayerIndex = _random.Next(0, 2);
            CurrentPlayer = Players[_currentPlayerIndex];
        }


        public void Start()
        {
            // Shuffle each player's deck and add first 5 to their hand
            foreach (Player player in Players)
            {
                ShuffleDeck(player.Deck);
                player.PickUp(5);
            }

            Players[0].playerIndex = Players.FindIndex(p => p == Players[0]);
            Players[1].playerIndex = (Players[0].playerIndex + 1) % 2;

            PickStartingPlayer();
            InitializeBoard();
        }

        public void MulliganCards(Player player, List<int> cardIndices)
        {
            cardIndices.Sort((a, b) => b.CompareTo(a));

            // Add back specified cards back to deck
            foreach (var index in cardIndices)
            {
                var card = player.Hand[index];
                player.Deck.Add(card);
            }

            // If we mulliganed, reshuffle deck and pick up
            if (cardIndices.Count > 0)
            {
                ShuffleDeck(player.Deck);
                foreach (var index in cardIndices)
                {
                    var card = player.Deck[0];
                    player.Deck.RemoveAt(0);
                    player.Hand[index] = card;
                }
            }
        }

        private bool CanPlaceCard(Card card, Tile tile)
        {
            var tileMeetsRankRequirement = tile.Rank >= card.Rank;
            var playerOwnsTile = tile.Owner?.Id == CurrentPlayer!.Id;
            var tileOccupied = tile.Card != null;

            if (card.Ability.Condition == "R")
            {
                // Replace cards can only be placed on occupied tiles, regardless of rank
                return playerOwnsTile && tileOccupied;
            }

            return tileMeetsRankRequirement && playerOwnsTile && !tileOccupied;
        }

        public void SwapPlayerTurns()
        {
            CurrentPlayer = Players[_currentPlayerIndex];

            if (CurrentPlayer == null)
            {
                throw new InvalidOperationException($"Current player is not set. {_currentPlayerIndex}");
            }

            _currentPlayerIndex = (_currentPlayerIndex + 1) % 2;
            CurrentPlayer = Players[_currentPlayerIndex];

            if (_consecutivePasses == 2)
                EndGame();
        }

        public void Pass()
        {
            _consecutivePasses++;
            SwapPlayerTurns();
        }

        public void ChangePower(Player instigator, Tile instigatorTile, Tile tile, int row, int col, int amount, bool isTilePowerBonus, string target)
        {
            var grid = _currentPlayerIndex == 0 ? Player1Grid : Player2Grid;

            if (isTilePowerBonus)
            {
                var enemyIndex = (instigator.playerIndex + 1) % 2;

                // Decide which player to give the bonus power to
                switch (target)
                {
                    // Target the instigator's allies (a), enemies (e), or both (ae)
                    case "a":
                        tile.PlayerTileBonusPower[instigator.playerIndex] += amount;
                        break;
                    case "e":
                        tile.PlayerTileBonusPower[enemyIndex] += amount;
                        break;
                    case "ae":
                        tile.PlayerTileBonusPower[instigator.playerIndex] += amount;
                        tile.PlayerTileBonusPower[enemyIndex] += amount;
                        break;
                    default:
                        throw new Exception($"An error occurred when changing the power of tile ({row}, {col})");
                }
                
                
            }
            else if (tile.Card != null)
                tile.CardBonusPower += amount;

            var tileBonus = tile.PlayerTileBonusPower[0] + tile.PlayerTileBonusPower[1];
            var bonusPower = tileBonus + tile.SelfBonusPower + tile.CardBonusPower;

            // Classify as Enhanced
            if (!EnhancedCards.Contains(tile) && bonusPower > 0)
            {
                AddToEnhancedCards(tile);
                RemoveFromEnfeebledCards(tile);
                ActionQueue.Enqueue(() => OnCardEnhanced?.Invoke(this, grid, row, col));
            }
            // Special case for Cloud's (first to reach 7 power) ability
            else if (tile.Card != null && tile.Card.Ability.Condition == "P1R" && tile.GetCumulativePower() >= 7)
            {
                // Won't retrigger ability once it's been executed
                ActionQueue.Enqueue(() => OnCardEnhanced?.Invoke(this, grid, row, col));
            }
            // Classify as Enfeebled
            else if (!EnfeebledCards.Contains(tile) && bonusPower < 0)
            {
                AddToEnfeebledCards(tile);
                RemoveFromEnhancedCards(tile);
                ActionQueue.Enqueue(() => OnCardEnfeebled?.Invoke(this, grid, row, col));

                if (tile.Card != null && instigator != null && tile.GetCumulativePower() <= 0)
                {
                    ActionQueue.Enqueue(() => DestroyCard(instigator, row, col, false));
                }
            }
            else if ((EnhancedCards.Contains(tile) && bonusPower <= 0) ||
                (EnfeebledCards.Contains(tile) && bonusPower >= 0))
            {
                RemoveFromEnhancedCards(tile);
                RemoveFromEnfeebledCards(tile);
            }
        }

        public void EnqueueOnPowerChange(string type, Tile[,] grid, int row, int col)
        {
            /* Used to re-invoke event when it's already been considered
             * e.g. Cactuar enhances tile while in play
             *		Chocobo & Moogle will dismiss the enhanced empty tile
             *		---
             *		When you play a card, it should reinvoke the event so that 
             *		Chocobo & Moogle updates its SelfBonusPower
			 */
            if (type == "enhance")
                ActionQueue.Enqueue(() => OnEnhancedCardsChanged?.Invoke(this));
            else
                ActionQueue.Enqueue(() => OnEnfeebledCardsChanged?.Invoke(this));
        }

        public void RemoveFromEnhancedCards(Tile tile)
        {
            if (EnhancedCards.Contains(tile))
            {
                EnhancedCards.Remove(tile);
                ActionQueue.Enqueue(() => OnEnhancedCardsChanged?.Invoke(this));
            }
        }

        public void AddToEnhancedCards(Tile tile)
        {
            if (!EnhancedCards.Contains(tile))
            {
                EnhancedCards.Add(tile);

                if (tile.Card != null)
                    ActionQueue.Enqueue(() => OnEnhancedCardsChanged?.Invoke(this));
            }
        }

        private void RemoveFromEnfeebledCards(Tile tile)
        {
            if (EnfeebledCards.Contains(tile))
            {
                EnfeebledCards.Remove(tile);

                if (tile.Card != null)
                    ActionQueue.Enqueue(() => OnEnfeebledCardsChanged?.Invoke(this));
            }
        }

        private void AddToEnfeebledCards(Tile tile)
        {
            if (!EnfeebledCards.Contains(tile))
            {
                EnfeebledCards.Add(tile);

                if (tile.Card != null)
                    ActionQueue.Enqueue(() => OnEnfeebledCardsChanged?.Invoke(this));
            }
        }

        public void DestroyCard(Player instigator, int row, int col, bool isTransferPower)
        {
            var grid = _currentPlayerIndex == 0 ? Player1Grid : Player2Grid;
            var tile = grid[row, col];

            if (tile == null) return;

            OnCardDestroyed?.Invoke(this, grid, row, col);
            if (grid[row, col].Card != null && grid[row, col].Card!.Name == "Ultimate Party Animal")
                UPAsPlaced--;

            int cumulativePower = tile.GetCumulativePower();
            var tileBonusPower = grid[row, col].PlayerTileBonusPower[instigator.playerIndex];

            tile.RemoveCard(instigator, this, grid, row, col);

            // Remove card-specific bonuses (not tile bonus)
            grid[row, col].CardBonusPower = 0;
            grid[row, col].SelfBonusPower = 0;

            // Remove from EnhancedCards if the tile is not enhanced
            if (grid[row, col].PlayerTileBonusPower[0] <= 0 && grid[row, col].PlayerTileBonusPower[1] <= 0)
                RemoveFromEnhancedCards(grid[row, col]);

            // Remove from EnfeebledCards if the tile is not enfeebled
            if (grid[row, col].PlayerTileBonusPower[0] >= 0 && grid[row, col].PlayerTileBonusPower[1] >= 0)
                RemoveFromEnfeebledCards(grid[row, col]);
            if (isTransferPower)
                PowerTransferAmount = cumulativePower;

            grid[row, col].Owner = instigator;
        }

        private void CalculatePlayerScores()
        {
            for (int row = 0; row < NUM_ROWS; row++)
            {
                var player1Score = 0;
                var player2Score = 0;

                for (int col = 0; col < NUM_COLS; col++)
                {
                    var tile = Player1Grid[row, col];
                    int tilePower = tile.GetCumulativePower();

                    if (tile.Owner?.Id == Players[0].Id && tile.Card != null)
                        player1Score += tilePower;
                    else if (tile.Owner?.Id == Players[1].Id && tile.Card != null)
                        player2Score += tilePower;
                }

                Players[0].Scores[row].score = player1Score;
                Players[1].Scores[row].score = player2Score;
            }
        }

        public (int player1Score, int player2Score) GetFinalScores()
        {
            var player1Total = 0;
            var player2Total = 0;

            for (int row = 0; row < NUM_ROWS; row++)
            {
                player1Total += Players[0].Scores[row].score;
                player2Total += Players[1].Scores[row].score;

                var winner = GetLaneWinner(row);
                var winBonus = winner != null ? winner.Scores[row].winBonus + winner.Scores[row].loserBonus : 0;

                if (winner == Players[0])
                    player1Total += winBonus;
                else if (winner == Players[1])
                    player2Total += winBonus;
            }

            return (player1Total, player2Total);
        }

        public Player? GetLaneWinner(int row)
        {
            if (Players[0].Scores[row].score > Players[1].Scores[row].score)
                return Players[0];
            else if (Players[0].Scores[row].score < Players[1].Scores[row].score)
                return Players[1];
            else
                return null;
        }

        public int GetPlayerLaneScore(int playerIndex, int row)
        {
            return Players[playerIndex].Scores[row].score;
        }

        public int GetPlayerLaneBonus(int playerIndex, int row)
        {
            var winBonus = Players[playerIndex].Scores[row].winBonus;
            var loserBonus = Players[playerIndex].Scores[row].loserBonus;
            return  winBonus + loserBonus;
        }

        private void ExecuteQueuedActions()
        {
            while (ActionQueue.Count > 0)
            {
                Action action = ActionQueue.Dequeue();
                action();
            }

            while (SelfEnhanceQueue.Count > 0)
            {
                var action = SelfEnhanceQueue.Dequeue();
                action();
            }
        }

        public bool PlaceCard(int handIndex, int row, int col)
        {
            var card = CurrentPlayer!.Hand[handIndex];
            var grid = _currentPlayerIndex == 0 ? Player1Grid : Player2Grid;
            var tile = grid[row, col];

            if (CanPlaceCard(card, tile))
            {
                _consecutivePasses = 0; // Reset to keep game going

                // Invoke card destroyed if replace card
                if (card.Ability.Condition == "R")
                {
                    var isTransferPower = card.Ability.Value == null && card.Ability.Action != "replace";
                    DestroyCard(CurrentPlayer, row, col, isTransferPower);
                }

                tile.Card = card;
                CurrentPlayer.Hand.RemoveAt(handIndex);

                tile.InitAbility(this);
                OnCardPlaced?.Invoke(this, grid, row, col);

                PowerTransferAmount = 0;

                ExecuteQueuedActions();
                CalculatePlayerScores();
                OnRoundEnd?.Invoke(this);
                return true;
            }

            return false;
        }

        public void EndGame()
        {
            CurrentPlayer = null;
            GameOver = true;
        }
    }
}
