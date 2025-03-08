namespace backend.DTO
{
    public class GameDTO
    {
        // First 3 lanes is for the player the GameDTO is directed to
        public int[] LaneScores { get; set; } = new int[6];
        public int[] LaneBonuses { get; set; } = new int[6];
        public TileDTO[] Board { get; set; } = new TileDTO[15];

        public GameDTO(int[] laneScores, int[] laneBonuses, TileDTO[] board)
        {
            LaneScores = laneScores;
            LaneBonuses = laneBonuses;
            Board = board;
        }
    }
}
