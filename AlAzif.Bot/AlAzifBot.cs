using System.Reflection;
using AlAzif.Bot.Configuration;
using AlAzif.Bot.Exceptions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AlAzif.Bot;

public class AlAzifBot : IHostedService
{

    private readonly ILogger<AlAzifBot> _logger;

    private readonly DiscordClient _client;

    public AlAzifBot(
        DiscordClient client,
        ILogger<AlAzifBot> logger,
        IOptions<AlAzifConfig> config,
        IServiceProvider services,
        IHostEnvironment env
        )
    {
        _logger = logger;
        _client = client;

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
        
        _logger.LogDebug("Registering commands for all guilds");
        commands.RegisterCommands(Assembly.GetExecutingAssembly());
        if (env.IsDevelopment())
        {
            _logger.LogDebug("Registering commands for test guilds: {TestGuilds}", config.Value.TestGuilds);
            foreach (var guild in config.Value.TestGuilds)
            {
                commands.RegisterCommands(Assembly.GetExecutingAssembly(), guild);
            }
        }
         
        _client.Ready += (s, e) =>
        {
            _logger.LogDebug("Registered commands: {Commands}", commands.RegisteredCommands);
            _logger.LogInformation("AlAzifBot started");
            return Task.CompletedTask;
        };
       
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