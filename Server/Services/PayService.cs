using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using Payments;

namespace Server.Services
{
    using static PayResponse.Types.PayStatus;
    using static Task;
    using static TimeSpan;
    
    public class PayService : Payer.PayerBase
    {
        public override async Task Pay(
            IAsyncStreamReader<PayRequest> requests,
            IServerStreamWriter<PayResponse> responseWriter,
            ServerCallContext context)
        {
            var runningResponses = new ConcurrentDictionary<Guid, Task>();

            await foreach (var request in requests.ReadAllAsync())
            {
                var guid = Guid.NewGuid();
                var run = Run(async () =>
                {
                    await DoResponseCycle(responseWriter, request.Reference);
                    runningResponses.TryRemove(guid, out var _);
                });
                runningResponses.TryAdd(guid, run);
            }

            await WhenAll(runningResponses.Values);
        }

        private static async Task DoResponseCycle(
            IServerStreamWriter<PayResponse> responseWriter,
            string reference)
        {
            await Delay(FromSeconds(1));
            await responseWriter.WriteAsync(new PayResponse {Reference = reference, Status = FirstAccept});
            await Delay(FromSeconds(2));
            await responseWriter.WriteAsync(new PayResponse {Reference = reference, Status = SecondAccept});
            await Delay(FromSeconds(2));
            await responseWriter.WriteAsync(new PayResponse {Reference = reference, Status = Made});
        }
    }
}
