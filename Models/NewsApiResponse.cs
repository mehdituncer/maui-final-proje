using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyMauiApp.Models
{
    public class NewsApiResponse
    {
        [JsonPropertyName("status")]
        public string? Durum { get; set; }

        [JsonPropertyName("feed")]
        public NewsFeed? Kaynak { get; set; }

        [JsonPropertyName("items")]
        public List<NewsItem>? Ogeler { get; set; }
    }
}
