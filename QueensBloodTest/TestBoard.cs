using System.Drawing;
using backend;
using backend.Models;
using backend.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace QueensBloodTest
{
    public class TestBoard : TestBase
    {
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
        public void CheckCurrentPlayerSelected()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            Assert.NotNull(game.CurrentPlayer);

        }

        [Fact]
        public void CheckSwapPlayersTurn()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            Assert.Equal("Player1", game.CurrentPlayer!.Name);
            game.SwapPlayerTurns();

            Assert.Equal("Player2", game.CurrentPlayer!.Name);
        }

        [Fact]
        public void InitializeCardRangeCorrectly()
        {
            var redXIII = _cards[(int)Cards.RedXIII];
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

        [Fact]
        public void CheckOrangeTilesUpdatesCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Place security officer in the 2nd row, 1st column
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0);

            // Ensure affected tiles are updated correctly
            AssertTileState(game.Player1Grid, 0, 0, 2, "Player1");
            AssertTileState(game.Player1Grid, 1, 1, 1, "Player1");
            AssertTileState(game.Player1Grid, 2, 0, 2, "Player1");

            AssertTileState(game.Player2Grid, 0, 4, 2, "Player1");
            AssertTileState(game.Player2Grid, 1, 3, 1, "Player1");
            AssertTileState(game.Player2Grid, 2, 4, 2, "Player1");
        }

        [Fact]
        public void VerifyScoreAfterPlay()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 1, 0);

            Assert.Equal(3, game.Players[0].Scores[0].score);
            Assert.Equal(1, game.Players[0].Scores[1].score);
        }

        [Fact]
        public void GetLaneWinnerForTie()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            game.SwapPlayerTurns();
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);

            Assert.Null(game.GetLaneWinner(0));
        }

        [Fact]
        public void GetLaneWinnerForWin()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Player 1 is winning
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            var player1 = game.Players[0];
            Assert.Equal(player1, game.GetLaneWinner(0));
        }

        [Fact]
        public void TwoConsecutivePassesEndsGame()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            Assert.False(game.GameOver);

            // Non-consecutive pass
            game.Pass(); // P1 - pass
            game.PlaceCard(0, 0, 0); // P2 - play
            game.SwapPlayerTurns();
            Assert.False(game.GameOver);

            // Consecutive passes
            game.Pass();
            game.Pass();
            Assert.True(game.GameOver);
        }

        [Fact]
        public void TestPickUp()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            Assert.Equal(5, game.CurrentPlayer!.Hand.Count);
            Assert.Equal(10, game.CurrentPlayer!.Deck.Count);

            game.CurrentPlayer.PickUp(1);
            Assert.Equal(6, game.CurrentPlayer!.Hand.Count);
            Assert.Equal(9, game.CurrentPlayer!.Deck.Count);
        }

        [Fact]
        public void GiveTileOwnershipToDestroyerOfCard()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            ForcePlace(game, Cards.SecurityOfficer, game.Players[1], 0, 0);
            Assert.Equal(game.Players[1], game.Player1Grid[0, 0].Owner);

            AddToHandAndPlaceCard(game, Cards.Capparwire, 1, 0);
            Assert.Null(game.Player1Grid[0, 0].Card);
            Assert.Equal(game.Players[0], game.Player1Grid[0, 0].Owner);
        }
    }
}