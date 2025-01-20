using System.Drawing;

namespace backend.Models
{
	public struct RangeCell
	{
		public string Colour;
		public (int row, int col) Offset;
	}

	public struct Ability
	{
		public string Description;
		public string Condition;
		public string? Action;
		public string? Target;
		public int? Value;
	}

	public class Card 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Rank { get; set; }
		public int Power { get; set; }
		public string Rarity { get; set; }
		public string Image { get; set; }
		public Card? Child { get; set; }
		public Ability Ability { get; set; }
		public List<RangeCell> Range { get; set; } = new List<RangeCell>();

		//Events
		public Action<Card>? OnCardPlaced;
        public Action<Card>? OnCardDestroyed;

		//Game
		//Draw the card
		

        public void SetChild(Card child)
		{
			Child = child; 
		}

		public void AddRangeCell(string offsetString, string colour)
		{
            var offsetTuple = offsetString.Replace("(", "").Replace(")", "").Split(",");

            RangeCell cell = new RangeCell()
            {
                Colour = colour,
                Offset = (Int32.Parse(offsetTuple[0].Trim()), Int32.Parse(offsetTuple[1].Trim()))
            };
            Range.Add(cell);
        }

  //      public void InitAbility(Game game)
  //      {
  //          game.OnCardDestroyed += AddToCardLevel;
  //      }

		//private void AddToCardLevel(Card destroyedCard)
		//{
		//	//
		//	Power += 5;
		//}
    }
}
