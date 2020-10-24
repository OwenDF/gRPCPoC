using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Server;

namespace Client
{
    public class Program
    {
        static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");

            Console.WriteLine(
                (await new Greeter.GreeterClient(channel)
                    .SayHelloAsync(new HelloRequest {Name = "Owen"}))
                .Message);
        }
    }
}
