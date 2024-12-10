namespace backend.Models
{
	public class Player(string Id, string Name)
	{
		public string Id { get; set; } = Id;
		public string Name { get; set; } = Name;
	}
}
