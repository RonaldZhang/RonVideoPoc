using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{
    public class Links
    {
        [JsonConstructor]
        public Links( Application application
        )
        {
            this.Application = application;
        }

        [JsonPropertyName("application")]
        public Application Application { get; }
    }


}
