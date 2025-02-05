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

        public TestBoard()
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
        public void CardsHaveChildrenSet()
        {
            // Cait Sith has Moogle as child card
            Assert.Contains<Card>(_cards[146], _cards[91].Children);

            // Moogle Trio has Moogle Mage and Moogle Bard as children cards
            Assert.Contains<Card>(_cards[147], _cards[109].Children);
            Assert.Contains<Card>(_cards[148], _cards[109].Children);

            // Shiva has 3 Diamond Dusts as children cards
            Assert.Contains<Card>(_cards[163], _cards[95].Children);
            Assert.Contains<Card>(_cards[164], _cards[95].Children);
            Assert.Contains<Card>(_cards[165], _cards[95].Children);
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

            // Place Twin Brain to raise position rank of tile 
            var twinBrain = _cards[70];
            SetFirstCardInHand(game, twinBrain);
            game.PlaceCard(0, 0, 0);

            AssertTileState(game.Player1Grid, 2, 0, 3, "Player1");
        }

        [Fact]
        public void PlaceCardWithRaisePositionRankAbilityButAlreadyCardOnTile()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place security officer at [2,0]
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 2, 0);

            // Back to Player 1's turn
            game.currentPlayer = game.Players[0];

            // Place Twin Brain but it shouldn't change the position rank of the affected tiles since there's already a card there
            var twinBrain = _cards[70];
            SetFirstCardInHand(game, twinBrain);
            game.PlaceCard(0, 0, 0);

            AssertTileState(game.Player1Grid, 2, 0, 1, "Player1");
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
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

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

            Assert.Equal(1, game.Player1Grid[1,0].SelfBonusPower);
            Assert.Equal(1, game.Player2Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhanceWhileInPlay()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Play Crystalline Crab
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 1, 0);

            // Tile above should have its power enhanced by 2
            Assert.Equal(2, game.Player1Grid[0,0].TileBonusPower);
            SetPlayer1Start(game);

            // When an allied security officer is placed, its cumulative power should be 3 (1+2)
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());
            SetPlayer1Start(game);

            // When Crystalline Crab is destroyed, the security officer should lose its enhancement
            var insectoidChimera = _cards[51];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0);

            Assert.Equal(1, game.Player1Grid[0, 0].GetCumulativePower());
        }

        // Used to place cards in "illegal" places and ignore abilities for more efficient testing
        private void ForcePlace(Game game, Card card, Player owner, int row, int col)
        {
            game.Player1Grid[row, col].Owner = owner;
            game.Player1Grid[row, col].Card = card;
        }

        [Fact]
        public void PlaceCardThatEnhanceWhenFirstPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[0], 0, 0); // Allied card
            ForcePlace(game, securityOfficer, game.Players[1], 0, 1); // Enemy card

            // Loveless is the only card with both this ability & trigger condition
            var loveless = _cards[138];
            SetFirstCardInHand(game, loveless);
            game.PlaceCard(0, 1, 0);

            Assert.Equal(2, game.Player1Grid[0, 0].GetCumulativePower()); // Allied card enhanced
            Assert.Equal(2, game.Player1Grid[0, 1].GetCumulativePower()); // Enemy card enhanced

            ForcePlace(game, securityOfficer, game.Players[1], 1, 1); // Place enemy card beneath the first enemy card
            // Card shouldn't have bonus power since it wasn't present at the time of Loveless being placed
            Assert.Equal(1, game.Player1Grid[1, 1].GetCumulativePower()); 
        }

        [Fact]
        public void PlaceCardThatEnfeeblesWhileInPlay()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var elphadunk = _cards[10];
            ForcePlace(game, elphadunk, game.Players[0], 0, 0);
            Assert.Equal(4, game.Player1Grid[0, 0].GetCumulativePower());

            var resurrectedAmalgram = _cards[155];
            SetFirstCardInHand(game, resurrectedAmalgram);
            game.PlaceCard(0, 1, 0);

            // Elphadunk's cumulative power should be reduced to 2
            Assert.Equal(2, game.Player1Grid[0, 0].GetCumulativePower());

            SetPlayer1Start(game);

            var insectoidChimera = _cards[50];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0); // Replace the Resurrect Amalgram card

            // Elphadunk's power should be back to 4
            Assert.Equal(4, game.Player1Grid[0, 0].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnfeeblesWhenFirstPlayed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var elphadunk = _cards[10];
            ForcePlace(game, elphadunk, game.Players[0], 0, 0);

            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 1, 0);

            // The Capparwire's ability should have affected the Elphadunk
            Assert.Equal(3, game.Player1Grid[0, 0].GetCumulativePower());

            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[0], 2, 0);

            /* Security Officer shouldn't be affected since it wasn't present at the
             * time of the Capparwire's ability being executed.
             */
            Assert.Equal(1, game.Player1Grid[2, 0].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnhancedAsFirstCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place Ifrit at the top of the first column
            var ifrit = _cards[94];
            SetFirstCardInHand(game, ifrit);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);

            // Place security officer in middle of first column
            ForcePlace(game, _cards[0], game.Players[0], 1, 0);

            // Place crystalline crab at bottom of first column, enhancing security officer
            SetPlayer1Start(game);
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);


            // Security Officer's power should be enhanced by 2
            Assert.Equal(3, game.Player1Grid[1, 0].GetCumulativePower());
            // Ifrit should enhance its own power because an allied card has been enhanced
            Assert.Equal(2, game.Player1Grid[0,0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnhancedAfterCardsHaveAlreadyBeenPlaced()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);

            SetPlayer1Start(game);
            var securityOfficer = _cards[0];
            SetFirstCardInHand(game, securityOfficer);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(3, game.Player1Grid[1, 0].GetCumulativePower());

            SetPlayer1Start(game);
            var ifrit = _cards[94];
            SetFirstCardInHand(game, ifrit);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);
            Assert.Equal(2, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnfeebledAsFirstCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var shadowBloodQueen = _cards[144];
            SetFirstCardInHand(game, shadowBloodQueen);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            SetPlayer1Start(game);
            var grasslandsWolf = _cards[7];
            SetFirstCardInHand(game, grasslandsWolf);
            game.PlaceCard(0, 1, 0);

            // Place Capparwire to enfeeble Grasslands Wolf
            SetPlayer1Start(game);
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 2, 0);
            Assert.Equal(-1, game.Player1Grid[1, 0].CardBonusPower);

            // Shadowblood Queen should have enhanced its own power by 3 due to the enfeeble
            Assert.Equal(3, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardThatEnhancesItselfWhenAlliesAreEnfeebledAfterCardsHaveAlreadyBeenPlaced()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var grasslandsWolf = _cards[7];
            SetFirstCardInHand(game, grasslandsWolf);
            game.PlaceCard(0, 1, 0);

            // Place Capparwire to enfeeble Grasslands Wolf
            SetPlayer1Start(game);
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 2, 0);
            Assert.Equal(-1, game.Player1Grid[1, 0].CardBonusPower);

            SetPlayer1Start(game);
            var shadowBloodQueen = _cards[144];
            SetFirstCardInHand(game, shadowBloodQueen);
            game.Player1Grid[0, 0].RankUp(2);
            game.PlaceCard(0, 0, 0);

            // Shadowblood Queen should have enhanced its own power by 3 due to the enfeeble
            Assert.Equal(3, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void CheckCardWithAEConditionIsWorkingCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            game.Player1Grid[0, 0].RankUp(2);
            var dio = _cards[140];
            SetFirstCardInHand(game, dio);
            game.PlaceCard(0, 0, 0);
            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            // Place security officers for both players
            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[1], 0, 4);
            ForcePlace(game, securityOfficer, game.Players[0], 1, 0);

            // Enhance player 2's (enemy) security officer using a Crystalline Crab
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(2, game.Player2Grid[0, 0].TileBonusPower);

            // Dio should've received its own power by 1
            Assert.Equal(1, game.Player1Grid[0, 0].SelfBonusPower);

            // Enhancing Player 1's (ally) security officer using a Crystalline Crab
            SetFirstCardInHand(game, crystallineCrab);
            game.PlaceCard(0, 2, 0);
            Assert.Equal(2, game.Player1Grid[1, 0].TileBonusPower);

            // Dio should have raise its own power by 1 again
            Assert.Equal(2, game.Player1Grid[0, 0].SelfBonusPower);
        }

        [Fact]
        public void CheckAbilityTriggerWhenItIsDestroyed()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var securityOfficer = _cards[0];
            ForcePlace(game, securityOfficer, game.Players[0], 0, 0);

            var sandhogPie = _cards[35];
            SetFirstCardInHand(game, sandhogPie);
            game.PlaceCard(0, 1, 0);

            SetPlayer1Start(game);
            // Destroy the Sandhog Pie using a replace card (Insectoid Chimera)
            var insectoidChimera = _cards[50];
            SetFirstCardInHand(game, insectoidChimera);
            game.PlaceCard(0, 1, 0);

            // Security Officer should have its power increased by 3 from the destruction of Sandhog Pie
            Assert.Equal(3, game.Player1Grid[0, 0].CardBonusPower);
        }

        [Fact]
        public void CheckWhenFirstEnhancedAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place a Security Officer in 1, 1 of Player1Grid
            ForcePlace(game, _cards[0], game.Players[0], 1, 1);

            // Place the Elena card so that it targets the Security Officer
            var elena = _cards[129];
            SetFirstCardInHand(game, elena);
            game.PlaceCard(0, 1, 0);

            // Place Crystalline Crab to enhance Elena
            SetPlayer1Start(game);
            var crystallineCrab = _cards[12];
            SetFirstCardInHand(game, crystallineCrab);

            Assert.Equal("Security Officer", game.Player1Grid[1, 1].Card!.Name);
            game.PlaceCard(0, 2, 0);

            // Security Officer card is gone
            Assert.Null(game.Player1Grid[1, 1].Card);

            /* The "CardBonusPower" of the Tile (1, 1) should be reset to 0 since the
             * Security Officer card was destroyed
             */
            Assert.Equal(0, game.Player1Grid[1, 1].CardBonusPower);
        }

        [Fact]
        public void CheckWhenFirstEnfeebledAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an enemy Archdragon in 1, 1 of Player1Grid
            ForcePlace(game, _cards[19], game.Players[1], 1, 1);

            // Place Reapertail card
            var reapertail = _cards[60];
            SetFirstCardInHand(game, reapertail);
            game.PlaceCard(0, 2, 0);

            // Enfeeble the Reapertail using a Capparwire
            SetPlayer1Start(game);
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 1, 0);

            // The Archdragon's power should be reduced by 2 (final power of 1)
            Assert.Equal(-2, game.Player1Grid[1, 1].CardBonusPower);
            Assert.Equal(1, game.Player1Grid[1, 1].GetCumulativePower());
        }

        [Fact]
        public void PlaceCardWithADTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an allied Security Officer
            ForcePlace(game, _cards[0], game.Players[0], 0, 0);

            var tonberryKing = _cards[34];
            SetFirstCardInHand(game, tonberryKing);
            game.Player1Grid[1, 0].RankUp(1);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Place a Gigantoad to replace (and destroy) the allied Security Officer
            SetPlayer1Start(game);
            var gigantoad = _cards[51];
            SetFirstCardInHand(game, gigantoad);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(2, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithEDTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place an enemy Security Officer
            ForcePlace(game, _cards[0], game.Players[1], 0, 4);

            var deathClaw = _cards[42];
            SetFirstCardInHand(game, deathClaw);
            game.PlaceCard(0, 1, 0);
            Assert.Equal(0, game.Player1Grid[1, 0].SelfBonusPower);

            // Place a Gigantoad to replace (and destroy) the enemy Security Officer
            var gigantoad = _cards[51];
            SetFirstCardInHand(game, gigantoad);
            game.PlaceCard(0, 0, 0);

            Assert.Equal(1, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithAEDTriggerCondition()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var joker = _cards[46];
            SetFirstCardInHand(game, joker);
            game.Player1Grid[1, 0].RankUp(1);
            game.PlaceCard(0, 1, 0);

            Assert.Equal(0, game.Player1Grid[0, 0].SelfBonusPower);

            // Place allied Security Officer
            ForcePlace(game, _cards[0], game.Players[0], 2, 4);

            // Place enemy Security Officer
            ForcePlace(game, _cards[0], game.Players[1], 0, 4);

            // Place Capparwire to enfeeble/destroy both the Security Officers
            var capparwire = _cards[25];
            SetFirstCardInHand(game, capparwire);
            game.PlaceCard(0, 1, 0);

            // Confirm the Security Officer cards are gone
            Assert.Null(game.Player1Grid[2, 4].Card);
            Assert.Null(game.Player1Grid[0, 4].Card);

            Assert.Equal(2, game.Player1Grid[1, 0].SelfBonusPower);
        }

        [Fact]
        public void PlaceCardWithChildrenCards()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var moogleTrio = _cards[109];
            SetFirstCardInHand(game, moogleTrio);
            game.Player1Grid[0, 0].RankUp(1);
            game.PlaceCard(0, 0, 0);

            // Confirm Player 1 has Moogle Mage & Moogle Bard added to hand
            var moogleMage = _cards[147];
            var moogleBard = _cards[148];

            Assert.Contains<Card>(moogleMage, game.Players[0].Hand);
            Assert.Contains<Card>(moogleBard, game.Players[0].Hand);
        }
    }
}