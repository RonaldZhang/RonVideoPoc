using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{
    public class RecordingFile
    {
        [JsonConstructor]
        public RecordingFile(string fileId, int size, int duration )
        {
            this.FileId = fileId;
            this.Size = size;
            this.Duration = duration;
        }

        [JsonPropertyName("fileId")]
        public string FileId { get; }

        [JsonPropertyName("size")]
        public int Size { get; }

        [JsonPropertyName("duration")]
        public int Duration { get; }
    }


}
