using backend.Models;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;
using System.Collections.Generic;

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
				command.CommandText = "SELECT * FROM Cards";

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
							Ability = reader.GetString(5),
							Image = reader.GetString(6),
							Condition = reader.GetString(7),
						};

						for (int i = 8; i <= 10; i++)
						{
							var isDbNull = reader.IsDBNull(i);

							if (isDbNull) break;
							switch (i)
							{
								case 8:
									card.Action = reader.GetString(i);
									break;
								case 9:
									card.Target = reader.GetString(i);
									break;
								default:
									card.Value = reader.GetInt32(i);
									break;
							}
						}

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
