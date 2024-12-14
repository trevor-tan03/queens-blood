using backend.Models;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;

namespace backend.Repositories
{
	public interface ICardRepository
	{
		List<Card> GetBaseCards();
	}

	public class CardRepository : ICardRepository
	{
		private readonly List<Card> _cards = new List<Card>();

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
						};

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
	}
}
