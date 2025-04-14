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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Establecer la página inicial dentro de un NavigationPage
            var mainPage = new NavigationPage(new EmpleadosPage());
            return new Window(mainPage);
        }
    }
}
