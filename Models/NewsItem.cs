using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyMauiApp.Models
{
    public class NewsItem
    {
        [JsonPropertyName("title")]
        public string? Baslik { get; set; }

        [JsonPropertyName("pubDate")]
        public string? YayinTarihi { get; set; }

        [JsonPropertyName("link")]
        public string? Baglanti { get; set; }

        [JsonPropertyName("guid")]
        public string? Kimlik { get; set; }

        [JsonPropertyName("author")]
        public string? Yazar { get; set; }

        [JsonPropertyName("thumbnail")]
        public string? KucukResim { get; set; }

        [JsonPropertyName("description")]
        public string? Aciklama { get; set; }

        [JsonPropertyName("content")]
        public string? Icerik { get; set; }

        [JsonPropertyName("categories")]
        public List<string>? Kategoriler { get; set; }
    }
}
