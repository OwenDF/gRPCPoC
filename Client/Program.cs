using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Payments;

namespace Client
{
    public class Program
    {
        static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var call = new Payer.PayerClient(channel).Pay(new PayRequest {Reference = "bla"});

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(response.Status);
            }
        }
    }
}
