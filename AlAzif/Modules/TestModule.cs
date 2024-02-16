using AlAzif.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AlAzif.Modules;

public class TestModule(IOptions<AlAzifConfig> options) : ApplicationCommandModule
{
    
    [SlashCommand("ping", "Replies with Pong!")]
    public async Task TestCommand(InteractionContext ctx) =>
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pong!"));

    [SlashCommand("echo", "Replies with the provided text")]
    public async Task EchoCommand(
        InteractionContext ctx,
        [Option("text", "The text to echo")] string text
    ) =>
         await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(text));
}