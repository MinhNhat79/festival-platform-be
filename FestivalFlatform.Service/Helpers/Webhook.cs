using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FestivalFlatform.Service.Helpers
{
    public class Webhook
    {
        public class WebhookPayload
        {
            [JsonPropertyName("code")]
            public string? Code { get; set; }
            [JsonPropertyName("status")]
            public string? Status { get; set; }
            [JsonPropertyName("desc")]
            public string? Desc { get; set; } 

            [JsonPropertyName("data")]
            public WebhookData? Data { get; set; }

            public bool? Cancel { get; set; } =  null;


        }

        public class WebhookData
        {
            [JsonPropertyName("orderCode")]
            public long? OrderCode { get; set; }
            [JsonPropertyName("status")]
            public string? Status { get; set; }
            [JsonPropertyName("amount")]
            public long? Amount { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }
        }
    }
}
