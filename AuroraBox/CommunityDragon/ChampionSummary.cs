using System.Text.Json.Serialization;

namespace AuroraBox.CommunityDragon;

public class ChampionSummary
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("alias")]
    public required string Alias { get; set; }
}