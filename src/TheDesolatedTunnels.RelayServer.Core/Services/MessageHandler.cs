using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ConnectionStore _connectionStore;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(ConnectionStore connectionStore, ILogger<MessageHandler> logger)
        {
            _connectionStore = connectionStore;
            _logger = logger;
        }

        public SixtyNineSendibleMessage? HandleCloseMessage(ConnectionContext connectionContext)
        {
            _connectionStore.Remove(connectionContext);
            return null;
        }

        public SixtyNineSendibleMessage HandleInitMessage(InitMessage socketMessage, ConnectionContext connectionContext)
        {
            var (source, _) = socketMessage;
            if (source != null)
            {
                if (!_connectionStore.Contains(source))
                {
                    _logger.LogInformation($"Client with id: {source} connected");
                    connectionContext.ConnectionId = source;
                    _connectionStore.Add(connectionContext);
                }
                else
                {
                    return new ErrorMessage(connectionContext.ConnectionId,
                        $"ClientId: {connectionContext.ConnectionId} is already initialized");
                }
            }
            else
            {
                _connectionStore.Add(connectionContext);
            }

            return new InitResponseMessage(connectionContext.ConnectionId);
        }
    }
}