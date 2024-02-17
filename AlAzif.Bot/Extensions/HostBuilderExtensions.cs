using AlAzif.Bot.Configuration;
using DSharpPlus;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlAzif.Bot.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseAlAzif(this IHostBuilder builder)
    {
        builder
            .ConfigureServices((ctx, services) =>
            {
                services
                    .AddOptions<AlAzifConfig>()
                    .Bind(ctx.Configuration.GetSection("AlAzif"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
                
                services.AddOptions<QueuedLavalinkPlayerOptions>()
                    .Bind(ctx.Configuration.GetSection("Lavalink"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                services.Scan(scan => scan
                    .FromCallingAssembly()
                    .AddClasses(classes => classes.InNamespaces("AlAzif.Bot.Services"))
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                );
                
                services
                    .AddSingleton<DiscordConfiguration>(provider => new()
                    {
                        Token = provider.GetService<IOptions<AlAzifConfig>>()?.Value.Token,
                        TokenType = TokenType.Bot,
                        Intents = DiscordIntents.All,
                        LoggerFactory = provider.GetService<ILoggerFactory>()
                    })
                    .AddInactivityTracking()
                    .AddSingleton<DiscordClient>()
                    .AddHostedService<AlAzifBot>();
                                
                services.AddOptions<InactivityTrackingOptions>()
                    .Bind(ctx.Configuration.GetSection("Lavalink:InactivityTracking"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            });

        return builder;
    }
}