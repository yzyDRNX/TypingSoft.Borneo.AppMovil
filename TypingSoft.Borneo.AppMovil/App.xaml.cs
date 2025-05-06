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

            // Asignar al acceso global
            GlobalValues.NavegacionGlobal = NavigationService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = new NavigationPage(new LoginPage());
            return new Window(mainPage);
        }
    }

}
