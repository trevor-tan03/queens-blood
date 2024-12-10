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

			if (Players.Count < 2 && !Players.Any(player => player.Id == playerId))
			{
				Players.Add (player);
			}
		}
	}
}
