namespace backend.Models
{
    public class Tile
    {
        public Player? Owner { get; set; }
        public int Rank { get; set; }
        public Card? Card { get; set; }

        public void RankUp(int amount)
        {
            Rank = Rank + amount > 3 ? 3 : Rank + amount;
        }

        public void PlaceCard(Card card)
        {
            Card ??= card;
        }

        public bool CanPlaceCard(Card card, Player player)
        {
            return Rank <= card.Rank && Owner != null && Owner.Equals(player);
        }
    }
}
