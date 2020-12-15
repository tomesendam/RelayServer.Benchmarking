using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Exceptions;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    public class MessageSender : IMessageSender
    {
        private readonly ConnectionStore _connectionStore;
        private readonly IMessageWriter<SixtyNineSendibleMessage> _messageWriter;

        public MessageSender(IMessageWriter<SixtyNineSendibleMessage> messageWriter, ConnectionStore connectionStore)
        {
            _messageWriter = messageWriter;
            _connectionStore = connectionStore;
        }

        public async ValueTask<FlushResult> TrySendAsync(SixtyNineSendibleMessage requestMessage, CancellationToken cancellationToken = default)
        {
            var connection = _connectionStore[requestMessage.Destination] ?? throw new ConnectionNotFoundException(requestMessage.Destination);
            // Write request message length
            _messageWriter.WriteMessage(requestMessage, connection.Transport.Output);

            return await connection.Transport.Output.FlushAsync(cancellationToken);
        }
    }
}