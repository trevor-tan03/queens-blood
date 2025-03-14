using static backend.Models.TileConstants;

using backend.Models;

namespace backend.Utility
{
    public class BoardUtility
    {
        public static Tile[,] MirrorBoard (Tile[,] board)
        {
            Tile[,] mirroredBoard = new Tile[3,5];

            // Mirror player 1's board for player 2
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < NUM_COLS; j++)
                {
                    mirroredBoard[i, j] = board[i, 4 - j];
                }
            }

            return mirroredBoard;
        }

        public static void ReInitTiles(Game game)
        {
            for (int i = 0; i < NUM_ROWS; i++)
            {
                for (int j = 0; j < NUM_COLS; j++)
                {
                    if (game.Player1Grid[i, j].Card != null)
                        game.Player1Grid[i, j].ReInitAbilities(game);
                }
            }
        }

        public static void RemapEnhancedAndEnfeebledCards(Game game)
        {
            for (int i = 0; i < game.EnhancedCards.Count; i++)
            {
                var row = (int)char.GetNumericValue(game.EnhancedCards[i].Id[0]);
                var col = (int)char.GetNumericValue(game.EnhancedCards[i].Id[1]);

                game.EnhancedCards[i] = game.Player1Grid[row, col];
            }

            for (int i = 0; i < game.EnfeebledCards.Count; i++)
            {
                var row = (int)char.GetNumericValue(game.EnfeebledCards[i].Id[0]);
                var col = (int)char.GetNumericValue(game.EnfeebledCards[i].Id[1]);

                game.EnfeebledCards[i] = game.Player1Grid[row, col];
            }
        }
    }
}
