namespace TypingSoft.Borneo.AppMovil.Pages;
using Microsoft.Maui.Controls;
using System;
using TypingSoft.Borneo.AppMovil.Models.API; // Asegúrate de que esta referencia sea correcta
using TypingSoft.Borneo.AppMovil.VModels;


public partial class MenuPage : ContentPage
{
     public MenuPage()
	{
		InitializeComponent();
        var viewModel = App.ServiceProvider.GetService<VModels.MenuVM>();
        if (viewModel != null)
            this.BindingContext = viewModel;
    }
   

}