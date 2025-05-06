using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class HttpClientBase : HttpClient
    {
        #region Constructores

        public HttpClientBase()
            : base(new LoggingHandler
            {
                InnerHandler = new HttpClientHandler
                {
                    // Bypass de validación SSL (solo para desarrollo)
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            })
        {
            this.urlController = string.Empty;
            this.UrlController = string.Empty;
            this.BaseAddress = new Uri(UrlBaseWebApi);
            this.InitHeaders();
        }

        public HttpClientBase(string urlController)
            : base(new LoggingHandler
            {
                InnerHandler = new HttpClientHandler
                {
                    // Bypass de validación SSL (solo para desarrollo)
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            })
        {
            this.urlController = string.Empty;
            this.UrlController = urlController;
            this.InitHeaders();
        }

        #endregion

        #region Propiedades        

        public string UrlBaseWebApi { get; set; } = Helpers.Settings.UrlBaseAPI;

        private string urlController;
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

        private void InitHeaders()
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        private (HttpStatusCode StatusCode, TResponse Content) ProcessResponse<TResponse>(HttpResponseMessage res)
        {
            var raw = res.Content.ReadAsStringAsync().Result;
            try
            {
                var payload = JsonConvert.DeserializeObject<TResponse>(raw);
                return (res.StatusCode, payload);
            }
            catch
            {
                return (res.StatusCode, default);
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)>
            CallGetAsync<TResponse>(string url)
        {
            try
            {
                var res = await GetAsync(url);
                return ProcessResponse<TResponse>(res);
            }
            catch
            {
                return (HttpStatusCode.BadRequest, default);
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)>
            CallPostAsync<TRequest, TResponse>(string url, TRequest req)
        {
            try
            {
                var json = JsonConvert.SerializeObject(req);
                var res = await PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                return ProcessResponse<TResponse>(res);
            }
            catch
            {
                return (HttpStatusCode.BadRequest, default);
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)>
            CallPutAsync<TRequest, TResponse>(string url, TRequest req)
        {
            try
            {
                var json = JsonConvert.SerializeObject(req);
                var res = await PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                return ProcessResponse<TResponse>(res);
            }
            catch
            {
                return (HttpStatusCode.BadRequest, default);
            }
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)>
            CallDeleteAsync<TResponse>(string url)
        {
            try
            {
                var res = await DeleteAsync(url);
                return ProcessResponse<TResponse>(res);
            }
            catch
            {
                return (HttpStatusCode.BadRequest, default);
            }
        }

        #endregion                
    }

    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage>
            SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var reqBody = request.Content == null
                ? ""
                : await request.Content.ReadAsStringAsync();
            Debug.WriteLine($"--> {request.Method} {request.RequestUri}\n{reqBody}");

            var response = await base.SendAsync(request, ct);

            var resBody = response.Content == null
                ? ""
                : await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"<-- {(int)response.StatusCode} {response.ReasonPhrase}\n{resBody}");

            return response;
        }
    }
}
