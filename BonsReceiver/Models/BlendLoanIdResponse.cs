using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RonVideo.Models
{
    class BlendLoanIdResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("losId")]
        public string LoanId { get; set; }
    }
}
