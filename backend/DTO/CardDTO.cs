using backend.Models;

namespace backend.DTO
{
    public class CardDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Power { get; set; }
        public string Rarity { get; set; }
        public string Image { get; set; }
        public string Ability { get; set; }
        public string? Action { get; set; }

        public CardDTO(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Rank = card.Rank;
            Power = card.Power;
            Rarity = card.Rarity;
            Image = card.Image;
            Ability = card.Ability.Description;
            Action = card.Ability.Action;
        }
    }
}
