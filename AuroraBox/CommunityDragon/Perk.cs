using System.Text.Json.Serialization;

namespace AuroraBox.CommunityDragon;

public class Perk
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}