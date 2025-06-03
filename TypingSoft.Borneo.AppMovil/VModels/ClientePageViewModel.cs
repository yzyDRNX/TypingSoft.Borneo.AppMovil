using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public class ClientePageViewModel : INotifyPropertyChanged
    {
        private string _descripcionRuta;
        public string DescripcionRuta
        {
            get => _descripcionRuta;
            set
            {
                _descripcionRuta = value;
                OnPropertyChanged(nameof(DescripcionRuta));
            }
        }

        public ClientePageViewModel() // <-- Corregido aquí
        {
            CargarDescripcionRuta();
        }

        private async void CargarDescripcionRuta()
        {
            var db = new LocalDatabaseService();
            DescripcionRuta = await db.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
