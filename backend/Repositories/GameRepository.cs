using backend.Models;
using System.Numerics;

namespace backend.Repositories
{
	public interface IGameRepository
	{
		Game? GetGameById(string id);
		Boolean AddGame(string id, string playerName);
		Boolean RemoveGame(string gameId);
	}

	public class GameRepository : IGameRepository
	{
		private readonly Dictionary<string, Game> _games = new Dictionary<string, Game>();

		public Game? GetGameById(string gameId) 
		{
			_games.TryGetValue(gameId, out var game);
			return game;
		}

		// Adds game to dictionary and adds host player to list of players inside the Game object
		public bool AddGame(string id, string playerName) 
		{
			if (_games.ContainsKey(id))
				return false;

			Game game = new(id);

			_games.Add(id, game);

			return true;
		}

		public bool RemoveGame(string gameId)
		{
			return _games.Remove(gameId);
		}
	}
}
