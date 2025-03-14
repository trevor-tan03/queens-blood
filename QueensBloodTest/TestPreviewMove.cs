using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Models;
using backend.Utility;
using static backend.Models.TileConstants;

namespace QueensBloodTest
{
    public class TestPreviewMove : TestBase
    {
        [Fact]
        public void TestDeepCopyCorrect()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // Ensure modifying the copy won't alter the original
            var gameCopy = Copy.DeepCopy(game);
            Assert.NotSame(game, gameCopy);

            if (gameCopy == null) return;

            // Card should only be placed in the copy
            AddToHandAndPlaceCard(gameCopy, Cards.SecurityOfficer, 0, 0);
            Assert.Null(game.Player1Grid[0, 0].Card);
            Assert.NotNull(gameCopy.Player1Grid[0, 0].Card);
        }

        [Fact]
        public void TestDeepCopyAbilityStructCorrect()
        {
            var abilityCopy = Copy.DeepCopy(_cards[0].Ability);
            Assert.Equal(_cards[0].Ability, abilityCopy);
        }

        [Fact]
        public void TestDeepCopyOffsetTupleCorrect()
        {
            var tuple = _cards[0].Range[0].Offset;
            var tupleCopy = Copy.DeepCopy(tuple);
            Assert.Equal(tuple, tupleCopy);
        }

        [Fact]
        public void PreviewShowsCardsSpawned()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var gameCopy = Copy.DeepCopy(game);
            Assert.NotSame(game, gameCopy);

            gameCopy.Player1Grid[0, 0].RankUp(2);
            AddToHandAndPlaceCard(gameCopy, Cards.Shiva, 0, 0);

            Assert.NotNull(gameCopy.Player1Grid[1, 0].Card);
            Assert.NotNull(gameCopy.Player1Grid[2, 0].Card);
        }

        [Fact]
        public void PreviewShowSelfEnhancePower()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 1, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0); // To be destroyed

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.ChocoboMoogle, 2, 0);
            Assert.Equal(1, gameCopy.Player1Grid[2, 0].SelfBonusPower);
        }

        [Fact]
        public void CloudPreviewShowEnhanceWhenThresholdMet()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // First row all security officers (3*1=3)
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 1);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 2);
            Assert.Equal(3, game.GetPlayerLaneScore(0, 0));

            // Aerith (1) + Security Officer (1) = 2
            AddToHandAndPlaceCard(game, Cards.Aerith, 1, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 2);
            Assert.Equal(2, game.GetPlayerLaneScore(0, 1));

            // 2 Security Officers (2*1=2) + 1 Shoalapod (1) = 3
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 2, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 2, 2);
            AddToHandAndPlaceCard(game, Cards.Shoalapod, 2, 1);
            Assert.Equal(3, game.GetPlayerLaneScore(0, 2));

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.Cloud, 1, 1);
            // +2 to allied cards surrounding Cloud
            var surroundingTiles = new List<Tile>();
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1) continue;
                    surroundingTiles.Add(gameCopy.Player1Grid[i, j]);
                }
            }

            foreach (var tile in surroundingTiles)
                Assert.Equal(2, tile.CardBonusPower);
        }

        [Fact]
        public void PreviewEnhancingCardTriggerSandhogPieAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.SandhogPie, 1, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 1);

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.InsectoidChimera, 1, 0);
            Assert.Equal(3, gameCopy.Player1Grid[0, 0].CardBonusPower);
            Assert.Equal(3, gameCopy.Player1Grid[1, 1].CardBonusPower);
        }

        [Fact]
        public void PreviewEnhancingCardTriggerCloudsAbility()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            // First row all security officers (3*1=3)
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 1);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 0, 2);
            Assert.Equal(3, game.GetPlayerLaneScore(0, 0));

            // Aerith (1) + Cloud (3+3) + Security Officer (1) = 8
            AddToHandAndPlaceCard(game, Cards.Aerith, 1, 0);
            AddToHandAndPlaceCard(game, Cards.Cloud, 1, 1);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 2);
            Assert.Equal(8, game.GetPlayerLaneScore(0, 1));

            // 2 Security Officers (2*1=2)
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 2, 0);
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 2, 2);
            Assert.Equal(2, game.GetPlayerLaneScore(0, 2));

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.Shoalapod, 2, 1);
            // +2 to allied cards surrounding Cloud
            var surroundingTiles = new List<Tile>();
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1) continue;
                    surroundingTiles.Add(gameCopy.Player1Grid[i, j]);
                }
            }

            foreach (var tile in surroundingTiles)
                Assert.Equal(2, tile.CardBonusPower);
        }

        [Fact]
        public void PreviewWinLaneBonus()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.Tifa, 0, 0);
            Assert.Equal(5, gameCopy.GetPlayerLaneBonus(0, 0));
        }

        [Fact]
        public void PreviewSelfEnhance()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0); // Enhances Security Officer
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0); // Enhances Space Ranger (P2)
            AddToHandAndPlaceCard(game, Cards.SpaceRanger, 2, 1);

            game.SwapPlayerTurns();

            AddToHandAndPlaceCard(game, Cards.CrystallineCrab, 2, 0); // Enhances Security Officer
            AddToHandAndPlaceCard(game, Cards.SecurityOfficer, 1, 0); // Enhances Space Ranger (P1)

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.SpaceRanger, 2, 1);
            Assert.Equal(2, gameCopy.Player2Grid[2, 1].SelfBonusPower);
        }

        [Fact]
        public void AfterPlacingResurrectedAmalgamDontBreak()
        {
            var game = CreateGameWithPlayers();
            game.Start();
            SetPlayer1Start(game);

            AddToHandAndPlaceCard(game, Cards.ResurrectedAmalgam, 1, 0);

            game.SwapPlayerTurns();

            var gameCopy = Copy.DeepCopy(game);
            AddToHandAndPlaceCard(gameCopy, Cards.Capparwire, 0, 0);
            Assert.NotNull(gameCopy.Player2Grid[0, 0].Card);
        }
    }
}
