namespace backend.Models
{
    public class TileConstants
    {
        public const int NUM_ROWS = 3;
        public const int NUM_COLS = 5;

        // Private variables
        public static readonly List<string> OnPlaceConditions = ["AP", "EP"];
        public static readonly List<string> OnDestroyConditions = ["AD", "ED", "AED"];
        public static readonly List<string> OnEnhanceConditions = ["P1R", "1+", "EE"];
        public static readonly List<string> OnEnfeebleConditions = ["1-", "EE"];
        public static readonly string OnRoundEndCondition = "L+V";
        public static readonly List<string> OnEnhancedCardsChangedConditions = ["+A", "+E", "+AE"];
        public static readonly List<string> OnEnfeebledCardsChangedConditions = ["-A", "-E", "-AE"];
    }
}
