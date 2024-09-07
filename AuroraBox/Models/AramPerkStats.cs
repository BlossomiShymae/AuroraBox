
using System.Text.Json.Serialization;
using MessagePack;

namespace AuroraBox.Models;

[MessagePackObject]
public class AramPerkStats
{
    [JsonPropertyName("id")]
    [Key(0)]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [Key(1)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("effects")]
    [Key(2)]
    public string Effects { get; set; } = string.Empty;
}