using System.Text.Json.Serialization;

namespace MyMauiApp.Models
{
    public class CurrencyInfo
    {
        [JsonPropertyName("Alış")]
        public string? AlisFiyati { get; set; }

        [JsonPropertyName("Satış")]
        public string? SatisFiyati { get; set; }

        [JsonPropertyName("Tür")]
        public string? Tur { get; set; }

        [JsonPropertyName("Değişim")]
        public string? Degisim { get; set; }
    }
}
