using Microsoft.Maui.Controls;
using MyMauiApp.Models;
using System;

namespace MyMauiApp
{
    public partial class GorevEklemeSayfasi : ContentPage
    {
        public event EventHandler<Gorev>? GorevKaydedildi;
        private Gorev? _mevcutGorev; // To store the task being edited

        // Constructor for adding a new task
        public GorevEklemeSayfasi()
        {
            InitializeComponent();
            Title = "Yeni Görev Ekle";
            BitisTarihiPicker.Date = DateTime.Today;
            BitisSaatiPicker.Time = DateTime.Now.TimeOfDay;
        }

        // Constructor for editing an existing task
        public GorevEklemeSayfasi(Gorev gorev) : this() // Calls the default constructor first
        {
            Title = "Görevi Düzenle";
            _mevcutGorev = gorev;

            BaslikEntry.Text = _mevcutGorev.Baslik;
            AciklamaEditor.Text = _mevcutGorev.Aciklama;
            BitisTarihiPicker.Date = _mevcutGorev.BitisTarihiSaati.Date;
            BitisSaatiPicker.Time = _mevcutGorev.BitisTarihiSaati.TimeOfDay;
        }

        private async void TamamButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BaslikEntry.Text))
            {
                await DisplayAlert("Hata", "Görev başlığı boş olamaz.", "Tamam");
                return;
            }

            DateTime bitisTarihi = BitisTarihiPicker.Date;
            TimeSpan bitisSaati = BitisSaatiPicker.Time;
            DateTime bitisTarihiSaati = bitisTarihi.Add(bitisSaati);

            if (_mevcutGorev != null) // Editing existing task
            {
                _mevcutGorev.Baslik = BaslikEntry.Text;
                _mevcutGorev.Aciklama = AciklamaEditor.Text;
                _mevcutGorev.BitisTarihiSaati = bitisTarihiSaati;
                // Tamamlandi status is not edited here, but could be added
                GorevKaydedildi?.Invoke(this, _mevcutGorev); // Pass back the updated task
            }
            else // Adding new task
            {
                var yeniGorev = new Gorev
                {
                    Baslik = BaslikEntry.Text,
                    Aciklama = AciklamaEditor.Text,
                    BitisTarihiSaati = bitisTarihiSaati
                };
                GorevKaydedildi?.Invoke(this, yeniGorev);
            }
            
            await Navigation.PopModalAsync();
        }

        private async void IptalButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
