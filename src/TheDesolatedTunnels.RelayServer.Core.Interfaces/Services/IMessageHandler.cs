using Microsoft.AspNetCore.Connections;
using TheDesolatedTunnels.RelayServer.Core.Domain;

namespace TheDesolatedTunnels.RelayServer.Core.Interfaces.Services
{
    public interface IMessageHandler
    {
        SixtyNineSendibleMessage? HandleCloseMessage(ConnectionContext connectionContext);
        SixtyNineSendibleMessage HandleInitMessage(InitMessage socketMessage, ConnectionContext connectionContext);
    }
}