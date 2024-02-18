using System.Diagnostics;
using AlAzif.Bot.Exceptions;
using AlAzif.Bot.Model;
using AlAzif.Core.Procedures;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Options;

namespace AlAzif.Bot.Services;

public class PlayerService(ILogger<PlayerService> logger, IAudioService audioService, IOptions<QueuedLavalinkPlayerOptions> playerOptions)
{
    public async Task<String> PlayAsync(string query, ulong guildId, ulong? channelId, SearchSite site)
    {
        var retrieveOptions = new PlayerRetrieveOptions
        {
            ChannelBehavior = PlayerChannelBehavior.Join,
            VoiceStateBehavior = MemberVoiceStateBehavior.RequireSame
        };
        logger.LogDebug("Retrieving player for guild {GuildId} and channel {ChannelId}", guildId, channelId);
        var result = await audioService.Players.RetrieveAsync(guildId, channelId,
            PlayerFactory.Queued, playerOptions, retrieveOptions);

        if (!result.IsSuccess)
        {
            var message = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You must be in a voice channel to use this command",
                PlayerRetrieveStatus.VoiceChannelMismatch => "You must be in the same voice channel as the bot to use this command",
                _ => throw new InvalidOperationException($"Cannot handle status {result.Status}")
            };
            throw new AlAzifException(message);
        }
        
        logger.LogDebug("Retrieved player for guild {GuildId} and channel {ChannelId}", guildId, channelId);

        var player = result.Player;
        
        logger.LogDebug("Loading track {Query}", query);

        var searchMode = ToTrackSearchMode(site);

        var track = await audioService.Tracks.LoadTrackAsync(query, searchMode);
        
        logger.LogDebug("Loaded track {Title}", track?.Title);
        
        if (track is null)
            throw new AlAzifException($"Failed to load track `{query}`");

        await player.PlayAsync(track);
        
        if (player.Queue.Any())
            // Clock emoji
            return $"\ud83d\udd53 Added `{track.Title}` to queue";
        // Play emoji
        return $"\u25b6\ufe0f Playing `{track.Title}`";
    }

    private static TrackSearchMode ToTrackSearchMode(SearchSite site)
    {
        return site switch
        {
            SearchSite.Youtube => TrackSearchMode.YouTube,
            SearchSite.YoutubeMusic => TrackSearchMode.YouTubeMusic,
            SearchSite.SoundCloud => TrackSearchMode.SoundCloud,
            _ => throw new UnreachableException("Invalid search site")
        };
    }
    
    public async Task<(LavalinkTrack, LavalinkTrack?)> SkipAsync(ulong guildId)
    {
        var player = GetPlayer(guildId, audioService);
        
        Debug.Assert(player is not null);
        
        var currentTrack = player.CurrentTrack;
        var nextTrack = player.Queue.FirstOrDefault()?.Track;
        
        if (currentTrack is null)
            throw new AlAzifException("Nothing to skip");
        
        logger.LogDebug("Skipping track. Tracks in queue: {Count}", player.Queue.Count);
        await player.SkipAsync();
        
        return (currentTrack, nextTrack);
    }
    
    public async Task<LavalinkTrack> PauseAsync(ulong guildId)
    {
        var player = GetPlayer(guildId, audioService);
        
        Debug.Assert(player is not null);
        
        var currentTrack = player.CurrentTrack;
        
        if (currentTrack is null)
            throw new AlAzifException("Noting to pause");
        
        logger.LogDebug("Pausing player");
        await player.PauseAsync();

        return currentTrack;
    }

    public async Task<LavalinkTrack> ResumeAsync(ulong guildId)
    {
        var player = GetPlayer(guildId, audioService);
        
        Debug.Assert(player is not null);
        
        var currentTrack = player.CurrentTrack;
        
        if (currentTrack is null)
            throw new AlAzifException("Noting to resume");
        
        logger.LogDebug("Resuming player");
        await player.ResumeAsync();

        return currentTrack;
    }

    public DiscordEmbedBuilder GetPlaylist(ulong guildId)
    {
        var player = GetPlayer(guildId, audioService);
        
        Debug.Assert(player is not null);

        return PlayerProcedures.displayQueue(player);
    }

    public async Task Disconnect(ulong guildId)
    {
        var player = GetPlayer(guildId, audioService);
                
        logger.LogDebug("Disconnecting player");
        await player.DisconnectAsync();
    }
    
    private static IQueuedLavalinkPlayer GetPlayer(ulong guildId, IAudioService audioService)
    {
        if (!audioService.Players.TryGetPlayer<QueuedLavalinkPlayer>(guildId, out var player))
            throw new AlAzifException($"Bot is not in a voice channel");
        
        Debug.Assert(player is not null);
        
        return player;
    }
}