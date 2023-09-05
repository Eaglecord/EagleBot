namespace EagleBot;
public partial class EagleBot {
    public static async Task Main(string[] args) {
        await Client.ConnectAsync();
        await Task.Delay(-1);
    }
}
