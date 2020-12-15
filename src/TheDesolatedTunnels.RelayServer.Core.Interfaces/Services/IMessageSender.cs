using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using TheDesolatedTunnels.RelayServer.Core.Domain;

namespace TheDesolatedTunnels.RelayServer.Core.Interfaces.Services
{
    public interface IMessageSender
    {
        ValueTask<FlushResult> TrySendAsync(SixtyNineSendibleMessage requestMessage, CancellationToken cancellationToken = default);
    }
}