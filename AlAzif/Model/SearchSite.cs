using DSharpPlus.SlashCommands;

namespace AlAzif.Model;

public enum SearchSite
{
    [ChoiceName("YouTube")]
    Youtube,
    [ChoiceName("YouTube Music")]
    YoutubeMusic,
    [ChoiceName("SoundCloud")]
    SoundCloud,
    
    // Implementing the following requires API keys
    
    // [ChoiceName("Spotify")]
    // Spotify,
    // [ChoiceName("Apple Music")]
    // AppleMusic,
    // [ChoiceName("Deezer")]
    // Deezer,
    // [ChoiceName("Yandex Music")]
    // YandexMusic,
}