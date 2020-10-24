using System;
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
            PayRequest request,
            IServerStreamWriter<PayResponse> responseWriter,
            ServerCallContext context)
        {
            await Delay(FromSeconds(1));
            await responseWriter.WriteAsync(new PayResponse {Status = FirstAccept});
            await Delay(FromSeconds(2));
            await responseWriter.WriteAsync(new PayResponse {Status = SecondAccept});
            await Delay(FromSeconds(2));
            await responseWriter.WriteAsync(new PayResponse {Status = Made});
        }
    }
}
