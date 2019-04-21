using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Acklann.Daterpillar
{
    public class Config
    {
        static Config()
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connections.json")));
            MockarooKey = config.GetValue("mockarooKey", StringComparison.OrdinalIgnoreCase).Value<string>();
        }

        public static readonly string MockarooKey;

        public static string GetConnectionString(Language kind)
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connections.json")));
            return config.SelectToken((kind).ToString().ToLowerInvariant())?.Value<string>();
        }
    }
}