using System.Text.Json.Serialization;

namespace EagleBot;
public class Tag
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("aliases")]
    public string[] Aliases { get; set; } = Array.Empty<string>();

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("isEmbed")]
    public bool IsEmbed { get; set; } = false;

    [JsonPropertyName("isPaged")]
    public bool IsPaged { get; set; } = false;
}
