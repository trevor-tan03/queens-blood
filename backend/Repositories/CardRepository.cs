using backend.Models;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;

namespace backend.Repositories
{
	public interface ICardRepository
	{
		List<Card> GetBaseCards();
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
				
				var command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM Cards UNION ALL SELECT * FROM Ranges";

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
                        var card = new Card
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Rank = reader.GetInt32(2),
                            Power = reader.GetInt32(3),
                            Rarity = reader.GetString(4),
                            Image = reader.GetString(6),
                        };

						var Description = reader.GetString(5);
						var Condition = reader.GetString(7);
						var Action = reader.IsDBNull(8) ? null : reader.GetString(8);
						var Target = reader.IsDBNull(9) ? null : reader.GetString(9);
						int? Value = reader.IsDBNull(10) ? null : reader.GetInt32(10);

						Ability ability = new Ability(
							Description,
							Condition,
							Action,
							Target,
							Value);

                        card.Ability = ability;

                        _cards.Add(card);
					}
				}
			}
		}

		public List<Card> GetBaseCards ()
		{
			// There are 145 base cards
			return _cards.GetRange(0, 145);
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
