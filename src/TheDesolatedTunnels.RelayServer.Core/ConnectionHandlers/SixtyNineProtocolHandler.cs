using System;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Exceptions;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;
using TheDesolatedTunnels.RelayServer.Core.Services;

namespace TheDesolatedTunnels.RelayServer.Core.ConnectionHandlers
{
    public class SixtyNineProtocolHandler : ConnectionHandler
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IMessageHandler _messageHandler;
        private readonly IMessageSender _messageSender;
        private readonly ConnectionStore _connectionStore;
        private readonly ILogger _logger;

        public SixtyNineProtocolHandler(ILogger<SixtyNineProtocolHandler> logger,
            IHostApplicationLifetime hostApplicationLifetime, IMessageHandler messageHandler,IMessageSender messageSender,ConnectionStore connectionStore)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _messageHandler = messageHandler;
            _messageSender = messageSender;
            _connectionStore = connectionStore;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var protocol = new SixtyNineReader();
            var reader = connection.CreateReader();



            while (!connection.ConnectionClosed.IsCancellationRequested)
            {
                try
                { 
                    var result = await reader.ReadAsync(protocol);
                    var message = result.Message;

                    
                    var returnMessage = message switch
                    {
                        null => null,
                        InitMessage x => _messageHandler.HandleInitMessage(x, connection),
                        CloseMessage => _messageHandler.HandleCloseMessage(connection),
                        ErrorMessage x => x,
                        PayloadMessage x => x,
                        _ => new ErrorMessage(connection.ConnectionId, "Invalid type")
                    };

                    if (returnMessage == null)
                    {
                        continue;
                    }

                    try
                    {
                         await _messageSender.TrySendAsync(returnMessage,
                             connection.ConnectionClosed);
                    }
                    catch (ConnectionNotFoundException e)
                    {
                        _logger.LogError($"kek: {e.Message}" );
                    }

                    if (result.IsCompleted) break;

                }
                finally
                {
                    reader.Advance();
                }
            }
            
            _logger.LogInformation($"Disconnected: {connection.ConnectionId}");
            _connectionStore.Remove(connection);
        }

        
    }
}