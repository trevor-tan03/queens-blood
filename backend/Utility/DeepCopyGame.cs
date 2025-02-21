using backend.Models;
using Newtonsoft.Json;
using System.Text.Json;

namespace backend.Utility
{
    public class Copy
    {
        public static T? DeepCopy<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T?>(json);
        }
    }
}
