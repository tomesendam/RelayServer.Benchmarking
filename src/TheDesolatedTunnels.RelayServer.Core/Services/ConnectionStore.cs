// ***********************************************************************
// Assembly         : TheDesolatedTunnels.RelayServer.Core
// Author           : Tom Esendam (ts.esendam@student.han.nl)
// Created          : 12-14-2020
//
// Last Modified By : Tom Esendam (ts.esendam@student.han.nl)
// Last Modified On : 12-15-2020
// ***********************************************************************
// <copyright file="ConnectionStore.cs" company="TheDesolatedTunnels.RelayServer.Core">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Connections;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    /// <summary>
    ///     Class ConnectionStore.
    /// </summary>
    /// <autogeneratedoc />
    public class ConnectionStore
    {
        /// <summary>
        ///     The connections
        /// </summary>
        /// <autogeneratedoc />
        private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new(StringComparer.Ordinal);

        /// <summary>
        ///     Gets the <see cref="Microsoft.AspNetCore.Connections.ConnectionContext?" /> with the specified connection
        ///     identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns>Microsoft.AspNetCore.Connections.ConnectionContext?.</returns>
        /// <autogeneratedoc />
        public virtual ConnectionContext? this[string connectionId]
        {
            get
            {
                _connections.TryGetValue(connectionId, out var connection);
                return connection;
            }
        }

        /// <summary>
        ///     Determines whether this instance contains the object.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns>bool.</returns>
        /// <autogeneratedoc />
        public virtual bool Contains(string connectionId)
        {
            return _connections.ContainsKey(connectionId);
        }

        /// <summary>
        ///     Counts this instance.
        /// </summary>
        /// <returns>int.</returns>
        /// <autogeneratedoc />
        public int Count()
        {
            return _connections.Count;
        }

        /// <summary>
        ///     Adds the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <autogeneratedoc />
        public virtual void Add(ConnectionContext connection)
        {
            _connections.TryAdd(connection.ConnectionId, connection);
        }

        /// <summary>
        ///     Removes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <autogeneratedoc />
        public virtual void Remove(ConnectionContext connection)
        {
            _connections.TryRemove(connection.ConnectionId, out _);
        }

        // TODO: Decide if code should be deleted. As it is unused. 
        // /// <summary>
        // /// Gets the enumerator.
        // /// </summary>
        // /// <returns>TheDesolatedTunnels.RelayServer.Core.Services.ConnectionStore.Enumerator.</returns>
        // /// <autogeneratedoc />
        // public Enumerator GetEnumerator()
        // {
        //     return new(this);
        // }
        //
        //
        // /// <summary>
        // /// Struct Enumerator
        // /// Implements the <see cref="System.Collections.Generic.IEnumerator`1" />
        // /// </summary>
        // /// <seealso cref="System.Collections.Generic.IEnumerator`1" />
        // /// <autogeneratedoc />
        // public readonly struct Enumerator : IEnumerator<ConnectionContext>
        // {
        //     /// <summary>
        //     /// The enumerator
        //     /// </summary>
        //     /// <autogeneratedoc />
        //     private readonly IEnumerator<KeyValuePair<string, ConnectionContext>> _enumerator;
        //
        //     /// <summary>
        //     /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        //     /// </summary>
        //     /// <param name="connectionStore">The connectionStore.</param>
        //     /// <autogeneratedoc />
        //     public Enumerator(ConnectionStore connectionStore)
        //     {
        //         _enumerator = connectionStore._connections.GetEnumerator();
        //     }
        //
        //     /// <summary>
        //     /// Gets the current.
        //     /// </summary>
        //     /// <value>The current.</value>
        //     /// <autogeneratedoc />
        //     public ConnectionContext Current => _enumerator.Current.Value;
        //
        //     /// <summary>
        //     /// Sets the current.
        //     /// </summary>
        //     /// <value>The current.</value>
        //     /// <autogeneratedoc />
        //     object IEnumerator.Current => Current;
        //
        //     /// <summary>
        //     /// Disposes this instance.
        //     /// </summary>
        //     /// <autogeneratedoc />
        //     public void Dispose()
        //     {
        //         _enumerator.Dispose();
        //     }
        //
        //     /// <summary>
        //     /// Moves the next.
        //     /// </summary>
        //     /// <returns>bool.</returns>
        //     /// <autogeneratedoc />
        //     public bool MoveNext()
        //     {
        //         return _enumerator.MoveNext();
        //     }
        //
        //     /// <summary>
        //     /// Resets this instance.
        //     /// </summary>
        //     /// <autogeneratedoc />
        //     public void Reset()
        //     {
        //         _enumerator.Reset();
        //     }
        // }
    }
}