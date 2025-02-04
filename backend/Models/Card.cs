using System.Drawing;

namespace backend.Models
{
	public struct RangeCell
	{
		public string Colour;
		public (int x, int y) Offset;
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
		public List<Card> Children { get; set; } = new List<Card>();
		public Ability Ability { get; set; }
		public List<RangeCell> Range { get; set; } = new List<RangeCell>();

		// Default is 1, but some cards have abilities which increases this up to 3.
		public int RankUpAmount { get; set; } = 1;


        public void AddChild(Card child)
		{
			Children.Add(child);
		}

		public void AddRangeCell(string offsetString, string colour)
		{
            var offsetTuple = offsetString.Replace("(", "").Replace(")", "").Split(",");
			var x = Int32.Parse(offsetTuple[0].Trim());
			var y = Int32.Parse(offsetTuple[1].Trim());

            RangeCell cell = new RangeCell()
            {
                Colour = colour,
                Offset = (x, y)
            };
            Range.Add(cell);
        }
	}
}
