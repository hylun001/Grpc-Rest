using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace GrpcClientSample
{
    internal class HttpServiceTest
    {
        public static async Task MainTest()
        {
            var httpClientHandler = new HttpClientHandler

            {

                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true

            }; 
            using var client = new HttpClient(httpClientHandler)
            {
                DefaultRequestVersion = HttpVersion.Version20,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher,

            };
            await InvokeHelper.TryInvokeAsync(async () =>
            {
                var responseText = await client.GetStringAsync("https://localhost:5001/v1/greeter/test");
                Console.WriteLine($"[Http] Response from https endpoint: {responseText}");
            });
            await InvokeHelper.TryInvokeAsync(async () =>
            {
                string aaa = "{\"name\":\"Hylun\",\"from\":\"China\"}";
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(new { name = "Hylun", from = "China" });
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage responseText = await client.PostAsync("http://localhost:5000/v1/greeter", httpContent);
                string res=await responseText.Content.ReadAsStringAsync();
                Console.WriteLine($"[Http] Response from http endpoint: {res}");
            });

            //
            await InvokeHelper.TryInvokeAsync(async () =>
            {
                var responseText = await client.GetStringAsync("http://localhost:5000/v1/todo");
                Console.WriteLine($"[Http] Response from todo endpoint: {responseText}");
            });
        }
    }
}
