using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheDesolatedTunnels.RelayServer.Core.ConnectionHandlers;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;
using TheDesolatedTunnels.RelayServer.Core.Services;

namespace TheDesolatedTunnels.RelayServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ConnectionStore>();
                    services.AddScoped<IMessageWriter<SixtyNineSendibleMessage>, SixtyNineWriter>();
                    services.AddScoped<IMessageHandler, MessageHandler>();
                    services.AddScoped<IMessageSender, MessageSender>();
                })
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    loggingBuilder.SetMinimumLevel(LogLevel.Error); 
                })
                .ConfigureServer(serverBuilder =>
                {
                    serverBuilder.UseSockets(sockets =>
                        {
                            sockets.ListenLocalhost(530,
                                builder => builder.UseConnectionLogging()
                                    .UseConnectionHandler<SixtyNineProtocolHandler>());
                        })
                        .Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<ClientWorker>();
                });
        }
    }
}