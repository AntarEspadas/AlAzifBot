using AlAzif.Configuration;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

namespace AlAzif;

public class AlAzifBot : IHostedService
{

    private readonly ILogger<AlAzifBot> _logger;

    private readonly DiscordClient _client;

    public AlAzifBot(
        ILogger<AlAzifBot> logger,
        ILoggerFactory loggerFactory,
        IOptions<AlAzifConfig> config,
        IServiceProvider services,
        IHostEnvironment env
        )
    {
        _logger = logger;
        
        var discordConfig = new DiscordConfiguration
        {
            Token = config.Value.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            LoggerFactory = loggerFactory
        };
        
        _client = new DiscordClient(discordConfig);
        
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
        
        if (env.IsDevelopment())
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
 
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting AlAzifBot...");
        return _client.ConnectAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping AlAzifBot...");
        return _client.DisconnectAsync();
    }
}