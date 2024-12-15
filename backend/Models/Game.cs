using System.Collections;

namespace backend.Models
{
	public class Game
	{
		public string Id { get; set; }
		public List<Player> Players { get; set; } = new List<Player>();

		public Game(string id) { Id = id; }

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

		public void Start()
		{
			// Shuffle each player's deck and add first 5 to their hand
			foreach (Player player in Players)
			{
				ShuffleDeck(player.Deck);
				player.PickUp(5);
			}

			// Decide who the starting player is

			// Initialize board
		}
	}
}
