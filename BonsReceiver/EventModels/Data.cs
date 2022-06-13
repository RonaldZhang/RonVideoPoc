using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{
    public class Data
    {
        [JsonConstructor]
        public Data(string id, string type, string action,string solution, Fields fields )
        {
            this.Id = id;
            this.Type = type;
            this.Action = action;
            this.Solution = solution;
            this.Fields = fields;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("action")]
        public string Action { get; }

        [JsonPropertyName("solution")]
        public string Solution { get; }

        [JsonPropertyName("fields")]
        public Fields Fields { get; }
    }


}
