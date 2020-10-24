using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Payments;

namespace Client
{
    using static Task;
    
    public class Program
    {
        static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Payer.PayerClient(channel);
            var call = client.Pay();
            await WhenAll(MakeCalls(call), ReadResponses(call));
        }

        private static async Task MakeCalls(AsyncDuplexStreamingCall<PayRequest, PayResponse> call)
        {
            for (var i = 0; i < 100; i++)
            {
                await call.RequestStream.WriteAsync(new PayRequest {Reference = $"Payment{i}"});
                await Delay(500);
            }

            await call.RequestStream.CompleteAsync();
        }

        private static async Task ReadResponses(AsyncDuplexStreamingCall<PayRequest, PayResponse> call)
        {
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{response.Reference}: {response.Status}");
            }
        }
    }
}
