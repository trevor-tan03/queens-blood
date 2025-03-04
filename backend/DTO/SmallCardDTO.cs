namespace backend.DTO
{
    public class SmallCardDTO
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Ability { get; set; }

        public SmallCardDTO(string name, string image, string ability)
        {
            Name = name;
            Image = image;
            Ability = ability;
        }
    }
}
