using System.Drawing;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace backend.Models
{
    public struct RangeCell
	{
		public string Colour { get; set; }
        public (int x, int y) Offset { get; set; }

		[JsonConstructor]
		public RangeCell(string colour, int x, int y)
		{
			(Colour, Offset) = (colour, (x, y));
		}
	}

	public struct Ability
	{
		public string Description { get; set; }
		public string Condition { get; set; }
		public string? Action { get; set; }
		public string? Target { get; set; }
		public int? Value { get; set; }

		[JsonConstructor]
		public Ability(string description, string condition, string? action, string? target, int? value)
		{
			(Description, Condition, Action, Target, Value) = (description, condition, action, target, value);
		}
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

			RangeCell cell = new RangeCell(colour, x, y);
            Range.Add(cell);
        }
	}
}
