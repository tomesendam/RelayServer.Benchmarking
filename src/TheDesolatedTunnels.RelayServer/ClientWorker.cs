// using System;
// using System.Net;
// using System.Threading;
// using System.Threading.Tasks;
// using Bedrock.Framework;
// using Microsoft.AspNetCore.Connections;
// using Microsoft.Extensions.Hosting;
// using TheDesolatedTunnels.RelayServer.Core.Domain;
//
// namespace TheDesolatedTunnels.RelayServer
// {
//     public class ClientWorker : IHostedService
//     {
//         private readonly IHostApplicationLifetime _hostApplicationLifetime;
//         private readonly IServiceProvider _serviceProvider;
//         private ConnectionContext? _connection;
//
//         public ClientWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
//         {
//             _serviceProvider = serviceProvider;
//             _hostApplicationLifetime = hostApplicationLifetime;
//         }
//
//         public async Task StartAsync(CancellationToken cancellationToken)
//         {
//             await Task.Yield();
//
//             var client = new ClientBuilder(_serviceProvider)
//                 .UseSockets()
//                 .UseConnectionLogging("Client")
//                 .Build();
//
//             _connection = await client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530),
//                 _hostApplicationLifetime.ApplicationStopping);
//
//             if (_connection == null) return;
//
//             var protocol = new SixtyNineClientProtocol(_connection);
//             while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
//             {
//                 await protocol.SendAsync(new InitMessage("1"), _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload0"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload1"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload2"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload3"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload4"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload5"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload6"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload7"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload8"),
//                     _hostApplicationLifetime.ApplicationStopping);
//                 await protocol.SendAsync(new PayloadMessage("1", "0", "Payload9"),
//                     _hostApplicationLifetime.ApplicationStopping);
//
//                 Thread.Sleep(100);
//             }
//         }
//
//         public async Task StopAsync(CancellationToken cancellationToken)
//         {
//             if (_connection != null) await _connection.DisposeAsync();
//         }
//     }
// }