using System.Text.Json.Serialization;

namespace MyMauiApp.Models
{
    public class NewsFeed
    {
        [JsonPropertyName("url")]
        public string? Adres { get; set; }

        [JsonPropertyName("title")]
        public string? Baslik { get; set; }

        [JsonPropertyName("link")]
        public string? Baglanti { get; set; }

        [JsonPropertyName("author")]
        public string? Yazar { get; set; }

        [JsonPropertyName("description")]
        public string? Aciklama { get; set; }

        [JsonPropertyName("image")]
        public string? Resim { get; set; }
    }
}
