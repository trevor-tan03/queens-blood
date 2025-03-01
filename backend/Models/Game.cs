using System.Collections;
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
		private Queue<Action> ActionQueue { get; set; } = new Queue<Action>();
		public List<Tile> EnhancedCards { get; set; } = new List<Tile>();
		public List<Tile> EnfeebledCards { get; set; } = new List<Tile>();
		private int _consecutivePasses { get; set; } = 0;
		public int _currentPlayerIndex = 0;

        // Events
        public event Action<Game, Tile[,], int, int> OnCardPlaced;
		public event Action<Game, Tile[,], int, int> OnCardDestroyed;
		public event Action<Game, Tile[,], int, int> OnCardEnhanced;
		public event Action<Game, Tile[,], int, int> OnCardEnfeebled;
		public event Action<Game> OnGameEnd;

        public Game(string id, int? seed = null)
        {
            Id = id;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public void AddPlayer (string playerId, string playerName)
		{
			if (Players.Count >= 2 || Players.Any(p => p.Id == playerId)) return;
			
			var player = new Player(playerId, playerName) { IsHost = Players.Count == 0 };
			Players.Add(player);
		}
		public void MakePlayerHost (string playerId)
		{
			var player = Players.Find(p => p.Id == playerId);
			if (player != null) player.IsHost = true;
		}

		public bool PlayersReady ()
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
                    Player1Grid[i, j] = new Tile();
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

			PickStartingPlayer();
			InitializeBoard();
		}

		public void MulliganCards(Player player, List<int> cardIndices)
		{
			cardIndices.Sort((a,b) => b.CompareTo(a));

			// Add back specified cards back to deck
			foreach(var index in cardIndices)
			{
				var card = player.Hand[index];
				player.Deck.Add(card);
			}

			// If we mulliganed, reshuffle deck and pick up
			if (cardIndices.Count > 0)
			{
				ShuffleDeck(player.Deck);
				foreach(var index in cardIndices)
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

            if (card.Ability.Action == "replace")
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

        public void ChangePower(Tile tile, int row, int col, int amount, bool isTilePowerBonus)
		{
            var grid = _currentPlayerIndex == 0 ? Player1Grid : Player2Grid;

			if (isTilePowerBonus)
				tile.TileBonusPower += amount;
			else
				tile.CardBonusPower += amount;

			if (amount > 0)
			{
                if (!EnhancedCards.Contains(tile)) EnhancedCards.Add(tile);
				ActionQueue.Enqueue(() => OnCardEnhanced?.Invoke(this, grid, row, col));
            }
			else
			{
                if (!EnfeebledCards.Contains(tile)) EnfeebledCards.Add(tile);
				ActionQueue.Enqueue(() => OnCardEnfeebled?.Invoke(this, grid, row, col));

				if (tile.Card != null && tile.GetCumulativePower() <= 0)
					ActionQueue.Enqueue(() => DestroyCard(grid, row, col));
			}
		}

		public void DestroyCard(Tile[,] griddy, int row, int col)
		{
            var grid = _currentPlayerIndex == 0 ? Player1Grid : Player2Grid;
            OnCardDestroyed?.Invoke(this, grid, row, col);
            grid[row, col].Card = null;

			if (EnhancedCards.Contains(grid[row, col]))
				EnhancedCards.Remove(grid[row, col]);
			if (EnfeebledCards.Contains(grid[row, col]))
				EnfeebledCards.Remove(grid[row, col]);

            // Reset card specifc power bonus when destroyed
            grid[row, col].CardBonusPower = 0;
			grid[row, col].SelfBonusPower = 0;
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

					if (tile.Owner?.Id == Players[0].Id)
						player1Score += tilePower;
					else if (tile.Owner?.Id == Players[1].Id)
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
                if (winner == Players[0])
                    player1Total += winner.Scores[row].winBonus;
                else if (winner == Players[1])
                    player2Total += winner.Scores[row].winBonus;
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

		private void ExecuteQueuedActions()
		{
			while (ActionQueue.Count > 0)
			{
				Action action = ActionQueue.Dequeue();
				action();
			}
		}

		public bool PlaceCard(int handIndex, int row, int col)
		{
			var playerIndex = Players.FindIndex(p => p.Id == CurrentPlayer!.Id);
			var card = CurrentPlayer!.Hand[handIndex];
			var grid = playerIndex == 0 ? Player1Grid : Player2Grid;
			var tile = grid[row, col];

			if (CanPlaceCard(card, tile))
			{
				_consecutivePasses = 0; // Reset to keep game going

                // Invoke card destroyed if replace card
                if (card.Ability.Condition == "R")
                    DestroyCard(grid, row, col);

                tile.Card = card;
                CurrentPlayer.Hand.RemoveAt(handIndex);

                tile.InitAbility(this);
                OnCardPlaced?.Invoke(this, grid, row, col);

				CalculatePlayerScores();
				ExecuteQueuedActions();
				return true;
            }

			return false;
		}

		public void EndGame()
		{
			CurrentPlayer = null;
			GameOver = true;
			OnGameEnd?.Invoke(this);
		}
	}
}
