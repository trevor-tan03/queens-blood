using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Models;

namespace QueensBloodTest
{
    public class TestGameInitialization : TestBase
    {
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
        public void BoardInitializedCorrectly()
        {
            var game = CreateGameWithPlayers();
            game.Start();

            var p1 = game.Players[0];
            var p2 = game.Players[1];

            for (int row = 0; row < 3; row++)
            {
                Assert.Equal(p1, game.Player1Grid[row, 0].Owner);
                Assert.Equal(p2, game.Player1Grid[row, 4].Owner);

                Assert.Equal(p2, game.Player2Grid[row, 0].Owner);
                Assert.Equal(p1, game.Player2Grid[row, 4].Owner);
            }
        }
    }
}
