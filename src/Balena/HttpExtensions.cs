namespace Balena
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal static class HttpExtensions
    {
        public static async Task<T> ExecuteAsync<T>(this HttpClient client, string request)
        {
            using (HttpResponseMessage res = await client.GetAsync(request))
            {
                res.ThrowIfNotSucess(new Uri(client.BaseAddress + request));
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(data);
                }
            }
        }

        public static async Task ExecuteAsync(this HttpClient client, string request)
        {
            using (HttpResponseMessage res = await client.GetAsync(request))
            {
                res.ThrowIfNotSucess(new Uri(client.BaseAddress + request));
            }
        }

        public static void ThrowIfNotSucess(this HttpResponseMessage response, Uri request)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw RestException.CreateException(request, response);
            }
        }
    }

    public class RestException : Exception
    {
        public RestException(HttpStatusCode httpStatusCode, Uri requestUri, string content, string message, Exception innerException)
          : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            RequestUri = requestUri;
            Content = content;
        }

        public HttpStatusCode HttpStatusCode { get; private set; }

        public Uri RequestUri { get; private set; }

        public string Content { get; private set; }

        public static RestException CreateException(Uri requestUri, HttpResponseMessage response)
        {
            return new RestException(response.StatusCode, requestUri, null, response.ReasonPhrase, null);
        }
    }
}
