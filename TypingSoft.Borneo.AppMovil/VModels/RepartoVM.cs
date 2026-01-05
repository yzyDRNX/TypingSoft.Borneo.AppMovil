using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class RepartoVM : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;

        public RepartoVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;
            ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>();
            ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>();
            ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>();
            ListadoPreciosGenerales = new ObservableCollection<Models.Custom.PreciosGeneralesLista>();
            ListadoPreciosPreferenciales = new ObservableCollection<Models.Custom.PreciosPreferencialesLista>();
            ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>();
            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
       
        }

        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;


        [ObservableProperty]
        ObservableCollection<Models.Custom.ProductosLista> listadoProductos;

        [ObservableProperty]
        ObservableCollection<Models.Custom.FormasLista> listadoFormas;

        [ObservableProperty]
        ObservableCollection<Models.Custom.CondicionesLista> listadoCondiciones;
        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosPreferencialesLista> listadoPreciosPreferenciales;

        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosGeneralesLista> listadoPreciosGenerales;

        [ObservableProperty]
        ObservableCollection<PreciosGeneralesLocal> listadoPreciosLocal = new();
        [ObservableProperty]
        Guid idClienteAsociado;
        [ObservableProperty]
        Guid idCondicionPago;

        public async Task CargarProductosDesdeLocal()
        {
            var productosLocales = await _localDb.ObtenerProductosAsync();

            if (productosLocales != null && productosLocales.Any())
            {
                ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>(
                    productosLocales.Select(p => new Models.Custom.ProductosLista
                    {
                        Id = p.Id,
                        Producto = p.Producto
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando productos locales.");
            }
            else
            {
                await MostrarAlertaAsync("Advertencia", "No hay productos locales disponibles.");
            }
        }

        public async Task CargarFormasPagosLocal()
        {
            string tipoPAgo = "";

            this.IdClienteAsociado= Helpers.Settings.IdClienteAsociado;
            var conidcionespagosLocales = await _localDb.ObtenerCondicionesPagoPorClienteAsync(IdClienteAsociado);
            if (conidcionespagosLocales!=null)
            {
                this.idCondicionPago = conidcionespagosLocales.FirstOrDefault().IdCondicionPago;
                //4AA68BBA - 5C73 - 4028 - A4CB - B3101A4906FA->contado
                if (conidcionespagosLocales.FirstOrDefault().IdCondicionPago.ToString().ToUpper()== "4AA68BBA-5C73-4028-A4CB-B3101A4906FA")
                {
                    tipoPAgo = "VALES,EFECTIVO";
                }
            }
            var formasLocales = await _localDb.ObtenerFormasAsync();
            if (formasLocales != null && formasLocales.Any())
            {
                if (tipoPAgo.Length > 0)
                {
                    ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>(
                       formasLocales.Select(f => new Models.Custom.FormasLista
                       {
                           IdForma = f.IdForma,
                           Forma = f.Forma
                       }).Where(f => f.Forma == tipoPAgo.Split(",")[0] || f.Forma == tipoPAgo.Split(",")[1])
                   );

                }
                else
                {
                    ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>(
                       formasLocales.Select(f => new Models.Custom.FormasLista
                       {
                           IdForma = f.IdForma,
                           Forma = f.Forma
                       }).Where(f=>f.Forma== "POR DEFINIR")
                   );
                }
                   
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando formas de pago locales.");
            }
            else
            {
                await MostrarAlertaAsync("Advertencia", "No hay formas de pago locales disponibles.");
            }

        }

        //public async Task ObtenerFormasAsync()
        //{
        //    try
        //    {
        //        MensajeProcesando = "Cargando Formas";
        //        Procesando = true;

        //        var (exitoso, mensaje, listaFormas) = await _catalogos.ObtenerFormas();

        //        if (exitoso)
        //        {
        //            ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>(listaFormas);

        //            // Convierte y guarda en SQLite
        //            var formasLocales = listaFormas
        //                .Select(f => new Local.FormaLocal { IdForma = f.IdForma, Forma = f.Forma })
        //                .ToList();

        //            await _localDb.GuardarFormasAsync(formasLocales);
        //        }
        //        else
        //        {
        //            await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener formas.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await MostrarAlertaAsync("Excepción", ex.Message);
        //    }
        //    finally
        //    {
        //        Procesando = false;
        //    }
        //}

        //public async Task ObtenerCondicionesAsync()
        //{
        //    try
        //    {
        //        MensajeProcesando = "Cargando Condiciones";
        //        Procesando = true;

        //        var (exitoso, mensaje, listaCondiciones) = await _catalogos.ObtenerCondiciones();

        //        if (exitoso)
        //        {
        //            ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>(listaCondiciones);

        //            // Convierte y guarda en SQLite
        //            var condicionesLocales = listaCondiciones
        //                .Select(c => new Local.CondicionLocal { IdCondicion = c.IdCondicion, Condicion = c.Condicion })
        //                .ToList();

        //            await _localDb.GuardarCondicionesAsync(condicionesLocales);
        //        }
        //        else
        //        {
        //            await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener formas.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await MostrarAlertaAsync("Excepción", ex.Message);
        //    }
        //    finally
        //    {
        //        Procesando = false;
        //    }
        //}

        //public async Task ObtenerPreciosGeneralesAsync()
        //{
        //    try
        //    {
        //        MensajeProcesando = "Cargando Precios";
        //        Procesando = true;

        //        Guid IdClienteAsociado = Helpers.Settings.IdClienteAsociado;
        //        var (exitoso, mensaje, listaPrecios) = await _catalogos.ObtenerPreciosGenerales();

        //        if (exitoso)
        //        {
        //            ListadoPreciosGenerales = new ObservableCollection<Models.Custom.PreciosGeneralesLista>(listaPrecios);

        //            // Convierte y guarda en SQLite
        //            var preciosLocales = listaPrecios
        //                .Select(p => new Local.PreciosGeneralesLocal { IdProducto = p.IdProducto, Producto = p.Producto, Precio = p.Precio.ToString() })
        //                .ToList();

        //            await _localDb.GuardarPreciosGeneralesAsync(preciosLocales);
        //        }
        //        else
        //        {
        //            await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener Precios.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await MostrarAlertaAsync("Excepción", ex.Message);
        //    }
        //    finally
        //    {
        //        Procesando = false;
        //    }
        //}

        public async Task CargarPreciosDesdeLocal()
        {
            var preciosLocales = await _localDb.ObtenerPreciosAsync(); // Este método debe devolver List<PrecioLocal>
            if (preciosLocales != null && preciosLocales.Any())
                ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>(preciosLocales);
            else
                ListadoPreciosLocal.Clear();
        }

        public async Task CargarPreciosPorClienteAsync(Guid idClienteAsociado)
        {
            // Intentar leer preferenciales desde SQLite
            var preciosPreferenciales = await _localDb.ObtenerPreciosPreferencialesPorClienteAsync(idClienteAsociado);

            // Si no hay, intenta traerlos del backend, guardarlos y reintentar
            if (preciosPreferenciales == null || !preciosPreferenciales.Any())
            {
                try
                {
                    MensajeProcesando = "Cargando precios preferenciales";
                    Procesando = true;

                    var (exitoso, mensaje, listaPref) = await _catalogos.ObtenerPreciosPreferenciales();
                    if (exitoso && listaPref != null && listaPref.Any())
                    {
                        // Mapear a entidad local y persistir
                        var prefLocales = listaPref.Select(p => new Local.PreciosPreferencialesLocal
                        {
                            IdProducto = p.IdProducto,
                            Producto = p.Producto,
                            // Nota: si p.Precio es decimal, ToString() mantiene la coherencia con los generales
                            Precio = p.Precio.ToString() ,
                            IdClienteAsociado = p.IdClienteAsociado
                        }).ToList();

                        await _localDb.GuardarPreciosPreferencialesAsync(prefLocales);

                        // Reintentar leer los del cliente
                        preciosPreferenciales = await _localDb.ObtenerPreciosPreferencialesPorClienteAsync(idClienteAsociado);
                    }
                    else if (!string.IsNullOrWhiteSpace(mensaje))
                    {
                        await MostrarAlertaAsync("Info", mensaje);
                    }
                }
                catch (Exception ex)
                {
                    await MostrarAlertaAsync("Excepción", ex.Message);
                }
                finally
                {
                    Procesando = false;
                }
            }

            if (preciosPreferenciales != null && preciosPreferenciales.Any())
            {
                // Merge: preferencial cuando exista, general de fallback
                var preciosGenerales = await _localDb.ObtenerPreciosGeneralesAsync();

                var merged =
                    from g in (preciosGenerales ?? Enumerable.Empty<PreciosGeneralesLocal>())
                    join pref in preciosPreferenciales on g.IdProducto equals pref.IdProducto into gj
                    from pref in gj.DefaultIfEmpty()
                    select new PreciosGeneralesLocal
                    {
                        IdProducto = g.IdProducto,
                        Producto = g.Producto,
                        Precio = pref?.Precio ?? g.Precio
                    };

                // Extras: preferenciales que no existen en generales
                var extras =
                    from pref in preciosPreferenciales
                    where !(preciosGenerales ?? Enumerable.Empty<PreciosGeneralesLocal>())
                            .Any(g => g.IdProducto == pref.IdProducto)
                    select new PreciosGeneralesLocal
                    {
                        IdProducto = pref.IdProducto,
                        Producto = pref.Producto,
                        Precio = pref.Precio
                    };

                ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>(merged.Concat(extras).ToList());
            }
            else
            {
                // Fallback a generales si definitivamente no hay preferenciales
                var preciosGenerales = await _localDb.ObtenerPreciosGeneralesAsync();
                ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>(preciosGenerales ?? Enumerable.Empty<PreciosGeneralesLocal>());

                if (preciosGenerales == null || !preciosGenerales.Any())
                    await MostrarAlertaAsync("Sin datos", "No hay precios generales en la base local.");
                else
                    await MostrarAlertaAsync("OK", $"Se cargaron {preciosGenerales.Count} precios generales.");
            }
        }

        public async Task AgregarDetalleVentaAsync(
            PreciosGeneralesLocal producto, 
            int cantidad, 
            decimal importeTotal, 
            Guid idClienteAsociado,
            Guid formaPago,
            Guid condicionPago)
        {
            if (producto == null || producto.IdProducto == Guid.Empty || cantidad <= 0)
                return;

            var ventaGeneral = await _localDb.ObtenerVentaGeneralActiva();
            if (ventaGeneral == null) return;

            // Guardar en VentaDetalleLocal
            var detalleVenta = new VentaDetalleLocal
            {
                IdVentaDetalle = Guid.NewGuid(),
                IdVentaGeneral = ventaGeneral.IdVentaGeneral,
                IdProducto = producto.IdProducto,
                Cantidad = cantidad,
                ImporteTotal = importeTotal,
                IdClienteAsociado = idClienteAsociado,
                IdCondicionPago = condicionPago, // Valor temporal
                IdFormaPago = formaPago,      // Valor temporal
            };
            await _localDb.InsertarVentaDetalleAsync(detalleVenta);

            // Guardar en TicketDetalleLocal
            // Busca el ticket cabecera actual (puedes ajustar la lógica según tu flujo)
            var tickets = await _localDb.ObtenerTicketsAsync();
            var ticketCabecera = tickets
                .Where(t => t.IdCliente == idClienteAsociado)
                .OrderByDescending(t => t.Fecha)
                .FirstOrDefault();

            if (ticketCabecera != null)
            {
                var detalleTicket = new TicketDetalleLocal
                {
                    Id = Guid.NewGuid(),
                    IdTicket = ticketCabecera.Id,
                    IdCliente = ticketCabecera.IdCliente,
                    Cliente = ticketCabecera.Cliente,
                    Empleado = ticketCabecera.Empleado,
                    Fecha = ticketCabecera.Fecha,
                    Descripcion = producto.Producto ?? string.Empty,
                    Cantidad = cantidad,
                    ImporteTotal = importeTotal
                };
                await _localDb.InsertarTicketDetalleAsync(detalleTicket);
            }
            // Si no hay ticket cabecera, puedes crear uno nuevo si tu lógica lo requiere
        }

        public async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
           await CargarFormasPagosLocal();
        }

        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
    }
}
