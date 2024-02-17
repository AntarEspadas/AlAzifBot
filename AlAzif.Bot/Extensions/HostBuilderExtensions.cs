using AlAzif.Bot.Configuration;
using DSharpPlus;
using Lavalink4NET.Extensions;
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
    public static T AddAlAzif<T>(this T builder) where T : IHostApplicationBuilder
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        services
            .AddOptions<AlAzifConfig>()
            .Bind(configuration.GetSection("AlAzif"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<QueuedLavalinkPlayerOptions>()
            .Bind(configuration.GetSection("Lavalink"))
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
            .AddLavalink()
            .AddInactivityTracking()
            .AddSingleton<DiscordClient>()
            .AddHostedService<AlAzifBot>();
                        
        services.AddOptions<InactivityTrackingOptions>()
            .Bind(configuration.GetSection("Lavalink:InactivityTracking"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return builder;
    }
}