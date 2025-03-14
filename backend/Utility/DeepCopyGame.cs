using backend.Models;
using Newtonsoft.Json;
using System.Text.Json;

namespace backend.Utility
{
    public class Copy
    {
        public static T DeepCopy<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var copy = JsonConvert.DeserializeObject<T>(json);

            if (typeof(T) == typeof(Game) && copy != null && obj != null)
            {
                var gameCopy = (Game)(object)copy;
                gameCopy.Player2Grid = BoardUtility.MirrorBoard(gameCopy.Player1Grid);
                BoardUtility.ReInitTiles(gameCopy);
                BoardUtility.RemapEnhancedAndEnfeebledCards(gameCopy);
                gameCopy.CurrentPlayer = gameCopy.Players[gameCopy._currentPlayerIndex];

                return Cast<T>(gameCopy);
            }

            return copy;
        }

        public static T Cast<T> (object obj)
        {
            return (T)obj;
        } 
    }
}
