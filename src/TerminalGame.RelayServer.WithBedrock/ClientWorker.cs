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
        private Client? _client;
        private ConnectionContext? _connection;

        public ClientWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            _client = new ClientBuilder(_serviceProvider)
                        .UseSockets()
                        .UseConnectionLogging("Client")
                        .Build();

            _connection = await _client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530), _hostApplicationLifetime.ApplicationStopping);

            if (_connection == null)
            {
                return;
            }

            var lines = new string[]
            {
                "{\"payloadType\":\"INIT\",\"source\":\"1\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}",
                "{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}",
            };

            foreach (var line in lines)
            {
                WriteLineHeader(_connection, line);
                WriteLine(_connection, Encoding.UTF8, line);
            }

            await _connection.Transport.Output.FlushAsync(_hostApplicationLifetime.ApplicationStopping);

            static void WriteLineHeader(ConnectionContext connection, string line)
            {
                var sizeSpan = connection.Transport.Output.GetSpan(4);
                BinaryPrimitives.WriteInt32BigEndian(sizeSpan, line.Length);

                connection.Transport.Output.Write(sizeSpan[..4]);
            }


            static void WriteLine(ConnectionContext connection, Encoding encoding, string line)
            {
                var length = encoding.GetByteCount(line);
                var payloadSpan = connection.Transport.Output.GetSpan(length);
                encoding.GetBytes(line, payloadSpan);
                connection.Transport.Output.Write(payloadSpan[..length]);
            }
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
