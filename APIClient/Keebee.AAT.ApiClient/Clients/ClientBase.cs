using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IClientBase
    {
        string Get(string url);
        byte[] GetBytes(string url);
        bool Exists(string url);
        int Post(string url, string value);
        void Patch(string url, string value);
        string Delete(string url);
    }

    public class ClientBase : IClientBase
    {
        private const string UriBase = "http://localhost/Keebee.AAT.Operations/api/";
        private readonly Uri _uriBase;

        public ClientBase()
        {
            _uriBase = new Uri(UriBase);
        }

        public string Get(string url)
        {
            string result = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uriBase;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new Exception(
                            $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                    }
                }
            }

            catch (Exception ex)
            {
                return $"ClientBase.Get: {ex.Message}{Environment.NewLine}url:{url}";
            }

            return result;
        }

        // TODO: Figure out why this doesn't work (returns too many bytes)
        public byte[] GetBytes(string url)
        {
            byte[] result = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = _uriBase;
                client.DefaultRequestHeaders.Accept.Clear();

                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result = response.Content.ReadAsByteArrayAsync().Result;
                }
                else
                {
                    throw new Exception(
                        $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                }
            }

            return result;
        }

        public bool Exists(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _uriBase;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
            }

            return false;
        }

        public int Post(string url, string value)
        {
            using (var client = new HttpClient())
            {
                // added additionally needed back slashes
                value = $"\"{value.Replace("\"", "\\\"")}\"";

                client.BaseAddress = _uriBase;

                HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                }

                int newId;
                var result = response.Content.ReadAsStringAsync().Result;
                var isValid = int.TryParse(result, out newId);

                if (isValid) return newId;
                return -1;
            }
        }

        public void Patch(string url, string value)
        {
            using (var client = new HttpClient())
            {
                // added additionally needed back slashes
                value = $"\"{value.Replace("\"", "\\\"")}\"";

                client.BaseAddress = _uriBase;

                var method = new HttpMethod("PATCH");
                var uri = new Uri($@"{_uriBase}{url}");
                var request = new HttpRequestMessage(method, uri)
                {
                    Content = new StringContent(value, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = client.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                }
            }
        }

        public string Delete(string url)
        {
            var result = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = _uriBase;

                HttpResponseMessage response = client.DeleteAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                }
            }

            return result;
        }
    }
}
