using System.IO;
using Newtonsoft.Json;

namespace KustoExplorer.Core
{
    public static class JsonHelpers
    {
        public static T DeserializeFromFile<T>(string filename)
        {
            using (var file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                var snippet = (T)serializer.Deserialize(file, typeof(T));
                return snippet;
            }
        }

        public static void SaveToFile<T>(T obj, string filename)
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, obj);
            }
        }
    }
}