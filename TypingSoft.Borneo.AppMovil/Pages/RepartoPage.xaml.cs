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
        // Obtener el producto seleccionado del Picker
        var productoSeleccionado = productosPicker.SelectedItem as Models.Custom.ProductosLista;

        if (productoSeleccionado != null)
        {
            // Verificar si el producto ya está en la lista
            if (productosSeleccionados.Contains(productoSeleccionado.Id))
            {
                // Mostrar un mensaje de alerta si el producto ya está en la lista
                DisplayAlert("Advertencia", "El producto ya está en la lista.", "OK");
                return;
            }

            // Añadir el producto al HashSet
            productosSeleccionados.Add(productoSeleccionado.Id);

            // Crear un nuevo Label para mostrar el nombre del producto seleccionado
            var productoLabel = new Label
            {
                Text = productoSeleccionado.Producto, // Accede directamente a la propiedad 'Producto'
                FontSize = 14,
                TextColor = Colors.Black,
                Margin = new Thickness(0, 5, 0, 0)
            };

            // Añadir el Label al StackLayout
            productosSeleccionadosStack.Children.Add(productoLabel);
        }
        else
        {
            // Mostrar un mensaje si no se seleccionó ningún producto
            DisplayAlert("Advertencia", "Por favor, selecciona un producto antes de añadirlo.", "OK");
        }
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