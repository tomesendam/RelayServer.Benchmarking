using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyCustomProtocol : ConnectionHandler
    {
        private readonly ILogger _logger;

        public MyCustomProtocol(ILogger<MyCustomProtocol> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            // Use a length prefixed protocol
            var protocol = new MyRequestMessageReader();
            var reader = connection.CreateReader();

            while (true)
            {
                try
                {
                    var result = await reader.ReadAsync(protocol);
                    var message = result.Message;

                    if(message is PayloadMessage payloadMessage )
                    _logger.LogInformation("Received a message of {Length} bytes", payloadMessage.Payload.Length);

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    reader.Advance();
                }
            }
        }
    }
}
