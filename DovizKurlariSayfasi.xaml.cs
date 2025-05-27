using Microsoft.Maui.Controls;
using MyMauiApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq; 

namespace MyMauiApp
{
    public partial class DovizKurlariSayfasi : ContentPage
    {
        private readonly HttpClient _httpİstemcisi; // Renamed
        private const string ApiAdresi = "https://finans.truncgil.com/today.json"; // Renamed

        // API Keys remain in English as they are external identifiers
        private const string KeyUSD = "USD";
        private const string KeyEUR = "EUR";
        private const string KeyGramGold = "gram-altin";
        private const string KeySilver = "gumus";
        private const string KeyGramPlatinum = "gram-platin";

        public DovizKurlariSayfasi()
        {
            InitializeComponent();
            _httpİstemcisi = new HttpClient(); // Renamed
            _httpİstemcisi.Timeout = TimeSpan.FromSeconds(30); // Renamed
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await KurlariYukleAsync(); // Renamed
        }

        private async void OnRefreshRatesClicked(object sender, EventArgs e)
        {
            await KurlariYukleAsync(); // Renamed
        }

        private async Task KurlariYukleAsync() // Renamed
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            RefreshButton.IsEnabled = false;

            try
            {
                HttpResponseMessage httpYaniti = await _httpİstemcisi.GetAsync(ApiAdresi); // Renamed
                httpYaniti.EnsureSuccessStatusCode(); 

                string jsonYanitDizgesi = await httpYaniti.Content.ReadAsStringAsync(); // Renamed
                
                var jsonSecenekleri = new JsonSerializerOptions // Renamed
                {
                    PropertyNameCaseInsensitive = true 
                };
                ApiResponse? apiYaniti = JsonSerializer.Deserialize<ApiResponse>(jsonYanitDizgesi, jsonSecenekleri); // Renamed

                if (apiYaniti?.KurVerileri != null)
                {
                    UpdateDateLabel.Text = $"Son Güncelleme: {apiYaniti.GuncellemeTarihi ?? "N/A"}"; 

                    KurArayuzunuGuncelle(KeyUSD, apiYaniti.KurVerileri, UsdBuyLabel, UsdSellLabel, UsdChangeLabel); // Renamed
                    KurArayuzunuGuncelle(KeyEUR, apiYaniti.KurVerileri, EurBuyLabel, EurSellLabel, EurChangeLabel); // Renamed
                    KurArayuzunuGuncelle(KeyGramGold, apiYaniti.KurVerileri, GoldGramBuyLabel, GoldGramSellLabel, GoldGramChangeLabel); // Renamed
                    KurArayuzunuGuncelle(KeySilver, apiYaniti.KurVerileri, SilverBuyLabel, SilverSellLabel, SilverChangeLabel); // Renamed
                    KurArayuzunuGuncelle(KeyGramPlatinum, apiYaniti.KurVerileri, PlatinumGramBuyLabel, PlatinumGramSellLabel, PlatinumGramChangeLabel); // Renamed
                }
                else
                {
                    await DisplayAlert("Hata", "Kur verileri ayrıştırılamadı.", "Tamam"); 
                    UpdateDateLabel.Text = "Veri yüklenemedi."; 
                }
            }
            catch (HttpRequestException httpIstisnasi) // Renamed
            {
                await DisplayAlert("API Hatası", $"Kurlar alınamadı: {httpIstisnasi.Message}", "Tamam"); 
                UpdateDateLabel.Text = "API'ye bağlanılamadı."; 
            }
            catch (JsonException jsonIstisnasi) // Renamed
            {
                await DisplayAlert("Veri Hatası", $"Kur verileri ayrıştırılamadı: {jsonIstisnasi.Message}", "Tamam"); 
                UpdateDateLabel.Text = "Veri ayrıştırma hatası."; 
            }
            catch (Exception genelIstisna) // Renamed
            {
                await DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {genelIstisna.Message}", "Tamam"); 
                UpdateDateLabel.Text = "Beklenmedik bir hata oluştu."; 
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                RefreshButton.IsEnabled = true;
            }
        }

        private void KurArayuzunuGuncelle(string anahtar, Dictionary<string, JsonElement> kurVerileriParam, Label buyLabel, Label sellLabel, Label changeLabel) // Renamed method and some params
        {
            if (kurVerileriParam.TryGetValue(anahtar, out JsonElement kurElementi)) // Renamed
            {
                try
                {
                    CurrencyInfo? kurBilgisi = kurElementi.Deserialize<CurrencyInfo>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // Renamed
                    if (kurBilgisi != null) // Renamed
                    {
                        buyLabel.Text = $"Alış: {kurBilgisi.AlisFiyati ?? "N/A"}"; // Renamed
                        sellLabel.Text = $"Satış: {kurBilgisi.SatisFiyati ?? "N/A"}"; // Renamed
                        changeLabel.Text = $"Değişim: {kurBilgisi.Degisim ?? "N/A"}"; // Renamed
                    }
                    else
                    {
                        EtiketleriHatayaAyarla(buyLabel, sellLabel, changeLabel, anahtar, " (veri yok)"); // Renamed method and param
                    }
                }
                catch (JsonException) 
                {
                     EtiketleriHatayaAyarla(buyLabel, sellLabel, changeLabel, anahtar, " (ayrıştırma hatası)"); // Renamed method and param
                }
            }
            else
            {
                EtiketleriHatayaAyarla(buyLabel, sellLabel, changeLabel, anahtar, " (bulunamadı)"); // Renamed method and param
            }
        }
        
        private void EtiketleriHatayaAyarla(Label buyLabel, Label sellLabel, Label changeLabel, string anahtar, string sonEk = "") // Renamed method and some params
        {
            string hataMesaji = $"{anahtar} için veri{sonEk} mevcut değil."; // Renamed
            buyLabel.Text = hataMesaji; // Renamed
            sellLabel.Text = string.Empty;
            changeLabel.Text = string.Empty;
        }
    }
}
