using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace AlAzif.Configuration;

public class AlAzifConfig
{
    [Required(ErrorMessage = "Please provide a Discord bot token", AllowEmptyStrings = false)]
    public required string Token { get; init; }

    public IList<ulong> TestGuilds { get; init; } = new List<ulong>();
}