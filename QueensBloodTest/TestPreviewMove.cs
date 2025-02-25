using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Utility;

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
    }
}
