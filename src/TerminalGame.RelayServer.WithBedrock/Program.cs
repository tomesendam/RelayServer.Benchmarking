﻿using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace TerminalGame.RelayServer.WithBedrock
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Debug);
                        builder.AddConsole();
                    });
                })
                .ConfigureServer(serverBuilder =>
                {
                    serverBuilder.UseSockets(sockets =>
                    {
                        sockets.ListenLocalhost(530, builder => builder.UseConnectionLogging().UseConnectionHandler<MyCustomProtocol>());
                    })
                    .Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                });
    }
}
