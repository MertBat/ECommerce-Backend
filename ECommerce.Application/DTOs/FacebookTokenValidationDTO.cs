using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs
{
    public class FacebookTokenValidationDTO
    {
        [JsonPropertyName("data")]
        public FacebookTokenValidationData Data { get; set; }
    }
     public class FacebookTokenValidationData
    {
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}
