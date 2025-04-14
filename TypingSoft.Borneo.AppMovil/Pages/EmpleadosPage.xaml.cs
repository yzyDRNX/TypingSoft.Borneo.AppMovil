using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace TypingSoft.Borneo.AppMovil.Pages;


public partial class EmpleadosPage : ContentPage
{
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;

    public EmpleadosPage()
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
            // Si el ServiceProvider es null, crear el ViewModel manualmente
            CrearViewModelManualmente();
        }



    }
    private void CrearViewModelManualmente()
    {
        var catalogosService = new Services.CatalogosService();
        var catalogosBL = new BL.CatalogosBL(catalogosService);
        this.BindingContext = new VModels.CatalogosVM(catalogosBL);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Cargar los empleados cuando la página aparece
        ViewModel?.ObtenerEmpleados();
    }


}