using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class HttpClientBase : HttpClient
    {
        #region Constructores
        public HttpClientBase()
        {
            this.urlController = string.Empty;
            this.UrlController = string.Empty;
            this.BaseAddress = new Uri(UrlBaseWebApi);
            this.InitHeaders();
        }

        public HttpClientBase(string urlController)
        {
            this.urlController = string.Empty;
            this.UrlController = urlController;
            this.InitHeaders();
        }
        #endregion

        #region Propiedades        
        public string UrlBaseWebApi { get; set; } = Helpers.Settings.UrlBaseAPI;

        string urlController;
        protected string UrlController
        {
            get => urlController;
            set
            {
                urlController = !value.EndsWith("/") ? value + "/" : value;
                UrlBaseWebApi += (!UrlBaseWebApi.EndsWith("/") ? "/" : string.Empty) + value;
                BaseAddress = new Uri(UrlBaseWebApi);
            }
        }
        #endregion

        #region Procesamiento
        void InitHeaders()
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
        }

        private (HttpStatusCode StatusCode, TResponse Content) ProcessResponse<TResponse>(HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode)
            {
                var s = res.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<TResponse>(s);
                return (res.StatusCode, response);
            }
            else
            {
                try
                {
                    //Para peticiones Bad request, el body contiene una instancia de error, por lo que se trata de obtener
                    var s = res.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<TResponse>(s);
                    return (res.StatusCode, response);
                }
                catch (Exception ex)
                {
                    var mensaje = ex.Message;
                    return (res.StatusCode, default(TResponse));
                }
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallGetAsync<TResponse>(string url)
        {
            try
            {
                var res = await GetAsync(url);
                return ProcessResponse<TResponse>(res);
            }
            catch (Exception ex)
            {
                return (HttpStatusCode.BadRequest, default(TResponse));
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallPostAsync<TRequest, TResponse>(string url, TRequest req)
        {
            try
            {
                var x = JsonConvert.SerializeObject(req);
                var res = await PostAsync(url, new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json"));
                return ProcessResponse<TResponse>(res);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                return (HttpStatusCode.BadRequest, default(TResponse));
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallPutAsync<TRequest, TResponse>(string url, TRequest req)
        {
            try
            {
                var res = await PutAsync(url, new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json"));
                return ProcessResponse<TResponse>(res);
            }
            catch
            {
                return (HttpStatusCode.BadRequest, default(TResponse));
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallDeleteAsync<TResponse>(string url)
        {
            try
            {
                var res = await DeleteAsync(url);
                return ProcessResponse<TResponse>(res);
            }
            catch
            {
                return (HttpStatusCode.BadRequest, default(TResponse));
            }
        }
        #endregion                
    }
}
