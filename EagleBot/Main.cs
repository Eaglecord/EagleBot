namespace EagleBot;
public partial class EagleBot {
    public static async Task Main(string[] args) {
        await Client.ConnectAsync();
        // Slashies
        SlashCommands.RegisterCommands<TagCommands>(Config.GuildId);
        await Task.Delay(-1);
    }
}
