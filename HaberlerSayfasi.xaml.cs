using Microsoft.Maui.Controls;
using MyMauiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.ObjectModel; 
using System.Text.RegularExpressions; 
using System.Web; 

namespace MyMauiApp
{
    public partial class HaberlerSayfasi : ContentPage
    {
        private readonly HttpClient _httpİstemcisi; // Renamed
        private string _mevcutKategori = "gundem"; // Renamed

        private readonly Dictionary<string, string> _kategoriAdresleri = new Dictionary<string, string> // Renamed
        {
            { "gundem", "https://api.rss2json.com/v1/api.json?rss_url=https://haberglobal.com.tr/rss/gundem" },
            { "dunya", "https://api.rss2json.com/v1/api.json?rss_url=https://haberglobal.com.tr/rss/dunya" },
            { "ekonomi", "https://api.rss2json.com/v1/api.json?rss_url=https://haberglobal.com.tr/rss/ekonomi" },
            { "spor", "https://api.rss2json.com/v1/api.json?rss_url=https://haberglobal.com.tr/rss/spor" },
            { "teknoloji", "https://api.rss2json.com/v1/api.json?rss_url=https://haberglobal.com.tr/rss/bilim-teknoloji" }
        };

        public ObservableCollection<NewsItem> HaberListesi { get; set; } = new ObservableCollection<NewsItem>(); // Renamed NewsItems

        public HaberlerSayfasi()
        {
            InitializeComponent();
            _httpİstemcisi = new HttpClient { Timeout = TimeSpan.FromSeconds(30) }; // Renamed
            NewsCollectionView.ItemsSource = HaberListesi; // Renamed NewsItems
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (HaberListesi.Count == 0) // Renamed NewsItems
            {
                await HaberleriYukleAsync(_mevcutKategori); // Renamed LoadNewsAsync, _currentCategory
            }
        }

        private async void OnCategoryClicked(object sender, EventArgs e)
        {
            if (sender is Button tiklananButon && tiklananButon.CommandParameter is string kategoriParam) // Renamed
            {
                _mevcutKategori = kategoriParam; // Renamed
                KategoriButonStilleriniGuncelle(tiklananButon); // Renamed
                await HaberleriYukleAsync(kategoriParam); // Renamed
            }
        }

        private void KategoriButonStilleriniGuncelle(Button seciliButon) // Renamed
        {
            foreach (var child in CategoryNavLayout.Children)
            {
                if (child is Button button)
                {
                    button.Style = (button == seciliButon) 
                        ? (Style)Resources["CategoryButtonSelectedStyle"] 
                        : (Style)Resources["CategoryButtonStyle"];
                }
            }
        }

        private async Task HaberleriYukleAsync(string kategoriAnahtari) // Renamed
        {
            NewsLoadingIndicator.IsVisible = true;
            NewsLoadingIndicator.IsRunning = true;
            HaberListesi.Clear(); // Renamed NewsItems

            try
            {
                if (_kategoriAdresleri.TryGetValue(kategoriAnahtari, out string? apiAdresi)) // Renamed
                {
                    HttpResponseMessage httpYaniti = await _httpİstemcisi.GetAsync(apiAdresi); // Renamed
                    httpYaniti.EnsureSuccessStatusCode();
                    string jsonYanitDizgesi = await httpYaniti.Content.ReadAsStringAsync(); // Renamed

                    var jsonSecenekleri = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; // Renamed
                    NewsApiResponse? haberApiYaniti = JsonSerializer.Deserialize<NewsApiResponse>(jsonYanitDizgesi, jsonSecenekleri); // Renamed

                    if (haberApiYaniti?.Durum == "ok" && haberApiYaniti.Ogeler != null) 
                    {
                        foreach (var haberOgesi in haberApiYaniti.Ogeler) // Renamed
                        {
                            if (!string.IsNullOrEmpty(haberOgesi.Aciklama)) 
                            {
                                haberOgesi.Aciklama = Regex.Replace(haberOgesi.Aciklama, "<.*?>", string.Empty).Trim(); 
                            }
                            HaberListesi.Add(haberOgesi); // Renamed
                        }
                    }
                    else
                    {
                        await DisplayAlert("Hata", $"{kategoriAnahtari} için haberler yüklenemedi. Durum: {haberApiYaniti?.Durum ?? "Bilinmiyor"}", "Tamam");
                    }
                }
                else
                {
                    await DisplayAlert("Hata", "Geçersiz kategori seçildi.", "Tamam");
                }
            }
            catch (HttpRequestException httpIstisnasi) // Renamed
            {
                await DisplayAlert("API Hatası", $"Haberler alınamadı: {httpIstisnasi.Message}", "Tamam");
            }
            catch (JsonException jsonIstisnasi) // Renamed
            {
                await DisplayAlert("Veri Hatası", $"Haber verileri ayrıştırılamadı: {jsonIstisnasi.Message}", "Tamam");
            }
            catch (Exception genelIstisna) // Renamed
            {
                await DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {genelIstisna.Message}", "Tamam");
            }
            finally
            {
                NewsLoadingIndicator.IsVisible = false;
                NewsLoadingIndicator.IsRunning = false;
            }
        }

        private async void OnReadMoreClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is NewsItem seciliHaberOgesi) // Renamed
            {
                try
                {
                    if (!string.IsNullOrEmpty(seciliHaberOgesi.Baglanti) && !string.IsNullOrEmpty(seciliHaberOgesi.Baslik)) 
                    {
                        var haberBaglantisiYerel = HttpUtility.UrlEncode(seciliHaberOgesi.Baglanti); // Renamed
                        var haberBasligiYerel = HttpUtility.UrlEncode(seciliHaberOgesi.Baslik);   // Renamed
                        await Shell.Current.GoToAsync($"{nameof(HaberDetaySayfasi)}?haberBaglantisi={haberBaglantisiYerel}&haberBasligi={haberBasligiYerel}");
                    }
                    else
                    {
                         await DisplayAlert("Hata", "Haber bağlantısı veya başlığı eksik.", "Tamam");
                    }
                }
                catch (Exception genelIstisna) // Renamed
                {
                    await DisplayAlert("Navigasyon Hatası", $"Detay sayfasına gidilemedi: {genelIstisna.Message}", "Tamam");
                }
            }
        }
    }
}
