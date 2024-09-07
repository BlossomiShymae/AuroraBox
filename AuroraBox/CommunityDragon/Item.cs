using System.Text.Json.Serialization;

namespace AuroraBox.CommunityDragon;

public class Item
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}