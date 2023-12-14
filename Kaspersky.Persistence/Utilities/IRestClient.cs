using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text;

namespace Kaspersky.Persistence.Utilities
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> PostDataAsync(HttpClient client, Uri endpoint, StringContent content);
        HttpClient CreateHttpClient();
        StringContent CreateJsonContent(object data);
        Task<HttpResponseMessage> GetDataAsync(HttpClient client, Uri endpoint);
    }
    public class RestClient : IRestClient
    {
        public HttpClient CreateHttpClient()
        {
            var client = new HttpClient(GetCertifiHandler());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Cookie", "NSC_TMAS=58741d2bb4e574acbea92392ec94471d");
            return client;
        }

        public StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public async Task<HttpResponseMessage> GetDataAsync(HttpClient client, Uri endpoint)
        {
            return await client.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> PostDataAsync(HttpClient client, Uri endpoint, StringContent content)
        {
            return await client.PostAsync(endpoint, content);
        }
        private HttpClientHandler GetCertifiHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            X509Certificate2 cert = new X509Certificate2("D:\\ProjectVTC\\DocsVTC\\API_KES_Onesme\\New folder\\41a6aabd427d43babb8229bf4d64bb6b50367847de834bfeba7968136641554a.pfx", "@@Vtc123");
            handler.ClientCertificates.Add(cert);
            return handler;
        }
    }

}
