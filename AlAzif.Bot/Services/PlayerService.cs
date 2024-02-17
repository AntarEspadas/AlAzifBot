using System.Diagnostics;
using AlAzif.Bot.Exceptions;
using AlAzif.Bot.Model;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlAzif.Bot.Services;

public class PlayerService(ILogger<PlayerService> logger, IAudioService audioService, IOptions<QueuedLavalinkPlayerOptions> playerOptions)
{
    public async Task<String> PlayAsync(string query, ulong guildId, ulong channelId, SearchSite site)
    {
        var retrieveOptions = new PlayerRetrieveOptions
        {
            ChannelBehavior = PlayerChannelBehavior.Join,
        };
        logger.LogDebug("Retrieving player for guild {GuildId} and channel {ChannelId}", guildId, channelId);
        var result = await audioService.Players.RetrieveAsync(guildId, channelId,
            PlayerFactory.Queued, playerOptions, retrieveOptions);

        if (!result.IsSuccess)
        {
            var message = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You must be in a voice channel to use this command",
                PlayerRetrieveStatus.VoiceChannelMismatch => "You must be in the same voice channel to use this command",
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
            throw new AlAzifException($"Failed to load track {query}");

        await player.PlayAsync(track);
        
        if (player.Queue.Count > 0)
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
    
    public async Task<String> SkipAsync(ulong guildId)
    {
        if (!audioService.Players.TryGetPlayer<QueuedLavalinkPlayer>(guildId, out var player))
            throw new AlAzifException($"No player found for guild {guildId}");
        
        Debug.Assert(player is not null);
        
        var skippedTrack = player.CurrentTrack;
        
        if (skippedTrack is null)
            throw new AlAzifException("No tracks in queue");
        
        logger.LogDebug("Skipping track");
        await player.SkipAsync();
        
        // Fast forward emoji
        var message = $"\u23e9 Skipped `{skippedTrack.Title}`";
        
        var currentTrack = player.CurrentTrack;
        
        if (currentTrack is not null)
            // Play emoji
            message += $"\n\u25b6\ufe0f Now playing `{currentTrack.Title}`";
        
        return message;
    }
    
    public async Task<string> PauseAsync(ulong guildId)
    {
        if (!audioService.Players.TryGetPlayer<QueuedLavalinkPlayer>(guildId, out var player))
            throw new AlAzifException($"No player found for guild {guildId}");
        
        Debug.Assert(player is not null);
        
        logger.LogDebug("Pausing player");
        await player.PauseAsync();
        
        // Pause emoji
        return "\u23f8 Paused";
    }
}