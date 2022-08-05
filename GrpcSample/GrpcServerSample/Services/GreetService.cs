using Greet.V1;
using Grpc.Core;
using System.Threading.Tasks;

namespace GrpcServerSample.Services
{
    public class GreetService : Greeter.GreeterBase
    {
        private readonly TestModel _testModel;

        public GreetService(TestModel testModel)
        {
            this._testModel = testModel;
        }
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply()
            {
                Message = $"Hello from {request.Name} [{_testModel.Name}]"
            });
        }

        public override async Task<HelloReply> SayHelloFrom(HelloRequestFrom request, ServerCallContext context)
        {
            return new HelloReply()
            {
                Message = $"[{request.From}] - {request.Name}"
            };
        }
    }
}
