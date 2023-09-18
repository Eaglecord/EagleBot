using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace EagleBot;
public class TagCommands : ApplicationCommandModule
{
    [SlashCommand("tag", "Request a single tag.")]
    public static async Task TagCommand(InteractionContext ctx,
            [Option("tag", "Tag ID as found on eaglecord meta repository.")] string tagId,
            [Option("ephemeral", "Request the tag as ephemeral")] bool ephemeral = false)
    {
        await ctx.CreateResponseAsync(
            InteractionResponseType.DeferredChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder() { IsEphemeral = ephemeral, Content = "Generating response." }
        );
        // Check if the requested tag exists
        Tag? tag = null;
        foreach (Tag tagEntry in EagleBot.TagList)
        {
            if (tagEntry.Identifier == tagId || tagEntry.Aliases.Any(x => x == tagId))
                tag = tagEntry;
        }
        // Tag was not found
        if (tag is null)
        { 
            await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = $"Could not fetch url for {tagId}." });
            return;
        }
        // Tag was found, generate response content
        string content = $"{(tag.Url.Contains("/single-line/") ? "" : $"[**{tag.Url}**]({EagleBot.Config.TagUrl}{tag.Url})\n\n")}"
        + $"{await EagleBot.HttpClient.GetStringAsync($"{EagleBot.Config.TagUrl}{tag.Url}")}";
        // Modify the appearance of requested tag if necessary.
        if ((content.Length >= 4000 || (ephemeral && tag.IsPaged)) && !tag.IsEmbed)
        {
            if (ephemeral)
                await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = $"Could not request tag as ephemeral." });
            tag.IsPaged = true;
            tag.IsEmbed = true;
        } 
        else if ((content.Length >= 4096 || (ephemeral && tag.IsPaged)) && tag.IsEmbed)
        {
            if (ephemeral)
                await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = $"Could not request tag as ephemeral." });
            tag.IsPaged = true;
        }
        // Finally send the requested tag
        if (!tag.IsEmbed)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder() { Content = content });
            return;
        }
        if (!tag.IsPaged)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder() { Description = content }));
            return;
        }
        IEnumerable<Page> pages = EagleBot.Interactivity.GeneratePagesInEmbed(
            content, 
            SplitType.Line, 
            new DiscordEmbedBuilder()
        );
        await ctx.Channel?.SendPaginatedMessageAsync(ctx.Member, pages)!;
    }

    [SlashCommand("tags", "List of all available tags.")]
    public static async Task TagsCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(
            InteractionResponseType.DeferredChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder() { IsEphemeral = true, Content = "Generating response." }
        );
        // Generate response content
        string content = "List of all tags & their aliases.";
        foreach (Tag tag in EagleBot.TagList)
        {
            content += $"\n\nTag: `{tag.Identifier}`\n";
            if (tag.Aliases.Length > 0)
            {
                content += $"Aliases: `{String.Join("` `", tag.Aliases)}`";
            }
            else
            {
                content += "No aliases.";
            }
        }
        // Finally send tags list
        IEnumerable<Page> pages = EagleBot.Interactivity.GeneratePagesInEmbed(
            content, 
            SplitType.Line, 
            new DiscordEmbedBuilder() { Title = "Tags"}
        );
        await ctx.Channel?.SendPaginatedMessageAsync(ctx.Member, pages)!;
        await ctx.DeleteResponseAsync();
    }
}
