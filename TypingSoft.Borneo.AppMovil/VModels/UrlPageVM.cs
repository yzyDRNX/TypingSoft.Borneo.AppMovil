using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TypingSoft.Borneo.AppMovil.Helpers;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class UrlPageVM : ObservableObject
    {
        [ObservableProperty]
        private string urlInput = Settings.UrlBaseAPI;

        [RelayCommand]
        public void GuardarUrl()
        {
            if (!string.IsNullOrWhiteSpace(UrlInput))
                Settings.UrlBaseAPI = UrlInput.Trim();
        }
    }
}
