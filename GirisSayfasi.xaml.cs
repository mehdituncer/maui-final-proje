using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel; 

namespace MyMauiApp
{
    public partial class GirisSayfasi : ContentPage
    {
        private const string FirebaseApiAnahtari = "AIzaSyDRixf_0So_CoqdbvvvVO2PEXWNuKL2sAw"; 
        private FirebaseAuthClient _kimlikDogrulamaIstemcisi; // Renamed _authClient

        public GirisSayfasi()
        {
            InitializeComponent();
            _kimlikDogrulamaIstemcisi = new FirebaseAuthClient(new FirebaseAuthConfig // Renamed _authClient
            {
                ApiKey = FirebaseApiAnahtari, // Renamed FirebaseApiKey
                AuthDomain = "fir-maui-611de.firebaseapp.com", 
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var ePosta = EmailEntry.Text; // Renamed email
                var sifre = PasswordEntry.Text; // Renamed password

                if (string.IsNullOrWhiteSpace(ePosta) || string.IsNullOrWhiteSpace(sifre)) // Renamed email, password
                {
                    await DisplayAlert("Hata", "E-posta ve şifre boş olamaz.", "Tamam");
                    return;
                }

                var kimlikDogrulamaBaglantisi = await _kimlikDogrulamaIstemcisi.SignInWithEmailAndPasswordAsync(ePosta, sifre); // Renamed authLink, _authClient, email, password
                var kullanici = kimlikDogrulamaBaglantisi.User; // Renamed user, authLink

                if (kullanici != null) // Renamed user
                {
                    await DisplayAlert("Başarılı", $"Giriş yapıldı: {ePosta}", "Tamam"); // Renamed email
                    
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await Shell.Current.GoToAsync("//AnaSayfa"); 
                        }
                        catch (Exception navigasyonHatasi) // Renamed navEx
                        {
                            await DisplayAlert("Navigasyon Hatası", $"Giriş sonrası navigasyon başarısız: {navigasyonHatasi.Message}", "Tamam");
                        }
                    });
                }
            }
            catch (FirebaseAuthException firebaseHatasi) // Renamed ex
            {
                await DisplayAlert("Giriş Başarısız", firebaseHatasi.Reason.ToString(), "Tamam");
            }
            catch (Exception genelHata) // Renamed ex
            {
                await DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {genelHata.Message}", "Tamam");
            }
        }

        async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(KayitOlSayfasi)); 
        }
    }
}
