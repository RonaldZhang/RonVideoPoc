using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{
    public class Event
    {
        [JsonConstructor]
        public Event(string id,DateTime firstEmittedAt,DateTime lastEmittedAt, int attempt,string status, Data data,  Links links)
        {
            this.Id = id;
            this.FirstEmittedAt = firstEmittedAt;
            this.LastEmittedAt = lastEmittedAt;
            this.Attempt = attempt;
            this.Status = status;
            this.Data = data;
            this.Links = links;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("firstEmittedAt")]
        public DateTime FirstEmittedAt { get; }

        [JsonPropertyName("lastEmittedAt")]
        public DateTime LastEmittedAt { get; }

        [JsonPropertyName("attempt")]
        public int Attempt { get; }

        [JsonPropertyName("status")]
        public string Status { get; }

        [JsonPropertyName("data")]
        public Data Data { get; }

        [JsonPropertyName("links")]
        public Links Links { get; }
    }


}
