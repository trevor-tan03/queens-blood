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

		// Default is 1, but some cards have abilities which increases this up to 3.
		public int RankUpAmount { get; set; } = 1;
		

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

		public void InitAbility(Game game)
		{
			var onPlaceConditions = new List<string> { "AP", "EP" };
			var onDestroyConditions = new List<string> { "D", "AD", "ED", "*" };
			var onEnhanceConditions = new List<string> { "P1R", "1+", "+A", "+E", "+AE" };
			var onEnfeebleConditions = new List<string> { "1-", "-A", "-E", "-AE" };
			var onWinLane = new List<string> { "+Score", "L+V" };

			// Subscribe based on the card's trigger condition for its ability
			if (onPlaceConditions.Contains(this.Ability.Condition))
			{
                game.OnCardPlaced += HandleCardPlaced;
            } 
			else if (onDestroyConditions.Contains(this.Ability.Condition))
			{
				game.OnCardDestroyed += HandleCardDestroyed;
			} 
			else if (onEnhanceConditions.Contains(this.Ability.Condition))
			{
				game.OnCardEnhanced += HandleCardEnhanced;
			} 
			else if (onEnfeebleConditions.Contains(this.Ability.Condition))
			{ 
				game.OnCardEnfeebled += HandleCardEnfeebled;
			}
		}

		private void UninitAbility(Game game)
		{
            // Unsubscribe the destroyed card from all events
            game.OnCardPlaced += HandleCardPlaced;
            game.OnCardDestroyed += HandleCardDestroyed;
            game.OnCardEnhanced += HandleCardEnhanced;
            game.OnCardEnfeebled += HandleCardEnfeebled;

            /*
			 * Undo the effects of the ability on other cards if applicable. This includes abilities with:
			 *  - "While in play" condition (*)
			 */
        }

        private void HandleCardPlaced(Card card)
		{
			//if (this.Ability.Condition == "R")
			//{
				
			//}
		}

		private void HandleCardDestroyed(Card card)
		{
			
			
		}

		private void HandleCardEnhanced(Card card)
		{

		}

		private void HandleCardEnfeebled(Card card)
		{

		}
	}
}
