using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyMauiApp.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("Update_Date")]
        public string? GuncellemeTarihi { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? KurVerileri { get; set; }
    }
}
