using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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
