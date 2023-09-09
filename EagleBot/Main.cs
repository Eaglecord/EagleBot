namespace EagleBot;
public partial class EagleBot {
    private static DateTimeOffset? DailyEagle { get; set; } = null;
    public static async Task Main(string[] args) {
        await Client.ConnectAsync();
        // Slashies
        SlashCommands.RegisterCommands<TagCommands>(Config.GuildId);

        // Daily Eagle Verification
        Client.MessageCreated += async (s, e) => {
            // Check if the message was sent in the Daily Eagle thread
            if (e.Message.Channel.Id != Config.DailyEagleThread)
                return;
            // Check if the message mentions EagleEye621
            if (!e.Message.MentionedUsers.Any(user => user.Id == /* EagleEye621 User Id */ 589604959352520725))
                return;
            // Check if the previous time Eagle was mentioned is stored
            DateTimeOffset current = e.Message.Timestamp;
            if (DailyEagle is null) {
                // Save the current mention time
                DailyEagle = current;
                return;
            }
            TimeSpan? timeSinceLastMention = current - DailyEagle;
            // Save the current mention
            DailyEagle = current;
            // Angry if pinged too early
            if (timeSinceLastMention < TimeSpan.FromHours(18))
                await e.Message.RespondAsync("Too early! :angry:\n*||You can ping Eagle again in 18 hours.||*");
        };
        await Task.Delay(-1);
    }
}
