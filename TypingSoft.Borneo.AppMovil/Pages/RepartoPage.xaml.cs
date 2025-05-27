namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class RepartoPage : ContentPage
{
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;
    public RepartoPage()
	{
        InitializeComponent();

        // Obtener el ViewModel del contenedor de servicios
        if (App.ServiceProvider != null)
        {
            var viewModel = App.ServiceProvider.GetService<VModels.CatalogosVM>();
            if (viewModel != null)
            {
                this.BindingContext = viewModel;
            }
            else
            {
                // Si no se puede obtener del contenedor, crear manualmente
                CrearViewModelManualmente();
            }
        }
        else
        {

            CrearViewModelManualmente();
        }
    }

    private readonly HashSet<Guid> productosSeleccionados = new HashSet<Guid>();

    private void OnAñadirProductoClicked(object sender, EventArgs e)
    {
        var productoSeleccionado = productosPicker.SelectedItem as Models.Custom.ProductosLista;

        if (productoSeleccionado == null)
        {
            DisplayAlert("Advertencia", "Por favor, selecciona un producto.", "OK");
            return;
        }

        if (productosSeleccionados.Contains(productoSeleccionado.Id))
        {
            DisplayAlert("Advertencia", "El producto ya está en la lista.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(cantidadEntry.Text) || !int.TryParse(cantidadEntry.Text, out int cantidad) || cantidad <= 0)
        {
            DisplayAlert("Advertencia", "Ingresa una cantidad válida.", "OK");
            return;
        }

        productosSeleccionados.Add(productoSeleccionado.Id);

        var productoLabel = new Label
        {
            Text = $"{productoSeleccionado.Producto} - Cantidad: {cantidad}",
            FontSize = 14,
            TextColor = Colors.Black,
            Margin = new Thickness(0, 5, 0, 0)
        };

        productosSeleccionadosStack.Children.Add(productoLabel);

        // Limpia el Entry después de añadir
        cantidadEntry.Text = string.Empty;
    }


    private async void OnConcluirClicked(object sender, EventArgs e)
    {
        //Navegacion a UtileriasPage
        await Navigation.PushAsync(new UtileriasPage());
    }
    private void CrearViewModelManualmente()
    {
        var catalogosService = new Services.CatalogosService();
        var catalogosBL = new BL.CatalogosBL(catalogosService);

        // Creamos también la instancia de la BD local
        var localDb = new Services.LocalDatabaseService();

        // Le pasamos ambos al VM
        this.BindingContext = new VModels.CatalogosVM(catalogosBL, localDb);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Cargar los empleados cuando la página aparece
        ViewModel?.ObtenerProductosAsync();
    }

    private async void Concluir(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(UtileriasPage));
    }
}