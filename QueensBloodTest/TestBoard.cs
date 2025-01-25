using System.Drawing;
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

                            if (offsetString != null && colour != null)
                            {
                                card.AddRangeCell(offsetString, colour);
                            }

                            _cards.Add(card);
                        }
                    }
                }

                CreateGameWithPlayers();
            }
        }

        private Game CreateGameWithPlayers()
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

        private void SetFirstCardInHand(Game game, Card card)
        {
            game.currentPlayer!.Hand[0] = card;
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

        private void SetPlayer1Start(Game game)
        {
            game.currentPlayer = game.Players.First();
        }

        [Fact]
        public void CheckSwapPlayersEachTurn()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);

            Assert.Equal("Player1", game.currentPlayer!.Name);
            game.PlaceCard(0, 0, 0);
            Assert.Equal("Player2", game.currentPlayer!.Name);
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
                Assert.Equal(expectedRedXIIIRanges[i].Offset.y, redXIII.Range[i].Offset.y);
                Assert.Equal(expectedRedXIIIRanges[i].Offset.x, redXIII.Range[i].Offset.x);
            }
        }

        private void AssertTileState(Tile[,] grid, int row, int column, int expectedRank, string expectedOwner)
        {
            Assert.Equal(expectedRank, grid[row, column].Rank);
            Assert.Equal(expectedOwner, grid[row, column].Owner!.Name);
        }

        [Fact]
        public void CheckOrangeTilesUpdatesCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place security officer in the 2nd row, 1st column
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 1, 0);

            // Ensure affected tiles are updated correctly
            AssertTileState(game.Player1Grid, 0, 0, 2, "Player1");
            AssertTileState(game.Player1Grid, 1, 1, 1, "Player1");
            AssertTileState(game.Player1Grid, 2, 0, 2, "Player1");

            AssertTileState(game.Player2Grid, 0, 4, 2, "Player1");
            AssertTileState(game.Player2Grid, 1, 3, 1, "Player1");
            AssertTileState(game.Player2Grid, 2, 4, 2, "Player1");
        }

        [Fact]
        public void PlaceReplaceCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 1, 0);

            game.currentPlayer = game.Players[0]; // Back to Player 1's turn

            var insectoidChimera = _cards[50];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0);

            // Ensure affected tiles are updated correctly
            AssertTileState(game.Player1Grid, 0, 0, 3, "Player1");
            AssertTileState(game.Player1Grid, 1, 1, 2, "Player1");
            AssertTileState(game.Player1Grid, 2, 0, 3, "Player1");
        }

        [Fact]
        public void PlaceCardWithRaisePositionRankAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var twinBrain = _cards[70];
            SetFirstCardInHand(game, twinBrain);
            game.PlaceCard(0, 0, 0);

            AssertTileState(game.Player1Grid, 2, 0, 3, "Player1");
        }

        [Fact]
        public void PlaceCardsThatEnhanceWhenCardsArePlayed()
        {
            // This refers to the two cards: Sea Devil and Bagnadrana
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Set middle row of each each player's side have a starting rank of 3
            game.Player1Grid[1, 0].RankUp(2);
            game.Player2Grid[1, 0].RankUp(2);

            // Player 1 places sea devil which raises its power by 1 when an allied card is played (AP)
            var seaDevil = _cards[31];
            SetFirstCardInHand(game, seaDevil);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].BonusPower);

            // Player 2 places sea devil which raises its power by 1 when an enemy card is played (EP)
            var bagnadrana = _cards[37];
            SetFirstCardInHand(game, bagnadrana);
            game.PlaceCard(0, 1, 0);

            /* Player 1 places an allied card (security officer) in the first column. This is an enemy card for Player 2.
             * This should raise the power of both the Sea Devil and Bagnadrana.
             */
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(1, game.Player1Grid[1,0].BonusPower);
            Assert.Equal(1, game.Player2Grid[1, 0].BonusPower);
        }
    }
}