using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.Pages.Modals
{
    public partial class SelectProductoModal : ContentPage
    {
        private readonly List<PreciosGeneralesLocal> _source;
        private readonly TaskCompletionSource<PreciosGeneralesLocal?> _tcs = new();

        public SelectProductoModal(IEnumerable<PreciosGeneralesLocal> productos)
        {
            InitializeComponent();
            _source = productos?.ToList() ?? new();
            itemsView.ItemsSource = _source;
        }

        public async Task<PreciosGeneralesLocal?> ShowAsync(INavigation nav)
        {
            await nav.PushModalAsync(this);
            return await _tcs.Task;
        }

        private async void OnCancel(object sender, System.EventArgs e)
        {
            _tcs.TrySetResult(null);
            await Navigation.PopModalAsync();
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as PreciosGeneralesLocal;
            _tcs.TrySetResult(selected);
            await Navigation.PopModalAsync();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var term = (e.NewTextValue ?? string.Empty).Trim().ToLowerInvariant();
            itemsView.ItemsSource = string.IsNullOrEmpty(term)
                ? _source
                : _source.Where(x => (x.Producto ?? "").ToLowerInvariant().Contains(term)).ToList();
        }
    }
}