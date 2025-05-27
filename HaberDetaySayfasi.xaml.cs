using Microsoft.Maui.Controls;
using System.Web; 
using System.Diagnostics; 
using Microsoft.Maui.ApplicationModel.DataTransfer; 

namespace MyMauiApp
{
    [QueryProperty(nameof(HaberBasligi), "haberBasligi")] 
    [QueryProperty(nameof(HaberBaglantisi), "haberBaglantisi")] 
    public partial class HaberDetaySayfasi : ContentPage
    {
        private string? _haberBaglantisi; 
        public string? HaberBaglantisi 
        {
            get => _haberBaglantisi;
            set
            {
                Debug.WriteLine($"[HaberDetaySayfasi] HaberBaglantisi Setter Called. Value (raw): {value}");
                _haberBaglantisi = HttpUtility.UrlDecode(value); 
                Debug.WriteLine($"[HaberDetaySayfasi] HaberBaglantisi Decoded: {_haberBaglantisi}");
                LoadContent();
            }
        }

        private string? _haberBasligi; 
        public string? HaberBasligi 
        {
            get => _haberBasligi;
            set
            {
                Debug.WriteLine($"[HaberDetaySayfasi] HaberBasligi Setter Called. Value (raw): {value}");
                _haberBasligi = HttpUtility.UrlDecode(value);
                Debug.WriteLine($"[HaberDetaySayfasi] HaberBasligi Decoded: {_haberBasligi}");
                if (NewsTitleLabel != null) 
                {
                     NewsTitleLabel.Text = _haberBasligi ?? "Haber Detayı"; 
                }
            }
        }

        public HaberDetaySayfasi()
        {
            InitializeComponent();
            Debug.WriteLine("[HaberDetaySayfasi] Constructor Called");
        }

        private void LoadContent()
        {
            Debug.WriteLine($"[HaberDetaySayfasi] LoadContent Called. Link: {HaberBaglantisi}"); 
            if (NewsWebView == null)
            {
                Debug.WriteLine("[HaberDetaySayfasi] NewsWebView is null in LoadContent!");
                return;
            }

            if (!string.IsNullOrEmpty(HaberBaglantisi)) 
            {
                if (Uri.TryCreate(HaberBaglantisi, UriKind.Absolute, out Uri? webAdresi)) // Renamed webUri
                {
                    NewsWebView.Source = new UrlWebViewSource { Url = webAdresi.ToString() }; // Renamed webUri
                    Debug.WriteLine($"[HaberDetaySayfasi] WebView source set to URL: {webAdresi.ToString()}"); // Renamed webUri
                }
                else
                {
                     NewsWebView.Source = new HtmlWebViewSource
                    {
                        Html = $"<html><body><h1>Geçersiz URL: {HttpUtility.HtmlEncode(HaberBaglantisi)}</h1></body></html>" 
                    };
                    Debug.WriteLine($"[HaberDetaySayfasi] Invalid URL, showing error HTML for: {HaberBaglantisi}"); 
                }
            }
            else
            {
                NewsWebView.Source = new HtmlWebViewSource
                {
                    Html = "<html><body><h1>İçerik URL'si mevcut değil.</h1></body></html>" 
                };
                Debug.WriteLine("[HaberDetaySayfasi] Content URL not available, showing placeholder HTML.");
            }
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs olayVerisi) // Renamed args
        {
            base.OnNavigatedTo(olayVerisi); // Renamed args
            Debug.WriteLine("[HaberDetaySayfasi] OnNavigatedTo Called.");
            if (NewsTitleLabel != null && !string.IsNullOrEmpty(_haberBasligi) && NewsTitleLabel.Text != _haberBasligi) 
            {
                 NewsTitleLabel.Text = _haberBasligi ?? "Haber Detayı"; 
            }
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_haberBaglantisi) && !string.IsNullOrEmpty(_haberBasligi)) 
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = _haberBasligi, 
                    Text = $"Bu habere göz atın: {_haberBasligi}", 
                    Uri = _haberBaglantisi 
                });
            }
            else
            {
                await DisplayAlert("Paylaşım Hatası", "Haber başlığı veya bağlantısı paylaşılamıyor.", "Tamam"); 
            }
        }
    }
}
