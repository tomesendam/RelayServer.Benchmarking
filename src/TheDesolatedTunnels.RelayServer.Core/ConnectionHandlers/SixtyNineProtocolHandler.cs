// ***********************************************************************
// Assembly         : TheDesolatedTunnels.RelayServer.Core
// Author           : Tom Esendam (ts.esendam@student.han.nl)
// Created          : 12-14-2020
//
// Last Modified By : Tom Esendam (ts.esendam@student.han.nl)
// Last Modified On : 12-15-2020
// ***********************************************************************
// <copyright file="SixtyNineProtocolHandler.cs" company="TheDesolatedTunnels.RelayServer.Core">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Exceptions;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;
using TheDesolatedTunnels.RelayServer.Core.Services;

namespace TheDesolatedTunnels.RelayServer.Core.ConnectionHandlers
{
    /// <summary>
    ///     Class SixtyNineProtocolHandler.
    ///     Implements the <see cref="Microsoft.AspNetCore.Connections.ConnectionHandler" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Connections.ConnectionHandler" />
    /// <autogeneratedoc />
    public class SixtyNineProtocolHandler : ConnectionHandler
    {
        private readonly ConnectionStore _connectionStore;
        private readonly IMessageReader<SixtyNineMessage> _messageReader;
        private readonly ILogger _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly IMessageSender _messageSender;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SixtyNineProtocolHandler" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="messageSender">The message sender.</param>
        /// <param name="connectionStore">The connection store.</param>
        /// <autogeneratedoc />
        public SixtyNineProtocolHandler(ILogger<SixtyNineProtocolHandler> logger, IMessageHandler messageHandler,
            IMessageSender messageSender, ConnectionStore connectionStore,IMessageReader<SixtyNineMessage> messageReader)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _messageSender = messageSender;
            _connectionStore = connectionStore;
            _messageReader = messageReader;
        }

        /// <summary>
        ///     on connected as an asynchronous operation.
        /// </summary>
        /// <param name="connection">The new <see cref="T:Microsoft.AspNetCore.Connections.ConnectionContext" /></param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <autogeneratedoc />
        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var protocol = _messageReader;
            var reader = connection.CreateReader();
            while (!connection.ConnectionClosed.IsCancellationRequested)
                try
                {
                    var result = await reader.ReadAsync(protocol);
                    var message = result.Message;


                    var returnMessage = _messageHandler.HandleMessage(connection, message);

                    if (returnMessage == null) continue;

                    try
                    {
                         await _messageSender.TrySendAsync(returnMessage,
                            connection.ConnectionClosed);
                    }
                    catch (ConnectionNotFoundException e)
                    {
                        _logger.LogError($"connection not found: {e.Message}");
                    }
                    catch (ArgumentNullException e)
                    {
                        _logger.LogError($"Client {connection.ConnectionId} threw: Argument null exception: {e.Message}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Connection {connection.ConnectionId} threw: {e.Message} /n trace: {e.StackTrace}");
                    }


                    if (result.IsCompleted) break;
                }
                catch (ConnectionResetException)
                {
                    var newSource = new CancellationTokenSource();
                    newSource.Cancel();
                    connection.ConnectionClosed = newSource.Token;
                }
                finally
                {
                    reader.Advance();
                }

            _logger.LogInformation($"Disconnected: {connection.ConnectionId}");
            _connectionStore.Remove(connection);
        }
    }
}