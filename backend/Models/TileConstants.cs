namespace backend.Models
{
    public class TileConstants
    {
        public const int NUM_ROWS = 3;
        public const int NUM_COLS = 5;

        // Private variables
        public static readonly List<string> OnPlaceConditions = new List<string> { "AP", "EP" };
        public static readonly List<string> OnDestroyConditions = new List<string> { "D", "AD", "ED", "AED", "*", "W" };
        public static readonly List<string> OnEnhanceConditions = new List<string> { "P1R", "1+", "+A", "+E", "+AE", "EE" };
        public static readonly List<string> OnEnfeebleConditions = new List<string> { "1-", "-A", "-E", "-AE", "EE" };
        public static readonly string OnRoundEndCondition = "L+V";
    }
}
