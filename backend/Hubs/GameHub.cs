using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.SignalR;
using HashidsNet;

namespace backend.Hubs
{
	public class GameHub : Hub
	{
		private IGameRepository _gameRepository;
		private ICardRepository _cardRepository;

		public GameHub(IGameRepository gameRepository, ICardRepository cardRepository)
		{
			_gameRepository = gameRepository;
			_cardRepository = cardRepository;
		}

		private async Task SendErrorMessage(string message)
		{
			await Clients.Client(Context.ConnectionId).SendAsync("ErrorMessage", message);
		}

		private async Task<(Game?, Player?)> FetchGameAndPlayer(string gameId)
		{
			var game = _gameRepository.GetGameById(gameId);
			var player = game?.Players.Find(p => p.Id == Context.ConnectionId);

			if (game == null)
			{
				await SendErrorMessage("Game not found.");
			} else if (player == null)
			{
				await SendErrorMessage("Player not found.");
			}
			return (game, player);
		}

		private string GenerateUniqueGameId()
		{
			string gameId;
			Game? game;

			do
			{
				gameId = new Hashids(Context.ConnectionId, 6).Encode(DateTime.Now.Second);
				game = _gameRepository.GetGameById(gameId);
			} while (game != null);

			return gameId;
		}

		public async Task CreateGame(string playerName)
		{
			var gameId = GenerateUniqueGameId();
			await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
			_gameRepository!.AddGame(gameId);
			var game = _gameRepository.GetGameById(gameId);
			game!.AddPlayer(Context.ConnectionId, playerName);

			// Send message saying the host has connected
			await Clients.Group(gameId).SendAsync("ReceiveMessage", $"{playerName} connected.");

			// Send out the list of players in the game
			await Clients.Group(gameId).SendAsync("ReceivePlayerList", game.Players);

			// Send the game code to the host
			await Clients.Client(Context.ConnectionId).SendAsync("GameCode", gameId);
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
				await SendErrorMessage("Game is full.");
			} 
		}

		public async Task LeaveGame(string gameId)
		{
			var (game, player) = await FetchGameAndPlayer(gameId);

			if (game != null && player != null)
			{
				game.Players.Remove(player);
				if (game.Players.Count == 0)
				{
					_gameRepository.RemoveGame(gameId);
				} else if (player.IsHost)
				{
					// Transfer ownership of game
					game.Players[0].IsHost = true;
				}

				await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
				await Clients.Group(gameId).SendAsync("ReceiveMessage", $"{player.Name} disconnected.");
				await Clients.Group(gameId).SendAsync("ReceivePlayerList", game.Players);
			} else
			{
				// Will say game not found even if the game exists, but the user isn't in that particular game
				await SendErrorMessage("Game not found.");
			}
		}

		public async Task ToggleReady(string gameId, bool isReadyUp, List<int> cardIds)
		{
			var (game, player) = await FetchGameAndPlayer(gameId);

			if (game != null && player != null)
			{
				// If the player is trying to "Ready up", make sure that their deck is valid
				if (isReadyUp && _cardRepository.IsDeckLegal(cardIds))
				{
					var deck = new List<Card>();

					foreach (var cardId in cardIds)
					{
						deck.Add(_cardRepository.GetCardById(cardId));
					}

					// Mark player as ready and update the player's deck
					player.Ready(deck);

					// If all players are ready then start the match
					if (game.PlayersReady())
					{
						game.Start();
						// Update the player's screens
						await Clients.Group(gameId).SendAsync("GameStart", true);
					}
                }
				else if (!isReadyUp)
				{
					player.Unready();
				} else
				{
					await SendErrorMessage($"{player.Name} does not have a valid deck.");
				}

				await Clients.Group(gameId).SendAsync("ReceivePlayerList", game.Players);
			}
		}

		public async Task SendMessage(string gameId, string message)
		{
			var (game, player) = await FetchGameAndPlayer(gameId);

			if (game != null && player != null)
			{
				await Clients.Group(gameId).SendAsync("ReceiveMessage", $"{player.Name}: {message}");
			}
		}

		public async Task GetHand(string gameId)
		{
			var (game, player) = await FetchGameAndPlayer(gameId);

			if (game != null && player != null)
			{
				await Clients.Client(Context.ConnectionId).SendAsync("CardsInHand", player.Hand);
			}
		}

		public async Task MulliganCards(string gameId, List<int> handIndices)
		{
			var (game, player) = await FetchGameAndPlayer(gameId);

			if (game == null || player == null) { return; }

			game.MulliganCards(player, handIndices);
			player.HasMulliganed = true;
			await Clients.Client(Context.ConnectionId).SendAsync("CardsInHand", player.Hand);

			var bothPlayersMulliganed = game.Players.All(p => p.HasMulliganed);

			if (bothPlayersMulliganed)
			{
				await Clients.Group(gameId).SendAsync("MulliganPhaseEnded", true);
			}
		}

	}
}
