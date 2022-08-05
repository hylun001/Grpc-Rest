using Greet.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace GrpcClientSample
{
    internal class GrpcServiceTest
    {
        public static async Task MainTest()
        {

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
            // http channel
            await InvokeHelper.TryInvokeAsync(HttpTest);

            // https
            await InvokeHelper.TryInvokeAsync(HttpsTest);

            // GrpcClientFactory
            await InvokeHelper.TryInvokeAsync(GrpcClientFactoryTest);
        }

        private static async Task HttpTest()
        {
            var httpChannel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions()
            {
                HttpClient = null,
                HttpHandler = new HttpClientHandler
                {
                    //方法一
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    //方法二
                    //ServerCertificateCustomValidationCallback = (a, b, c, d) => true
                },
            });
            var client = new Greeter.GreeterClient(httpChannel);
            var reply = await client.SayHelloAsync(new HelloRequest()
            {
                Name = "Http test"
            });
            Console.WriteLine($"[GRPC]{reply.Message}");
        }

        private static async Task GrpcClientFactoryTest()
        {
            var services = new ServiceCollection();

            services.AddGrpcClient<Greeter.GreeterClient>("greeter", (s, o) =>
            {
                o.Address = new Uri("https://localhost:5001");

            }).ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var handle = new HttpClientHandler();
                handle.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
                return handle;
            }); ;
            await using var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<Greeter.GreeterClient>();

            var reply = await client.SayHelloAsync(new HelloRequest()
            {
                Name = "GrpcClientFactory test"
            });
            Console.WriteLine($"[GRPC]{reply.Message}");
        }

        private static async Task HttpsTest()
        {
            var httpsChannel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions()
            {
                HttpClient = null,
                HttpHandler = new HttpClientHandler
                {
                    //方法一
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    //方法二
                    //ServerCertificateCustomValidationCallback = (a, b, c, d) => true
                },
            });
            var client = new Greeter.GreeterClient(httpsChannel);
            var reply = await client.SayHelloFromAsync(new HelloRequestFrom() { From = "China", Name = "Hylun" });
            Console.WriteLine($"[GRPC]{reply.Message}");
        }
    }
}
