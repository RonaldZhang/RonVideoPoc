using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RonVideo.Models
{
    //Add a comment

    class BlendUrlResponse
    {
        [JsonPropertyName("expiresAt")]
        public string ExpiresAt { get; set; }

        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}
