namespace backend.Models
{
	public class Player
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Boolean IsReady { get; set; } = false;
		public Boolean IsHost { get; set; } = false;
		public List<Card> Deck { get; set; } = new List<Card>();
		public List<Card> Hand { get; set; } = new List<Card>();

		public Player(string Id, string Name)
		{
			this.Id = Id;
			this.Name = Name;
		}

		public void Unready ()
		{
			IsReady = false;
			Deck = [];
		}

		public void Ready (List<Card> _deck)
		{
			IsReady = true;
			Deck = _deck;
		}

		public void PickUp (int numCards)
		{
			if (Deck.Count >= numCards)
			{
				var pickedUpCards = Deck.GetRange(0,numCards);
				Hand.AddRange(pickedUpCards);
				Deck.RemoveRange(0,numCards);
			}
		}
	}
}
