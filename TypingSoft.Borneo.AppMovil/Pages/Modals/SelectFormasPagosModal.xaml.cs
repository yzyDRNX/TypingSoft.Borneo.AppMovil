
using TypingSoft.Borneo.AppMovil.Models.Custom;

namespace Borneo.Pages.Modals;

public partial class SelectFormasPagosModal : ContentPage
{
    private readonly TaskCompletionSource<FormasLista?> _tcs = new();
    private readonly List<FormasLista> _source;
    public SelectFormasPagosModal(IEnumerable<FormasLista> productos)
	{
		InitializeComponent();
        _source = productos?.ToList() ?? new();
        itemsView.ItemsSource = _source;
    }
    public async Task<FormasLista?> ShowAsync(INavigation nav)
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
        var selected = e.CurrentSelection?.FirstOrDefault() as FormasLista;
        _tcs.TrySetResult(selected);
        await Navigation.PopModalAsync();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var term = (e.NewTextValue ?? string.Empty).Trim().ToLowerInvariant();
        itemsView.ItemsSource = string.IsNullOrEmpty(term)
            ? _source
            : _source.Where(x => (x.Forma ?? "").ToLowerInvariant().Contains(term)).ToList();
    }
}