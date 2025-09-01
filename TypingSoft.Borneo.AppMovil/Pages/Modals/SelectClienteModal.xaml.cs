using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using TypingSoft.Borneo.AppMovil.Models.Custom;

namespace TypingSoft.Borneo.AppMovil.Pages.Modals
{
    public partial class SelectClienteModal : ContentPage
    {
        private readonly List<ClientesLista> _source;
        private readonly TaskCompletionSource<ClientesLista?> _tcs = new();

        public SelectClienteModal(IEnumerable<ClientesLista> clientes)
        {
            InitializeComponent();
            _source = clientes?.ToList() ?? new();
            itemsView.ItemsSource = _source;
        }

        public async Task<ClientesLista?> ShowAsync(INavigation nav)
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
            var selected = e.CurrentSelection?.FirstOrDefault() as ClientesLista;
            _tcs.TrySetResult(selected);
            await Navigation.PopModalAsync();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var term = (e.NewTextValue ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(term))
            {
                itemsView.ItemsSource = _source;
            }
            else
            {
                itemsView.ItemsSource = _source.Where(c => (c.Cliente ?? "").ToLowerInvariant().Contains(term)).ToList();
            }
        }
    }
}