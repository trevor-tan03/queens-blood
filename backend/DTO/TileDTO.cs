namespace backend.DTO
{
    public class TileDTO
    {
        public string? OwnerId { get; set; }
        public int BonusPower { get; set; }
        public SmallCardDTO? Card { get; set; }

        public TileDTO(string? ownerId, int bonusPower, SmallCardDTO? cardDTO)
        {
            OwnerId = ownerId;
            BonusPower = bonusPower;
            Card = cardDTO;
        }
    }
}
