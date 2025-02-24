using backend.Models;
using backend.DTO;
using static backend.Models.TileConstants;

namespace backend.DTO
{
    public class DTOConverter
    {
        public static CardDTO GetCardDTO(Card card)
        {
            return new CardDTO(card);
        }

        public static List<CardDTO> GetCardDTOList(List<Card> cards)
        {
            List<CardDTO> cardsDTO = new List<CardDTO>();
            foreach (Card card in cards)
                cardsDTO.Add(new CardDTO(card));
            return cardsDTO;
        }

        public static SmallCardDTO GetSmallCardDTO(Card card)
        {
            return new SmallCardDTO(card.Name, card.Image, card.Ability.Description);
        }

        public static TileDTO GetTileDTO(Tile tile)
        {
            var ownerId = tile.Owner?.Id;
            var bonusPower = tile.TileBonusPower + tile.SelfBonusPower + tile.CardBonusPower;
            var card = tile.Card != null ? GetSmallCardDTO(tile.Card) : null;
            return new TileDTO(ownerId, bonusPower, card, tile.Rank);
        }

        public static GameDTO GetGameDTO(Game game, int playerIndex)
        {
            var player = game.Players[playerIndex];
            var grid = playerIndex == 0 ? game.Player1Grid : game.Player2Grid;

            var laneScores = new int[6];
            var board = new TileDTO[15];

            var count = 0;
            for (int i = 0; i < 2; i++)
            {
                var index = i == 0 ? playerIndex : (playerIndex + 1) % 2; // Your score comes first (0-2)
                for (int j = 0; j < NUM_ROWS; j++)
                {
                    var laneScore = game.GetPlayerLaneScore(index, j); // (playerIndex, row)
                    laneScores[count++] = laneScore;
                }
            }

            count = 0;
            for (int i = 0; i < NUM_ROWS; i++)
                for (int j = 0; j < NUM_COLS; j++)
                    board[count++] = GetTileDTO(grid[i, j]);

            return new GameDTO(laneScores, board);
        }
    }
}
