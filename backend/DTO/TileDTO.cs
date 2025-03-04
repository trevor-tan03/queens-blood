﻿namespace backend.DTO
{
    public class TileDTO
    {
        public string? OwnerId { get; set; }
        public int BonusPower { get; set; }
        public SmallCardDTO? Card { get; set; }
        public int Rank { get; set; }

        public TileDTO(string? ownerId, int bonusPower, SmallCardDTO? cardDTO, int rank)
        {
            OwnerId = ownerId;
            BonusPower = bonusPower;
            Card = cardDTO;
            Rank = rank;
        }
    }
}
