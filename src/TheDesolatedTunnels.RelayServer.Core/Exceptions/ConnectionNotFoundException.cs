using System;

namespace TheDesolatedTunnels.RelayServer.Core.Exceptions
{
    internal class ConnectionNotFoundException : Exception
    {
        public ConnectionNotFoundException(string connectionId) : base(connectionId) {}
    }
}