namespace Balena.Cloud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Models;

    public class BalenaCloudConnection
    {
        private readonly HttpClient _client;

        public string ApiUrl { get; } 
        public string ApiToken { get; }

        public BalenaCloudConnection(string url, string apiToken)
        {
            this._client = CreateClient(url, apiToken);
            ApiUrl = url;
            ApiToken = apiToken;
        }

        private HttpClient CreateClient(string url, string apiToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            
            // base address must end in /, see https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            if (url.Last() != '/')
            {
                url = url + '/';
            }
            client.BaseAddress = new Uri(url);
            return client;
        }

        public class BalenaResponse<T>
        {
            public T d { get; set; }
        }

        public async Task<List<Application>> GetAllApplications()
        {
            var balenaResponse = await _client.ExecuteAsync<BalenaResponse<List<Application>>>("application");
            return balenaResponse.d;
        }

        public async Task<Application> GetApplicationById(int id)
        {
            var balenaResponse = await _client.ExecuteAsync<BalenaResponse<List<Application>>>("application");
            return balenaResponse.d[0];
        }
    }
}