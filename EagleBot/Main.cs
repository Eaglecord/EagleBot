using DSharpPlus;
using DSharpPlus.EventArgs;

namespace EagleBot;
public partial class EagleBot
{
    private static DateTimeOffset? DailyEagle { get; set; } = null;
    public static async Task Main(string[] args)
    {
        await Client.ConnectAsync();
        // Slashies
        SlashCommands.RegisterCommands<TagCommands>(Config.GuildId);
        
        // Daily Eagle Verification
        Client.MessageCreated += DailyEaglePingReceived;
        Client.MessageDeleted += DailyEaglePingDeleted;
        
        await Task.Delay(-1);
    }
    private static async Task DailyEaglePingReceived(DiscordClient s, MessageCreateEventArgs e)
    {
        if (e.Message.Channel.Id != Config.DailyEagleThread)
            return;
        if (!e.Message.MentionedUsers.Any(user => user.Id == /* EagleEye621 User Id */ 589604959352520725))
            return;
        
        DateTimeOffset current = e.Message.Timestamp;
        if (DailyEagle is null)
        {
            DailyEagle = current;
            return;
        }
        TimeSpan t = current - (DateTimeOffset) DailyEagle;
        TimeSpan timeSinceLastMention = new TimeSpan(t.Days, t.Hours, t.Minutes, 0);
        DailyEagle = current;
        
        if (timeSinceLastMention < TimeSpan.FromHours(18))
            await e.Message.RespondAsync("Too early! :angry:\n*||You can ping Eagle again in 18 hours.||*");
    }
    private static async Task DailyEaglePingDeleted(DiscordClient s, MessageDeleteEventArgs e)
    {
        if (e.Message.Channel.Id != Config.DailyEagleThread)
            return;
        if (!e.Message.MentionedUsers.Any(user => user.Id == /* EagleEye621 User Id */ 589604959352520725))
            return;
        // send the timestamp of a deleted message
        await e.Channel.SendMessageAsync($"A message from {e.Message.Author.Mention} pinging EagleEye621 <t:{e.Message.Timestamp.ToUnixTimeSeconds()}:R> was deleted.");
    }
}
