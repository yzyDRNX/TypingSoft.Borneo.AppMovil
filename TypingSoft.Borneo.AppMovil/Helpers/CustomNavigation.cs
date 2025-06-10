

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class CustomNavigation
    {
        public async Task Atras()
        { }

        public async Task Navegar(string pagina, params object[] parametros)
        {
            switch (pagina)
            {
                case nameof(Pages.LoginPage):
                    App.Current.MainPage = new NavigationPage(new Pages.LoginPage());
                    break;
                case nameof(Pages.EmpleadosPage):
                    App.Current.MainPage = new NavigationPage(new Pages.EmpleadosPage());
                    break;
                case nameof(Pages.ClientePage):
                    App.Current.MainPage = new NavigationPage(new Pages.ClientePage());
                    break;
                case nameof(Pages.RepartoPage):
                    App.Current.MainPage = new NavigationPage(new Pages.RepartoPage());
                    break;
                case nameof(Pages.UtileriasPage):
                    App.Current.MainPage = new NavigationPage(new Pages.UtileriasPage());
                    break;
                case nameof(Pages.MenuPage):
                    App.Current.MainPage = new NavigationPage(new Pages.MenuPage());
                    break;
                case nameof(Pages.ImpresionPage):
                    App.Current.MainPage = new NavigationPage(new Pages.ImpresionPage());
                    break;
                case nameof( Pages.InicioPage):
                    App.Current.MainPage = new NavigationPage(new Pages.InicioPage());
                    break;
                default:
                    break;
            }
            return;
        }

     
    }
}
