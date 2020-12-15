using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Connections;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    public class ConnectionStore
    {
        private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new(StringComparer.Ordinal);

        public ConnectionContext? this[string connectionId]
        {
            get
            {
                _connections.TryGetValue(connectionId, out var connection);
                return connection;
            }
        }

        public bool Contains(string connectionId)
        {
            return _connections.ContainsKey(connectionId);
        }

        public int Count()
        {
            return _connections.Count;
        }

        public void Add(ConnectionContext connection)
        {
            _connections.TryAdd(connection.ConnectionId, connection);
        }

        public void Remove(ConnectionContext connection)
        {
            _connections.TryRemove(connection.ConnectionId, out _);
        }

        public Enumerator GetEnumerator()
        {
            return new(this);
        }


        public readonly struct Enumerator : IEnumerator<ConnectionContext>
        {
            private readonly IEnumerator<KeyValuePair<string, ConnectionContext>> _enumerator;

            public Enumerator(ConnectionStore hubConnectionList)
            {
                _enumerator = hubConnectionList._connections.GetEnumerator();
            }

            public ConnectionContext Current => _enumerator.Current.Value;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }
    }
}