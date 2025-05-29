using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TypingSoft.Borneo.AppMovil.Models.Custom;

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
        private VentaGeneralRequestDTO _ventaActual;
        public VentaGeneralRequestDTO VentaActual
        {
            get => _ventaActual;
            set
            {
                _ventaActual = value;
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
