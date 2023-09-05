using System.Text.Json;
using System.Text.Json.Serialization;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Enums;
using Microsoft.Extensions.Logging;

namespace EagleBot;
public partial class EagleBot
{
    public static DiscordClient Client = new DiscordClient(
        new DiscordConfiguration() {
            Token = Config.Token,
            TokenType = TokenType.Bot,
            MinimumLogLevel = LogLevel.Information
        }
    );
    public static InteractivityExtension Interactivity { get; set; } = Client.UseInteractivity(new InteractivityConfiguration() {
        ButtonBehavior = ButtonPaginationBehavior.Ignore,
        PaginationBehaviour = PaginationBehaviour.WrapAround,
        PaginationDeletion = PaginationDeletion.DeleteMessage,
        Timeout = TimeSpan.FromMinutes(5)
    });
    public static SlashCommandsExtension SlashCommands { get; private set; } = Client.UseSlashCommands();
    public static Configuration Config { 
        get {
            if (!File.Exists("./configuration.json"))
                throw new FileNotFoundException("Missing configuration file in root.", "./configuration.json");
            StreamReader sr = new("configuration.json");
            string config = sr.ReadToEnd();
            sr.Dispose();
            return JsonSerializer.Deserialize<Configuration>(config)!;
        }
    }
}

public class Configuration {
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("guildId")]
    public ulong GuildId { get; set; } = 0;

    [JsonPropertyName("tagUrl")]
    public string TagUrl { get; set; } = string.Empty;
}
