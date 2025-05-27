using Microsoft.Maui.Controls;
using MyMauiApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Firebase.Database;
using Firebase.Database.Query;

namespace MyMauiApp
{
    public partial class YapilacaklarSayfasi : ContentPage
    {
        public ObservableCollection<Gorev> GorevListesi { get; set; }
        private FirebaseClient firebaseClient;

        public ICommand ToggleTamamlandiCommand { get; }
        public ICommand SilGorevCommand { get; }
        public ICommand DuzenleGorevCommand { get; } // New Edit Command

        // TODO: Firebase Realtime Database URL'nizi buraya girin
        private const string FirebaseDbUrl = "https://fir-maui-611de-default-rtdb.europe-west1.firebasedatabase.app/";
        private const string GorevlerNode = "Gorevler";

        public YapilacaklarSayfasi()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient(FirebaseDbUrl);
            GorevListesi = new ObservableCollection<Gorev>();
            GorevlerCollectionView.ItemsSource = GorevListesi;

            ToggleTamamlandiCommand = new Command<Gorev>(async (gorev) => await OnToggleTamamlandi(gorev));
            SilGorevCommand = new Command<Gorev>(async (gorev) => await OnSilGorev(gorev));
            DuzenleGorevCommand = new Command<Gorev>(async (gorev) => await OnDuzenleGorev(gorev));
            
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await VerileriYukle();
        }

        private async Task VerileriYukle()
        {
            try
            {
                var items = await firebaseClient
                    .Child(GorevlerNode)
                    .OnceAsync<Gorev>();

                GorevListesi.Clear();
                foreach (var item in items)
                {
                    // Firebase'den gelen Id string olduğu için Guid'e parse ediyoruz.
                    // Eğer Firebase'in kendi key'lerini kullanıyorsanız bu kısım değişebilir.
                    var gorev = item.Object;
                    if (Guid.TryParse(item.Key, out Guid parsedId))
                    {
                        gorev.Id = parsedId; // Firebase key'ini Gorev nesnesinin Id'sine atıyoruz.
                    }
                    GorevListesi.Add(gorev);
                }
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("Hata", $"Görevler yüklenirken bir hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task OnToggleTamamlandi(Gorev gorev)
        {
            if (gorev != null)
            {
                gorev.Tamamlandi = !gorev.Tamamlandi;
                try
                {
                    await firebaseClient
                        .Child(GorevlerNode)
                        .Child(gorev.Id.ToString()) // Guid'i string'e çeviriyoruz
                        .PutAsync(gorev);
                }
                catch (System.Exception ex)
                {
                    await DisplayAlert("Hata", $"Görev güncellenirken bir hata oluştu: {ex.Message}", "Tamam");
                    gorev.Tamamlandi = !gorev.Tamamlandi; // Başarısız olursa geri al
                }
            }
        }

        private async Task OnSilGorev(Gorev gorev)
        {
            if (gorev != null)
            {
                bool silmeyiOnayla = await DisplayAlert(
                    "Görevi Sil",
                    $"'{gorev.Baslik}' başlıklı görevi silmek istediğinize emin misiniz?",
                    "Sil",
                    "İptal");

                if (silmeyiOnayla)
                {
                    try
                    {
                        await firebaseClient
                            .Child(GorevlerNode)
                            .Child(gorev.Id.ToString()) // Guid'i string'e çeviriyoruz
                            .DeleteAsync();
                        GorevListesi.Remove(gorev);
                    }
                    catch (System.Exception ex)
                    {
                        await DisplayAlert("Hata", $"Görev silinirken bir hata oluştu: {ex.Message}", "Tamam");
                    }
                }
            }
        }

        private async Task OnDuzenleGorev(Gorev gorev)
        {
            if (gorev == null) return;

            var gorevEklemeSayfasi = new GorevEklemeSayfasi(gorev);
            
            gorevEklemeSayfasi.GorevKaydedildi += async (s, duzenlenmisGorev) =>
            {
                if (duzenlenmisGorev != null)
                {
                    try
                    {
                        // Gorev nesnesinin Id'si zaten doğru olmalı (düzenlenen görevin Id'si)
                        await firebaseClient
                            .Child(GorevlerNode)
                            .Child(duzenlenmisGorev.Id.ToString()) // Guid'i string'e çeviriyoruz
                            .PutAsync(duzenlenmisGorev);
                        
                        // ObservableCollection'daki öğeyi güncellemek yerine,
                        // INotifyPropertyChanged düzgün çalışıyorsa UI güncellenmeli.
                        // Eğer sorun olursa, eskiyi silip yeniyi ekleyebilirsiniz:
                        // var index = GorevListesi.IndexOf(gorev);
                        // if (index != -1)
                        // {
                        //    GorevListesi[index] = duzenlenmisGorev;
                        // }
                    }
                    catch (System.Exception ex)
                    {
                        await DisplayAlert("Hata", $"Görev güncellenirken bir hata oluştu: {ex.Message}", "Tamam");
                    }
                }
            };
            
            await Navigation.PushModalAsync(new NavigationPage(gorevEklemeSayfasi));
        }

        private async void GorevEkleButton_Clicked(object sender, System.EventArgs e)
        {
            var gorevEklemeSayfasi = new GorevEklemeSayfasi();
            
            gorevEklemeSayfasi.GorevKaydedildi += async (s, yeniGorev) =>
            {
                if (yeniGorev != null)
                {
                    try
                    {
                        // Firebase'e yeni görevi eklerken, Firebase'in oluşturduğu key'i kullanmak yerine
                        // Gorev nesnesinin kendi Id'sini (Guid) key olarak kullanıyoruz.
                        await firebaseClient
                            .Child(GorevlerNode)
                            .Child(yeniGorev.Id.ToString()) // Guid'i string'e çeviriyoruz
                            .PutAsync(yeniGorev);
                        
                        if (!GorevListesi.Any(g => g.Id == yeniGorev.Id))
                        {
                            GorevListesi.Add(yeniGorev);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        await DisplayAlert("Hata", $"Görev eklenirken bir hata oluştu: {ex.Message}", "Tamam");
                    }
                }
            };
            
            await Navigation.PushModalAsync(new NavigationPage(gorevEklemeSayfasi));
        }
    }
}
