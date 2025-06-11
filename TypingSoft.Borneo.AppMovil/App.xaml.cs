using TypingSoft.Borneo.AppMovil.Helpers;
using TypingSoft.Borneo.AppMovil.Pages;

namespace TypingSoft.Borneo.AppMovil
{
    public partial class App : Application
    {
        public static CustomNavigation NavigationService { get; private set; }
        public static IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            InitializeComponent();

            // Inicializar el servicio de navegación
            NavigationService = new CustomNavigation();
            GlobalValues.NavegacionGlobal = NavigationService;

        }
        protected override Window CreateWindow(IActivationState? activationState)
        {

            NavigationPage mainPage;
            string IdRuta =Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.IdRuta);

            if (IdRuta != null)
            {
                mainPage = new NavigationPage(new MenuPage());
            } else
            {
                mainPage = new NavigationPage(new InicioPage());
            }
          
            return new Window(mainPage);
        }

    }

}
