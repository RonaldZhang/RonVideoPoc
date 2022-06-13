using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RonVideo.EventModels
{
    public class Fields
    {
        [JsonConstructor]
        public Fields(Application application, SolutionSubType solutionSubType, List<RecordingFile> recordingFiles)
        {
            this.Application = application;
            this.SolutionSubType = solutionSubType;
            this.RecordingFiles = recordingFiles;
        }

        [JsonPropertyName("application")]
        public Application Application { get; }

        [JsonPropertyName("solutionSubType")]
        public SolutionSubType SolutionSubType { get; }

        [JsonPropertyName("recordingFiles")]
        public IReadOnlyList<RecordingFile> RecordingFiles { get; }
    }


}
