using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage; // For Preferences
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json; // For JSON serialization
using System.Threading.Tasks;

namespace MyMauiApp
{
    public partial class HavaDurumuSayfasi : ContentPage
    {
        private const string TemelResimAdresi = "http://www.mgm.gov.tr/sunum/tahmin-show-2.aspx?basla=1&bitir=5&rZ=fff";
        private const string FavoriIllerKey = "HavaDurumuFavoriIller"; 
        
        private List<string> _tumIllerDisplayName = new List<string>(); 
        private List<string> _gosterilenIllerApiAdlari = new List<string>(); 

        public HavaDurumuSayfasi()
        {
            InitializeComponent();
            IlleriYukle(); 
            // KaydedilmisIlleriYukle will be called in OnAppearing, no need for MevcutResimleriYukleVeTakipEt here
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            KaydedilmisIlleriYukle();
        }

        private void IlleriYukle()
        {
            var sehirler = new List<string> 
            {
                "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin",
                "Aydın", "Balıkesir", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa", "Çanakkale",
                "Çankırı", "Çorum", "Denizli", "Diyarbakır", "Edirne", "Elazığ", "Erzincan", "Erzurum", "Eskişehir",
                "Gaziantep", "Giresun", "Gümüşhane", "Hakkari", "Hatay", "Isparta", "Mersin", "İstanbul", "İzmir",
                "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir", "Kocaeli", "Konya", "Kütahya", "Malatya",
                "Manisa", "Kahramanmaraş", "Mardin", "Muğla", "Muş", "Nevşehir", "Niğde", "Ordu", "Rize", "Sakarya",
                "Samsun", "Siirt", "Sinop", "Sivas", "Tekirdağ", "Tokat", "Trabzon", "Tunceli", "Şanlıurfa", "Uşak",
                "Van", "Yozgat", "Zonguldak", "Aksaray", "Bayburt", "Karaman", "Kırıkkale", "Batman", "Şırnak",
                "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye", "Düzce"
            };
            _tumIllerDisplayName.AddRange(sehirler); 
            _tumIllerDisplayName.Sort(); 
        }
        
        private string NormalizeCityNameForApi(string displayName)
        {
            string normalized = displayName.ToUpperInvariant()
                                    .Replace("Ç", "C")
                                    .Replace("Ğ", "G")
                                    .Replace("İ", "I") 
                                    .Replace("Ö", "O")
                                    .Replace("Ş", "S")
                                    .Replace("Ü", "U");
            if (displayName == "Kahramanmaraş") return "K.MARAS"; 
            return normalized;
        }
        
        private string GetDisplayNameForApiName(string apiName)
        {
            if (apiName == "K.MARAS") return "Kahramanmaraş";
            // Find the original display name from _tumIllerDisplayName that normalizes to this apiName
            return _tumIllerDisplayName.FirstOrDefault(dn => NormalizeCityNameForApi(dn) == apiName) ?? apiName;
        }

        private void IlKartiOlusturVeEkle(string displayName, string apiName)
        {
            var yeniIlCercevesi = new Border 
            {
                Padding = new Thickness(10),
                Stroke = Colors.LightGray, 
                StrokeThickness = 1,
                Margin = new Thickness(0,0,0,10)
            };

            var dikeyYerlesim = new VerticalStackLayout { Spacing = 5 };
            var ustGrid = new Grid { ColumnSpacing = 10, VerticalOptions = LayoutOptions.Center };
            ustGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ustGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var ilEtiketi = new Label
            {
                Text = displayName, 
                FontSize = 18, 
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start
            };
            Grid.SetColumn(ilEtiketi, 0);

            var kaldirButonu = new Button
            {
                Text = "Kaldır",
                HorizontalOptions = LayoutOptions.End,
                CommandParameter = yeniIlCercevesi 
            };
            kaldirButonu.Clicked += RemoveCityButton_Clicked; 
            Grid.SetColumn(kaldirButonu, 1);

            ustGrid.Children.Add(ilEtiketi);
            ustGrid.Children.Add(kaldirButonu);

            var havaDurumuResmi = new Image
            {
                WidthRequest = 300, 
                HeightRequest = 100, 
                Aspect = Aspect.AspectFit,
                Source = $"{TemelResimAdresi}&m={apiName}"
            };

            dikeyYerlesim.Children.Add(ustGrid);
            dikeyYerlesim.Children.Add(havaDurumuResmi);
            yeniIlCercevesi.Content = dikeyYerlesim;
            
            var pageSubTitleLabel = MainWeatherLayout.Children.FirstOrDefault(c => c is Label lbl && lbl.Text == "5 Günlük Hava Tahmini");
            int insertIndex = MainWeatherLayout.Children.IndexOf(pageSubTitleLabel ?? AddCityButton) + 1;
            if (insertIndex < 0 || insertIndex > MainWeatherLayout.Children.Count) insertIndex = MainWeatherLayout.Children.Count; // Ensure valid index

            MainWeatherLayout.Children.Insert(insertIndex, yeniIlCercevesi);
        }

        private void KaydedilmisIlleriYukle()
        {
            var dynamicBorders = MainWeatherLayout.Children.OfType<Border>().ToList();
            foreach (var border in dynamicBorders)
            {
                MainWeatherLayout.Children.Remove(border);
            }
            _gosterilenIllerApiAdlari.Clear();

            string kaydedilmisIllerJson = Preferences.Get(FavoriIllerKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(kaydedilmisIllerJson))
            {
                try
                {
                    var illerListesi = JsonSerializer.Deserialize<List<string>>(kaydedilmisIllerJson);
                    if (illerListesi != null && illerListesi.Any()) // Check if list is not null and has items
                    {
                        _gosterilenIllerApiAdlari.AddRange(illerListesi);
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Favori iller deserializasyon hatası: {ex.Message}");
                    _gosterilenIllerApiAdlari.Clear(); 
                }
            }

            if (!_gosterilenIllerApiAdlari.Any())
            {
                _gosterilenIllerApiAdlari.Add("ISTANBUL");
                _gosterilenIllerApiAdlari.Add("ANKARA");
                _gosterilenIllerApiAdlari.Add("BARTIN");
                FavoriIlleriKaydet(); 
            }

            foreach (var apiName in _gosterilenIllerApiAdlari)
            {
                IlKartiOlusturVeEkle(GetDisplayNameForApiName(apiName), apiName);
            }
        }

        private void FavoriIlleriKaydet()
        {
            try
            {
                string kaydedilecekIllerJson = JsonSerializer.Serialize(_gosterilenIllerApiAdlari);
                Preferences.Set(FavoriIllerKey, kaydedilecekIllerJson);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Favori iller serileştirme hatası: {ex.Message}");
            }
        }

        private void RemoveCityButton_Clicked(object? sender, System.EventArgs e)
        {
            if (sender is Button buton && buton.CommandParameter is Border kaldirilacakIlCercevesi)
            {
                if (kaldirilacakIlCercevesi.Content is VerticalStackLayout vsl &&
                    vsl.Children.FirstOrDefault() is Grid headerGrid &&
                    headerGrid.Children.FirstOrDefault(c => c is Label) is Label ilEtiketi)
                {
                    string displayName = ilEtiketi.Text;
                    string apiName = NormalizeCityNameForApi(displayName);
                    bool removed = _gosterilenIllerApiAdlari.Remove(apiName);
                    if(removed) FavoriIlleriKaydet();
                }
                
                if (MainWeatherLayout.Children.Contains(kaldirilacakIlCercevesi))
                {
                     MainWeatherLayout.Children.Remove(kaldirilacakIlCercevesi);
                }
            }
        }

        private async void AddCityButton_Clicked(object? sender, System.EventArgs e)
        {
            var ilSecimSayfasi = new IlSecimSayfasi(_tumIllerDisplayName); 
            await Navigation.PushModalAsync(new NavigationPage(ilSecimSayfasi));
            string? secilenIlDisplayName = await ilSecimSayfasi.WaitForSelectionAsync();

            if (!string.IsNullOrWhiteSpace(secilenIlDisplayName)) 
            {
                string secilenIlApiName = NormalizeCityNameForApi(secilenIlDisplayName);

                if (_gosterilenIllerApiAdlari.Contains(secilenIlApiName))
                {
                    await DisplayAlert("Uyarı", $"{secilenIlDisplayName} zaten listede mevcut.", "Tamam");
                    return;
                }
                
                IlKartiOlusturVeEkle(secilenIlDisplayName, secilenIlApiName);
                _gosterilenIllerApiAdlari.Add(secilenIlApiName);
                FavoriIlleriKaydet();
            }
        }
    }
}
