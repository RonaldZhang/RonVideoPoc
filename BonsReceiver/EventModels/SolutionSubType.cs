using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{

    public class SolutionSubType
    {
        [JsonConstructor]
        public SolutionSubType( string value)
        {
            this.Value = value;
        }

        [JsonPropertyName("value")]
        public string Value { get; }
    }


}
