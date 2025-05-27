using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyMauiApp.Models
{
    public class Gorev : INotifyPropertyChanged // Implement INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        
        private string? _baslik;
        public string? Baslik
        {
            get => _baslik;
            set => SetProperty(ref _baslik, value);
        }

        private string? _aciklama;
        public string? Aciklama
        {
            get => _aciklama;
            set => SetProperty(ref _aciklama, value);
        }

        private DateTime _bitisTarihiSaati;
        public DateTime BitisTarihiSaati
        {
            get => _bitisTarihiSaati;
            set => SetProperty(ref _bitisTarihiSaati, value);
        }

        private bool _tamamlandi;
        public bool Tamamlandi
        {
            get => _tamamlandi;
            set => SetProperty(ref _tamamlandi, value);
        }

        public Gorev()
        {
            Id = Guid.NewGuid();
            Tamamlandi = false;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
