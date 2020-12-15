using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Services;

namespace TheDesolatedTunnels.RelayServer
{
    //This class only exists for testing purpose
    public class SixtyNineClientProtocol
    {
        private readonly ConnectionContext _connection;
        private readonly SixtyNineWriter _messageWriter;
        private readonly ProtocolReader _reader;

        public SixtyNineClientProtocol(ConnectionContext connection)
        {
            _connection = connection;
            _reader = connection.CreateReader();

            _messageWriter = new SixtyNineWriter();
        }

        public async ValueTask SendAsync(SixtyNineSendibleMessage requestMessage, CancellationToken cancellationToken)
        {
            // Write request message length
            _messageWriter.WriteMessage(requestMessage, _connection.Transport.Output);

            await _connection.Transport.Output.FlushAsync(cancellationToken);
        }
    }
}