using System.Collections;
using System.Numerics;

namespace backend.Models
{
	public class Game
	{
		public string Id { get; set; }
		public List<Player> Players { get; set; } = new List<Player>();
        public Tile[,] Player1Grid = new Tile[3, 5];
		public Tile[,] Player2Grid = new Tile[3, 5];
		public Player? currentPlayer { get; set; }
		public Random Random { get; set; }

        public Game(string id) { Id = id; }
		public Game(string id, int seed)
		{
			Id = id;
			Random = new Random(seed);
		}

		public void AddPlayer (string playerId, string playerName)
		{
			var player = new Player(playerId, playerName);
			if (Players.Count == 0)
			{
				player.IsHost = true;
			}

			if (Players.Count < 2 && !Players.Any(player => player.Id == playerId))
			{
				Players.Add (player);
			}
		}
		public void MakePlayerHost (string playerId)
		{
			var player = Players.Find(p => p.Id == playerId);
			player!.IsHost = true;
		}

		public bool PlayersReady ()
		{
			// Returns true if all players in the game are ready
			return Players.Count == 2 && Players[0].IsReady && Players[1].IsReady;
		}

		private void ShuffleDeck(List<Card> deck)
		{
			Random rng = new Random();
			int n = deck.Count;

			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				Card card = deck[k];
				deck[k] = deck[n];
				deck[n] = card;
			}
		}

        private void InitializeBoard()
        {
            // Populate player 1 board and set initial owner
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Player1Grid[i, j] = new Tile();
                }
                Player1Grid[i, 0].Owner = Players[0];
                Player1Grid[i, 4].Owner = Players[1]; // Setting owner for player 2's view
            }

            // Mirror player 1's board for player 2
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Player2Grid[i, j] = Player1Grid[i, 4 - j];
                }
            }

            // Each player will be the owner of the first column on their respective boards
            for (int i = 0; i < 3; i++)
            {
                Player1Grid[i, 0].Owner = Players[0];
                Player2Grid[i, 0].Owner = Players[1];
            }
        }

		private void PickStartingPlayer()
		{
			Random rand = new Random();
			currentPlayer = Players[rand.Next(0,2)];
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
			var playerOwnsTile = tile.Owner == currentPlayer;
			var tileNotOccupied = tile.Card == null;

			return tileMeetsRankRequirement && playerOwnsTile && tileNotOccupied;
        }

		public void PlaceCard(int handIndex, int row, int col)
		{
			var playerIndex = Players.IndexOf(currentPlayer!);
			var card = currentPlayer!.Hand[handIndex];
			var grid = playerIndex == 0 ? Player1Grid : Player2Grid;
			var tile = grid[row, col];

			if (CanPlaceCard(card, tile))
			{
                tile.Card = card;

                // Rank up tiles in range

                // Effect
            }
		}
	}
}
