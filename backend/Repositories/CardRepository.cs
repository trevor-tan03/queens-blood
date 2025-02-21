using backend.Models;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;
using backend.Utility;
using backend.DTO;

namespace backend.Repositories
{
    public interface ICardRepository
	{
		List<CardDTO> GetBaseCards();
		Card GetCardById(int id);
		Boolean IsDeckLegal(List<int> cardIds);
	}

	public class CardRepository : ICardRepository
	{
		private readonly List<Card> _cards = new List<Card>();
		private readonly int MAX_STANDARD = 2;
		private readonly int MAX_LEGENDARY = 1;

		public CardRepository()
		{
			SQLitePCL.Batteries.Init();

			var connectionString = "Data Source=QB_card_info.db";
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				ReadDatabase.PopulateCards(connection, _cards);
				ReadDatabase.SetChildCards(connection, _cards);
			}
		}

		/* This should only be called when outside of a game
		   e.g. In lobby or main menu */
		public List<CardDTO> GetBaseCards ()
		{
			// There are 145 base cards
			var baseCards = _cards.GetRange(0, 145);
            return DTOConverter.GetCardDTOList(baseCards);
		}

		public Card GetCardById (int id)
		{
			// Decrement since list starts at index 0.
			return _cards[id-1];
		}

		public Boolean IsDeckLegal(List<int> cardIds)
		{
			if (cardIds.Count != 15) return false;

			var compressedDeck = new Dictionary<int, int>();

			foreach (var cardId in cardIds)
			{
				if (!compressedDeck.ContainsKey(cardId))
				{
					compressedDeck.Add(cardId, 1);
				} else
				{
					compressedDeck[cardId]++;
				}
			}

			foreach (var cardId in compressedDeck.Keys)
			{
				Card card = GetCardById(cardId);
				if (
					(card.Rarity == "Standard" && compressedDeck[cardId] > MAX_STANDARD) ||
					(card.Rarity == "Legendary" && compressedDeck[cardId] > MAX_LEGENDARY)
				) {
					return false;
				}
			}

			return true;
		}
	}
}
