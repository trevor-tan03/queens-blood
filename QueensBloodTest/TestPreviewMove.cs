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

            var gameCopy = Copy.DeepCopy(game);
            Assert.NotSame(game, gameCopy);
        }

        [Fact]
        public void TestDeepCopyAbilityStructCorrect()
        {
            var abilityCopy = Copy.DeepCopy(_cards[0].Ability);
            Assert.Equal(_cards[0].Ability, abilityCopy);
        }

        [Fact]
        public void TestDeepCopyTupleCorrect()
        {
            var tuple = _cards[0].Range[0].Offset;
            var tupleCopy = Copy.DeepCopy(tuple);
            Assert.Equal(tuple, tupleCopy);
        }
    }
}
