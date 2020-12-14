using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using System.Threading;
using System.Threading.Tasks;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyClientProtocol
    {
        private readonly ConnectionContext _connection;
        private readonly ProtocolReader _reader;
        private readonly MyRequestMessageWriter _messageWriter;

        public MyClientProtocol(ConnectionContext connection)
        {
            _connection = connection;
            _reader = connection.CreateReader();

            _messageWriter = new MyRequestMessageWriter();
        }

        public async ValueTask SendAsync(MyRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            // Write request message length
            _messageWriter.WriteMessage(requestMessage, _connection.Transport.Output);

            await _connection.Transport.Output.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
