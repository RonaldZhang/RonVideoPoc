using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RonVideo.Models
{
    class BlendUrlResponse
    {
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}
