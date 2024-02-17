using AlAzif.Bot.Model;
using AlAzif.Bot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AlAzif.Bot.Modules;

public class PlayerModule(PlayerService playerService) : ApplicationCommandModule
{
    [SlashCommand("play", "Play a track")]
    public async Task EchoCommand(
        InteractionContext ctx,
        [Option("track", "The track to play")] string query,
        [Option("site", "The site to search")] SearchSite site = SearchSite.Youtube
    )
    {
        var message = await playerService.PlayAsync(query, ctx.Guild.Id, ctx.Member.VoiceState.Channel.Id, site);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }
    
    [SlashCommand("skip", "Skip the current track")]
    public async Task SkipCommand(
        InteractionContext ctx
    )
    {
        var message = await playerService.SkipAsync(ctx.Guild.Id);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }
    
    [SlashCommand("pause", "Pause the current track")]
    public async Task PauseCommand(
        InteractionContext ctx
    )
    {
        var message = await playerService.PauseAsync(ctx.Guild.Id);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }

    [SlashCommand("resume", "Resume the current track")]
    public async Task ResumeCommand(
        InteractionContext ctx
    )
    {
    }
}