using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMauiApp
{
    public partial class IlSecimSayfasi : ContentPage
    {
        private readonly List<string> _tumIller;
        private TaskCompletionSource<string?> _tcs;

        public IlSecimSayfasi(List<string> iller)
        {
            InitializeComponent();
            _tumIller = iller;
            IllerListView.ItemsSource = _tumIller;
            _tcs = new TaskCompletionSource<string?>();
        }

        // Method to be called by the presenting page to get the selection
        public Task<string?> WaitForSelectionAsync()
        {
            return _tcs.Task;
        }

        private void IlSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var aramaMetni = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(aramaMetni))
            {
                IllerListView.ItemsSource = _tumIller;
            }
            else
            {
                IllerListView.ItemsSource = _tumIller
                    .Where(il => il.ToLowerInvariant().Contains(aramaMetni.ToLowerInvariant()))
                    .ToList();
            }
        }

        private async void IllerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is string secilenIl)
            {
                _tcs.TrySetResult(secilenIl);
                await Navigation.PopModalAsync();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // If the page is closed without a selection (e.g., back button), resolve with null
            if (!_tcs.Task.IsCompleted)
            {
                _tcs.TrySetResult(null);
            }
        }
    }
}
