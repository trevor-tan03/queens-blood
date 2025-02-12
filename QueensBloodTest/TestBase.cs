using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.Data.Sqlite;

namespace QueensBloodTest
{
    public abstract class TestBase
    {
        protected List<Card> _cards = new List<Card>();

        private void PopulateCards(SqliteConnection connection)
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
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Rank = reader.GetInt32(2),
                            Power = reader.GetInt32(3),
                            Rarity = reader.GetString(4),
                            Image = reader.GetString(6),
                        };

                        Ability ability = new Ability()
                        {
                            Description = reader.GetString(5),
                            Condition = reader.GetString(7),
                            Action = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Target = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Value = reader.IsDBNull(10) ? null : reader.GetInt32(10),
                        };

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

        private void SetChildCards(SqliteConnection connection)
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

        public TestBase()
        {
            SQLitePCL.Batteries.Init();

            var connectionString = "Data Source=../../../../backend/QB_card_info.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                PopulateCards(connection);
                SetChildCards(connection);

                CreateGameWithPlayers();
            }
        }

        public Game CreateGameWithPlayers()
        {
            Game game = new Game("myGameId");

            game.AddPlayer("Player1_ID", "Player1");
            game.AddPlayer("Player2_ID", "Player2");

            int[] defaultDeck = { 1, 1, 2, 7, 7, 8, 8, 11, 11, 12, 12, 13, 13, 98, 107 };

            for (int i = 0; i < 15; i++)
            {
                Card card = _cards[defaultDeck[i] - 1];

                foreach (Player player in game.Players)
                {
                    player.Deck.Add(card);
                }
            }

            return game;
        }

        public void SetFirstCardInHand(Game game, Card card)
        {
            game.CurrentPlayer!.Hand[0] = card;
        }

        public void AssertTileState(Tile[,] grid, int row, int column, int expectedRank, string expectedOwner)
        {
            Assert.Equal(expectedRank, grid[row, column].Rank);
            Assert.Equal(expectedOwner, grid[row, column].Owner!.Name);
        }

        public void SetPlayer1Start(Game game)
        {
            game.CurrentPlayer = game.Players[0];
        }
    }
}
