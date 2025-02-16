using backend.Models;
using Microsoft.Data.Sqlite;

namespace backend.Utility
{
    public class ReadDatabase
    {
        public static void PopulateCards(SqliteConnection connection, List<Card> _cards)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                    SELECT c.*, r.Offset, r.Colour 
                    FROM Cards c 
                    LEFT JOIN Ranges r 
                    ON c.Id = r.CardId;";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var card = _cards.Find(c => c.Id == reader.GetInt32(0));
                    var offsetString = reader.IsDBNull(11) ? null : reader.GetString(11);
                    var colour = reader.IsDBNull(12) ? null : reader.GetString(12);

                    if (card != null && offsetString != null && colour != null)
                    {
                        card.AddRangeCell(offsetString, colour);
                    }
                    else
                    {
                        card = new Card
                        (
                            reader.GetInt32(0), // ID
                            reader.GetString(1), // Name
                            reader.GetInt32(2), // Rank
                            reader.GetInt32(3), // Power
                            reader.GetString(4), // Rarity
                            reader.GetString(6) // Image
                        );

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

                        if (card.Ability.Action == "+R" && ability.Value != null)
                        {
                            card.RankUpAmount = (int)ability!.Value;
                        }

                        if (offsetString != null && colour != null)
                        {
                            card.AddRangeCell(offsetString, colour);
                        }

                        _cards.Add(card);
                    }
                }
            }
        }

        public static void SetChildCards(SqliteConnection connection, List<Card> _cards)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM ParentChild";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var parentID = reader.GetInt32(0);
                    var childID = reader.GetInt32(1);

                    _cards[parentID - 1].AddChild(_cards[childID - 1]);
                }
            }
        }

    }
}
