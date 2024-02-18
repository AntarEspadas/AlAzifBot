using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AlAzif.Bot.Modules;

public class ProfilePictureModule : ApplicationCommandModule
{
    [SlashCommand("pfp", "Get the profile picture of a user")]
    public async Task ProfilePictureCommand(
        InteractionContext ctx,
        [Option("user", "The user to get the profile picture of")] DiscordUser user
    )
    {
        var response = new DiscordInteractionResponseBuilder()
            .AddEmbed(
                new DiscordEmbedBuilder()
                    .WithImageUrl(user.AvatarUrl)
                    .WithTitle(user.Username)
                );
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
    }
}