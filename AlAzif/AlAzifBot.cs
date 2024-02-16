using AlAzif.Configuration;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;
using AlAzif.Exceptions;
using DSharpPlus.Entities;
using Lavalink4NET.InactivityTracking;

namespace AlAzif;

public class AlAzifBot : IHostedService
{

    private readonly ILogger<AlAzifBot> _logger;

    private readonly DiscordClient _client;

    public AlAzifBot(
        DiscordClient client,
        ILogger<AlAzifBot> logger,
        IOptions<AlAzifConfig> config,
        IServiceProvider services,
        IHostEnvironment env,
        IInactivityTrackingService inactivityTrackingService
        )
    {
        _logger = logger;
        _client = client;
        
        _client.Ready += (s, e) =>
        {
            _logger.LogInformation("AlAzifBot started");
            return Task.CompletedTask;
        };

        var commandsConfig = new SlashCommandsConfiguration
        {
            Services = services
        };
        
        var commands = _client.UseSlashCommands(commandsConfig);
        
        commands.SlashCommandErrored += async (s, e) =>
        {
            _logger.LogError(e.Exception, "An error occurred while executing a slash command");
            if (e.Exception is AlAzifException alAzifException)
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("\ud83d\udeab " + alAzifException.Message));
            else
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("\ud83d\udeab An error occurred while executing the command"));
        };
        
        if (env.IsProduction())
        {
            commands.RegisterCommands(Assembly.GetExecutingAssembly());
        }
        else
        {
            foreach (var guild in config.Value.TestGuilds)
            {
                commands.RegisterCommands(Assembly.GetExecutingAssembly(), guild);
            }
        }
        
        _client.MessageCreated += async (s, e) =>
        {
            if (e.Message.Content.StartsWith("ping", StringComparison.CurrentCultureIgnoreCase))
                await e.Message.RespondAsync("Pong!");
        };
    }
 
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting AlAzifBot...");
        await _client.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping AlAzifBot...");
        await _client.DisconnectAsync();
    }
}