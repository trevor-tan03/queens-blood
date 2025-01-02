namespace backend.Models
{
	public class Card
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Rank { get; set; }
		public int Power { get; set; }
		public string Rarity { get; set; }
		public string Ability { get; set; }
		public string Image { get; set; }
		public Card? Child { get; set; }
		public string Condition { get; set; }
		public string? Action { get; set; }
		public string? Target { get; set; }
		public int? Value { get; set; }
		public void SetChild(Card child)
		{
			Child = child; 
		}

	}
}
