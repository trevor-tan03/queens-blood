namespace backend.Models
{
	public struct LaneScore
	{
		public int score;
		public int winBonus;
		// Player who loses the lane gives their points in the lane to the victor
		public int loserBonus;
	}

	public class Player
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public Boolean IsReady { get; set; } = false;
		public Boolean IsHost { get; set; } = false;
		public Boolean HasMulliganed { get; set; } = false;
		public List<Card> Deck { get; set; } = new List<Card>();
		public List<Card> Hand { get; set; } = new List<Card>();
		public int playerIndex { get; set; } = -1;

		public LaneScore[] Scores =
		{
			new LaneScore() { score = 0, winBonus = 0, loserBonus = 0 },
			new LaneScore() { score = 0, winBonus = 0, loserBonus = 0 },
			new LaneScore() { score = 0, winBonus = 0, loserBonus = 0 }
		};

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
