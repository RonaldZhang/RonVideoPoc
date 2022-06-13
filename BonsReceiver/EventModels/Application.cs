using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Application
    {
        [JsonConstructor]
        public Application( string id,string href)
        {
            this.Id = id;
            this.Href = href;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("href")]
        public string Href { get; }
    }


}
