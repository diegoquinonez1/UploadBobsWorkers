using Serialize = Newtonsoft.Json;
using Deserialize = System.Text.Json;
using System.Text;

namespace UploadBobsWorkers.Helpers
{
    public static class ServiceHelper
    {
        private static readonly Deserialize.JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

        public static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                }
            };
            return handler;
        }

        public static HttpClient GetHttpClient(string BaseAdress)
        {
            HttpClientHandler InSecureHandler = GetInsecureHandler();
            HttpClient Client = new(InSecureHandler)
            {
                BaseAddress = new Uri(BaseAdress)
            };
            return Client;
        }

        public static async Task<T?> GetResultGet<T>(HttpResponseMessage Response)
        {
            if (Response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                {
                    string ResultString = await Response.Content.ReadAsStringAsync();
                    return (T)(object)ResultString;
                }
                else
                {
                    Stream Result = await Response.Content.ReadAsStreamAsync();
                    return await Deserialize.JsonSerializer.DeserializeAsync<T?>(Result, Options);
                }
            }
            return default;
        }

        public static T? GetResultGet<T>(string Content)
        {
            if (!string.IsNullOrEmpty(Content))
            {
                return Deserialize.JsonSerializer.Deserialize<T?>(Content, Options);
            }
            return default;
        }

        public static async Task<T?> GetItems<T>(string BaseAdress, string API)
        {
            try
            {
                HttpClient Client = GetHttpClient(BaseAdress);
                HttpResponseMessage Response;
                Response = await Client.GetAsync(API);
                return await GetResultGet<T>(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public static async Task<T?> PostItem<T>(string BaseAdress, string API, object Item)
        {
            try
            {
                string body = Serialize.JsonConvert.SerializeObject(Item);
                StringContent Content = new(body, Encoding.UTF8, "application/json");

                HttpClient Client = GetHttpClient(BaseAdress);
                HttpResponseMessage Response;
                Response = await Client.PostAsync(API, Content);

                return await GetResultGet<T>(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public static async Task<bool> PostItemWithoutResult(string BaseAdress, string API, object Item)
        {
            try
            {
                string body = Serialize.JsonConvert.SerializeObject(Item);
                StringContent Content = new(body, Encoding.UTF8, "application/json");

                HttpClient Client = GetHttpClient(BaseAdress);
                HttpResponseMessage Response;
                Response = await Client.PostAsync(API, Content);
                return Response.EnsureSuccessStatusCode().IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public static async Task<T?> PutItem<T>(string BaseAdress, string API, object Item)
        {
            try
            {
                string body = Serialize.JsonConvert.SerializeObject(Item);
                StringContent Content = new(body, Encoding.UTF8, "application/json");

                HttpClient Client = GetHttpClient(BaseAdress);
                HttpResponseMessage Response;
                Response = await Client.PutAsync(API, Content);
                return await GetResultGet<T>(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public static async Task<bool> PutItemWithoutResult(string BaseAdress, string API, object Item)
        {
            try
            {
                string body = Serialize.JsonConvert.SerializeObject(Item);
                StringContent Content = new(body, Encoding.UTF8, "application/json");

                HttpClient Client = GetHttpClient(BaseAdress);
                HttpResponseMessage Response;
                Response = await Client.PutAsync(API, Content);
                return Response.EnsureSuccessStatusCode().IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }
    }
}
