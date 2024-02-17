using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AlAzif.Bot.Modules;

public class ProfilePictureModule : ApplicationCommandModule
{
    [SlashCommand("pfp", "Get the profile picture of a user")]
    public async Task ProfilePictureCommand(
        InteractionContext ctx,
        [Option("user", "The user to get the profile picture of")] string user
    )
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(user));
    }
}