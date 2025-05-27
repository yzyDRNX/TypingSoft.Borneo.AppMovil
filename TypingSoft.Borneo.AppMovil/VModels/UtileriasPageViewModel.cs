using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public class UtileriasPageViewModel : INotifyPropertyChanged
    {
        private string _fechaActual;
        public string FechaActual
        {
            get => _fechaActual;
            set
            {
                _fechaActual = value;
                OnPropertyChanged();
            }
        }

        public UtileriasPageViewModel()
        {
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
