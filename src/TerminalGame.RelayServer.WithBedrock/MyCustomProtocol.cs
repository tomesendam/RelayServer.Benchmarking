using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyCustomProtocol : ConnectionHandler
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public MyCustomProtocol(ILogger<MyCustomProtocol> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            // Use a length prefixed protocol
            var protocol = new MyRequestMessageReader();
            var reader = connection.CreateReader();

            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
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
