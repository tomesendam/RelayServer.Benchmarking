// ***********************************************************************
// Assembly         : TheDesolatedTunnels.RelayServer.Core
// Author           : Tom Esendam (ts.esendam@student.han.nl)
// Created          : 12-15-2020
//
// Last Modified By : Tom Esendam (ts.esendam@student.han.nl)
// Last Modified On : 12-15-2020
// ***********************************************************************
// <copyright file="MessageSender.cs" company="TheDesolatedTunnels.RelayServer.Core">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Exceptions;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    /// <summary>
    ///     Class MessageSender.
    ///     Implements the <see cref="TheDesolatedTunnels.RelayServer.Core.Interfaces.Services.IMessageSender" />
    /// </summary>
    /// <seealso cref="TheDesolatedTunnels.RelayServer.Core.Interfaces.Services.IMessageSender" />
    /// <autogeneratedoc />
    public class MessageSender : IMessageSender
    {
        /// <summary>
        ///     The connection store
        /// </summary>
        /// <autogeneratedoc />
        private readonly ConnectionStore _connectionStore;

        /// <summary>
        ///     The message writer
        /// </summary>
        /// <autogeneratedoc />
        private readonly IMessageWriter<SixtyNineSendibleMessage> _messageWriter;

        private SemaphoreSlim _semaphore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageSender" /> class.
        /// </summary>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="connectionStore">The connection store.</param>
        /// <autogeneratedoc />
        public MessageSender(IMessageWriter<SixtyNineSendibleMessage> messageWriter, ConnectionStore connectionStore)
        {
            _messageWriter = messageWriter;
            _connectionStore = connectionStore;
            _semaphore = new SemaphoreSlim(1);
        }

        /// <summary>
        ///     try send as an asynchronous operation.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="cancellationToken">
        ///     The cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>A Task&lt;FlushResult&gt; representing the asynchronous operation.</returns>
        /// <exception cref="TheDesolatedTunnels.RelayServer.Core.Exceptions.ConnectionNotFoundException"></exception>
        /// <autogeneratedoc />
        public async ValueTask TrySendAsync(SixtyNineSendibleMessage requestMessage,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var connection = _connectionStore[requestMessage.Destination] ??
                                 throw new ConnectionNotFoundException(requestMessage.Destination);
                // Write request message length
                //_messageWriter.WriteMessage(requestMessage, connection.Transport.Output);
                var ff = new ProtocolWriter(connection.Transport.Output);
                await ff.WriteAsync(_messageWriter,requestMessage,cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
           
            //return await connection.Transport.Output.FlushAsync(cancellationToken);
        }
    }
}