using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class ClientWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private ConnectionContext? _connection;

        public ClientWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            var client = new ClientBuilder(_serviceProvider)
                        .UseSockets()
                        .UseConnectionLogging("Client")
                        .Build();

            _connection = await client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530), _hostApplicationLifetime.ApplicationStopping);

            if (_connection == null)
            {
                return;
            }

            var protocol = new MyClientProtocol(_connection);
            await protocol.SendAsync(new InitMessage("1"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload0"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload1"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload2"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload3"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload4"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload5"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload6"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload7"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload8"));
            await protocol.SendAsync(new PayloadMessage("1", "0", "Payload9"));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
