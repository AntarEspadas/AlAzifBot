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
        var message = await playerService.PlayAsync(query, ctx.Guild.Id, ctx.Member.VoiceState?.Channel?.Id, site);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }
    
    [SlashCommand("skip", "Skip the current track")]
    public async Task SkipCommand(
        InteractionContext ctx
    )
    {
        var (currentTrack, nextTrack) = await playerService.SkipAsync(ctx.Guild.Id);
        var message = $"\u23e9 Skipped `{currentTrack.Title}`";
        if (nextTrack is not null)
            message += $"\n\u25b6\ufe0f Now playing `{nextTrack.Title}`";
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }
    
    [SlashCommand("pause", "Pause the current track")]
    public async Task PauseCommand(
        InteractionContext ctx
    )
    {
        var track = await playerService.PauseAsync(ctx.Guild.Id);
        var message = $"\u23f8\ufe0f Paused `{track.Title}`";
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }

    [SlashCommand("resume", "Resume the current track")]
    public async Task ResumeCommand(
        InteractionContext ctx
    )
    {
        var track = await playerService.ResumeAsync(ctx.Guild.Id);
        var message = $"\u25b6\ufe0f Resumed `{track.Title}`";
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }
    
    [SlashCommand("playlist", "View the current playlist")]
    public async Task PlaylistCommand(
        InteractionContext ctx
    )
    {
        var embed = playerService.GetPlaylist(ctx.Guild.Id);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }
    
    [SlashCommand("disconnect", "Disconnect the bot from the voice channel")]
    public async Task DisconnectCommand(
        InteractionContext ctx
    )
    {
        await playerService.Disconnect(ctx.Guild.Id);
        const string message = "\ud83d\udd34 Disconnected";
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));
    }
}