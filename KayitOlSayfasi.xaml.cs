using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace MyMauiApp
{
    public partial class KayitOlSayfasi : ContentPage
    {
        private const string FirebaseApiAnahtari = "AIzaSyDRixf_0So_CoqdbvvvVO2PEXWNuKL2sAw"; 
        private FirebaseAuthClient _kimlikDogrulamaIstemcisi; // Renamed _authClient

        public KayitOlSayfasi()
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

        async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var ePosta = EmailEntry.Text; // Renamed email
                var sifre = PasswordEntry.Text; // Renamed password
                var sifreTekrar = ConfirmPasswordEntry.Text; // Renamed confirmPassword

                if (string.IsNullOrWhiteSpace(ePosta) || string.IsNullOrWhiteSpace(sifre) || string.IsNullOrWhiteSpace(sifreTekrar)) // Renamed variables
                {
                    await DisplayAlert("Hata", "Tüm alanlar zorunludur.", "Tamam");
                    return;
                }

                if (sifre != sifreTekrar) // Renamed variables
                {
                    await DisplayAlert("Hata", "Şifreler eşleşmiyor.", "Tamam");
                    return;
                }

                var kullaniciKimlikBilgileri = await _kimlikDogrulamaIstemcisi.CreateUserWithEmailAndPasswordAsync(ePosta, sifre); // Renamed userCredential, _authClient, email, password
                var kullanici = kullaniciKimlikBilgileri.User; // Renamed user, userCredential

                if (kullanici != null) // Renamed user
                {
                    await DisplayAlert("Başarılı", $"Kullanıcı {ePosta} başarıyla kaydedildi. Lütfen giriş yapın.", "Tamam"); // Renamed email
                    await Shell.Current.GoToAsync($"//{nameof(GirisSayfasi)}"); 
                }
            }
            catch (FirebaseAuthException firebaseHatasi) // Renamed ex
            {
                string hataMesaji = firebaseHatasi.Reason switch // Renamed errorMessage, firebaseHatasi
                {
                    AuthErrorReason.EmailExists => "Bu e-posta adresi zaten kullanılıyor.",
                    AuthErrorReason.WeakPassword => "Şifre çok zayıf. En az 6 karakter olmalıdır.",
                    _ => $"Bir hata oluştu: {firebaseHatasi.Reason}" // Renamed firebaseHatasi
                };
                await DisplayAlert("Kayıt Başarısız", hataMesaji, "Tamam"); // Renamed errorMessage
            }
            catch (Exception genelHata) // Renamed ex
            {
                await DisplayAlert("Hata", $"Beklenmedik bir hata oluştu: {genelHata.Message}", "Tamam");
            }
        }

        async void OnBackToLoginButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(GirisSayfasi)}"); 
        }
    }
}
