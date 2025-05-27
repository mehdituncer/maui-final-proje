using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage; // For Preferences
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel; // For ObservableCollection
using System.ComponentModel; // For INotifyPropertyChanged
using System.Diagnostics;
using System.Runtime.CompilerServices; // For CallerMemberName
using System.Text.Json; // For JSON serialization
using System.Threading.Tasks;
using System.Windows.Input; // For ICommand

namespace MyMauiApp
{
    public class SehirHavaDurumuViewModel : INotifyPropertyChanged
    {
        private string _displayName = string.Empty;
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private string _apiName = string.Empty;
        public string ApiName
        {
            get => _apiName;
            set => SetProperty(ref _apiName, value);
        }

        public string WeatherImageSource => $"http://www.mgm.gov.tr/sunum/tahmin-show-2.aspx?basla=1&bitir=5&rZ=fff&m={ApiName}";

        public ICommand RemoveCommand { get; set; } = new Command(() => {}); // Initialize to a default non-null command

        public event PropertyChangedEventHandler? PropertyChanged; // Made nullable

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            System.Action? onChanged = null) // Made nullable
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            if (propertyName == nameof(ApiName)) // If ApiName changes, WeatherImageSource also changes
            {
                OnPropertyChanged(nameof(WeatherImageSource));
            }
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class HavaDurumuSayfasi : ContentPage
    {
        private const string TemelResimAdresi = "http://www.mgm.gov.tr/sunum/tahmin-show-2.aspx?basla=1&bitir=5&rZ=fff"; // Kept for reference, but image source is in ViewModel
        private const string FavoriIllerKey = "HavaDurumuFavoriIller";

        private List<string> _tumIllerDisplayName = new List<string>();
        public ObservableCollection<SehirHavaDurumuViewModel> GosterilenIller { get; set; }

        public HavaDurumuSayfasi()
        {
            InitializeComponent();
            IlleriYukle();
            GosterilenIller = new ObservableCollection<SehirHavaDurumuViewModel>();
            BindingContext = this; // Set BindingContext for the page to itself for CollectionView binding
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
                                    .Replace("Ü", "U")
                                    .Replace("ç", "C")
                                    .Replace("ğ", "G")
                                    .Replace("ı", "I")
                                    .Replace("ö", "O")
                                    .Replace("ş", "S")
                                    .Replace("ü", "U");
            if (displayName == "Kahramanmaraş") return "K.MARAS";
            return normalized;
        }

        private string GetDisplayNameForApiName(string apiName)
        {
            if (apiName == "K.MARAS") return "Kahramanmaraş";
            return _tumIllerDisplayName.FirstOrDefault(dn => NormalizeCityNameForApi(dn) == apiName) ?? apiName;
        }

        private void KaydedilmisIlleriYukle()
        {
            GosterilenIller.Clear();
            List<string> kaydedilmisApiAdlari = new List<string>();

            string kaydedilmisIllerJson = Preferences.Get(FavoriIllerKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(kaydedilmisIllerJson))
            {
                try
                {
                    var illerListesi = JsonSerializer.Deserialize<List<string>>(kaydedilmisIllerJson);
                    if (illerListesi != null && illerListesi.Any())
                    {
                        kaydedilmisApiAdlari.AddRange(illerListesi);
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Favori iller deserializasyon hatası: {ex.Message}");
                    kaydedilmisApiAdlari.Clear();
                }
            }

            if (!kaydedilmisApiAdlari.Any())
            {
                kaydedilmisApiAdlari.Add("ISTANBUL");
                kaydedilmisApiAdlari.Add("ANKARA");
                kaydedilmisApiAdlari.Add("BARTIN");
                FavoriIlleriKaydet(kaydedilmisApiAdlari); // Save defaults if none exist
            }

            foreach (var apiName in kaydedilmisApiAdlari)
            {
                GosterilenIller.Add(new SehirHavaDurumuViewModel
                {
                    ApiName = apiName,
                    DisplayName = GetDisplayNameForApiName(apiName),
                    RemoveCommand = new Command<SehirHavaDurumuViewModel>(RemoveCity)
                });
            }
        }

        private void FavoriIlleriKaydet(List<string>? apiAdlariListesi = null) // Made nullable
        {
            try
            {
                // If a list is provided, use it; otherwise, use the ApiNames from GosterilenIller
                var apiAdlari = apiAdlariListesi ?? GosterilenIller.Select(vm => vm.ApiName).ToList();
                string kaydedilecekIllerJson = JsonSerializer.Serialize(apiAdlari);
                Preferences.Set(FavoriIllerKey, kaydedilecekIllerJson);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Favori iller serileştirme hatası: {ex.Message}");
            }
        }
        
        private void RemoveCity(SehirHavaDurumuViewModel cityToRemove)
        {
            if (cityToRemove != null && GosterilenIller.Contains(cityToRemove))
            {
                GosterilenIller.Remove(cityToRemove);
                FavoriIlleriKaydet();
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

                if (GosterilenIller.Any(vm => vm.ApiName == secilenIlApiName))
                {
                    await DisplayAlert("Uyarı", $"{secilenIlDisplayName} zaten listede mevcut.", "Tamam");
                    return;
                }

                GosterilenIller.Add(new SehirHavaDurumuViewModel
                {
                    ApiName = secilenIlApiName,
                    DisplayName = secilenIlDisplayName,
                    RemoveCommand = new Command<SehirHavaDurumuViewModel>(RemoveCity)
                });
                FavoriIlleriKaydet();
            }
        }
    }
}
