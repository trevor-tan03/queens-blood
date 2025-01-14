using backend;
using backend.Models;
using backend.Repositories;
using Microsoft.Data.Sqlite;

namespace QueensBloodTest
{
    public class TestBoard
    {

        private List<Card> _cards = new List<Card>();

        public TestBoard()
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

            CreateGameWithPlayers();
        }

        private Game CreateGameWithPlayers()
        {
            Game game = new Game("myGameId");

            game.AddPlayer("Player1_ID", "Player1");
            game.AddPlayer("Player2_ID", "Player2");

            int[] defaultDeck = { 1, 1, 2, 7, 7, 8, 8, 11, 11, 12, 12, 13, 13, 98, 107 };

            for (int i = 0; i < 15; i++)
            {
                Card card = _cards[defaultDeck[i]-1];

                foreach (Player player in game.Players)
                {
                    player.Deck.Add(card);
                }
            }

            return game;
        }

        [Fact]
        public void CreateGameWithTwoPlayersEachFifteenCards()
        {
            Game game = CreateGameWithPlayers();

            Assert.Equal(2, game.Players.Count);
            Assert.Equal("Player1", game.Players[0].Name);
            Assert.Equal("Player2", game.Players[1].Name);

            Assert.Equal(15, game.Players[0].Deck.Count);
            Assert.Equal(15, game.Players[1].Deck.Count);
        }

        [Fact]
        public void PlayersHaveDefaultDeck()
        {
            Game game = CreateGameWithPlayers();

            var p1 = game.Players[0];
            string[] expectedCards = { 
                "Security Officer", 
                "Security Officer", 
                "Riot Trooper",
                "Levrikon",
                "Levrikon",
                "Grasslands Wolf",
                "Grasslands Wolf",
                "Elphadunk",
                "Elphadunk",
                "Cactuar", 
                "Cactuar",
                "Crystalline Crab",
                "Crystalline Crab",
                "Titan", 
                "Chocobo & Moogle" 
            };

            Assert.Equal(15, expectedCards.Length);

            for (int i = 0; i < 15; i++)
            {
                Assert.Equal(expectedCards[i], p1.Deck[i].Name);
            }
        }

        [Fact]
        public void MoveCardsToDeckAfterGameStarts()
        {
            var game = CreateGameWithPlayers();
            game.Start();

            Assert.Equal(5, game.Players[0].Hand.Count);
            Assert.Equal(10, game.Players[0].Deck.Count);
        }


    }
}