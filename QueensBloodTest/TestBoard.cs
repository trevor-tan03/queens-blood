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
                        } else
                        {
                            card = new Card
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Rank = reader.GetInt32(2),
                                Power = reader.GetInt32(3),
                                Rarity = reader.GetString(4),
                                Ability = reader.GetString(5),
                                Image = reader.GetString(6),
                                Condition = reader.GetString(7),
                                Action = reader.IsDBNull(8) ? null : reader.GetString(8),
                                Target = reader.IsDBNull(9) ? null : reader.GetString(9),
                                Value = reader.IsDBNull(10) ? null : reader.GetInt32(10),
                            };

                            if (offsetString != null && colour != null)
                            {
                                card.AddRangeCell(offsetString, colour);
                            }

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

            Assert.Equal(5, game.Players[1].Hand.Count);
            Assert.Equal(10, game.Players[1].Deck.Count);
        }

        [Fact]
        public void BoardInitializedCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();

            var p1 = game.Players[0];
            var p2 = game.Players[1];

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(p1, game.Player1Grid[i, 0].Owner);
                Assert.Equal(p2, game.Player1Grid[i, 4].Owner);

                Assert.Equal(p2, game.Player2Grid[i, 0].Owner);
                Assert.Equal(p1, game.Player2Grid[i, 4].Owner);
            }
        }

        [Fact]
        public void CheckCurrentPlayerSelected()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            Assert.NotNull(game.currentPlayer);

        }

        [Fact]
        public void InitializeCardRangeCorrectly()
        {
            var redXIII = _cards[89];
            Assert.Equal(10, redXIII.Range.Count);
            var expectedRedXIIIRanges = new List<RangeCell>()
            {
                new RangeCell() { Colour = "R", Offset = (-2,-2) },
                new RangeCell() { Colour = "R", Offset = (-2,0) },
                new RangeCell() { Colour = "R", Offset = (-2,2) },
                new RangeCell() { Colour = "O", Offset = (0,-1) },
                new RangeCell() { Colour = "R", Offset = (0,-2) },
                new RangeCell() { Colour = "R", Offset = (0,2) },
                new RangeCell() { Colour = "O", Offset = (1,0) },
                new RangeCell() { Colour = "R", Offset = (2,-2) },
                new RangeCell() { Colour = "R", Offset = (2,0) },
                new RangeCell() { Colour = "R", Offset = (2,2) },
            };

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(expectedRedXIIIRanges[i].Colour, redXIII.Range[i].Colour);
                Assert.Equal(expectedRedXIIIRanges[i].Offset.row, redXIII.Range[i].Offset.row);
                Assert.Equal(expectedRedXIIIRanges[i].Offset.col, redXIII.Range[i].Offset.col);
            }
        }

        [Fact]
        public void CheckOrangeTilesUpdatesCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            game.currentPlayer = game.Players[0]; // Force player 1 to be current player

            // Place security officer in the 2nd row, 1st column
            var securityOfficer = _cards[0];
            game.currentPlayer.Hand[0] = securityOfficer;
            game.PlaceCard(0, 1, 0);

            // Ensure affected tiles are updated correctly
            Assert.Equal(2, game.Player1Grid[0, 0].Rank);
            Assert.Equal("Player1", game.Player1Grid[0,0].Owner!.Name);

            Assert.Equal(1, game.Player1Grid[1, 1].Rank);
            Assert.Equal("Player1", game.Player1Grid[1, 1].Owner!.Name);

            Assert.Equal(2, game.Player1Grid[2, 0].Rank);
            Assert.Equal("Player1", game.Player1Grid[2, 0].Owner!.Name);

            // Ensure changes are reflected in player 2's board too
            Assert.Equal(2, game.Player2Grid[0, 4].Rank);
            Assert.Equal("Player1", game.Player2Grid[0, 4].Owner!.Name);

            Assert.Equal(1, game.Player2Grid[1, 3].Rank);
            Assert.Equal("Player1", game.Player2Grid[1, 3].Owner!.Name);

            Assert.Equal(2, game.Player2Grid[2, 4].Rank);
            Assert.Equal("Player1", game.Player2Grid[2, 4].Owner!.Name);
        }
    }
}