using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.SignalR;
using HashidsNet;

namespace backend.Hubs
{
	public class GameHub : Hub
	{
		private IGameRepository _gameRepository;

		public GameHub(IGameRepository gameRepository)
		{
			_gameRepository = gameRepository;
		}

		public async Task SendMessage(string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", message);
		}

		public async Task CreateGame(string playerName)
		{
			var gameId = new Hashids(Context.ConnectionId, 6).Encode(123456);
			var game = _gameRepository.GetGameById(gameId);

			if (game == null)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
				_gameRepository!.AddGame(gameId, playerName);
				game = _gameRepository!.GetGameById(gameId);
				game!.AddPlayer(Context.ConnectionId, playerName);

				// Send message saying the host has connected
				await Clients.Group(gameId).SendAsync("ReceiveMessage", $"{playerName} connected.");

				// Send out the list of players in the game
				await Clients.Group(gameId).SendAsync("ReceivePlayerList", game.Players);

				// Send the game code to the host
				await Clients.Client(Context.ConnectionId).SendAsync("GameCode", gameId);
			}
		}

		public async Task JoinGame(string gameId, string playerName)
		{
			var game = _gameRepository.GetGameById(gameId);

			if (game != null && game.Players.Count < 2)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
				_gameRepository!.GetGameById(gameId)!.AddPlayer(Context.ConnectionId, playerName);
				await Clients.Group(gameId).SendAsync("ReceiveMessage", $"{playerName} connected.");
				await Clients.Group(gameId).SendAsync("ReceivePlayerList", game.Players);
			} 
			else if (game != null) {
				await Clients.Group(gameId).SendAsync("ErrorMessage", $"Game is full.");
			} 
			else
			{
				await Clients.Client(Context.ConnectionId).SendAsync("ErrorMessage", $"Game not found.");
			}
		}

		public async Task LeaveGame(string gameId)
		{
			var game = _gameRepository.GetGameById(gameId);
			var player = game?.Players.Find(x => x.Id == Context.ConnectionId);

			if (game != null && player != null)
			{
				game.Players.Remove(player);
				if (game.Players.Count == 0)
				{
					_gameRepository.RemoveGame(gameId);
				}

				await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
				await Clients.Group(gameId).SendAsync("ReceiveMessage", $"{player.Name} disconnected.");
				await Clients.Group(gameId).SendAsync("ReceivePlayerList", game.Players);
			} else
			{
				// Will say game not found even if the game exists, but the user isn't in that particular game
				await Clients.Client(Context.ConnectionId).SendAsync("ErrorMessage", "Game not found.");
			}
		}
	}
}
