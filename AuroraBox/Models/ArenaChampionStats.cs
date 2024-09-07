using System.Text.Json.Serialization;
using System.Web;
using HtmlAgilityPack;
using MessagePack;

namespace AuroraBox.Models;

[MessagePackObject]
public class ArenaChampionStats
{
    [JsonPropertyName("name")]
    [Key(0)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    [Key(1)]
    public int Id { get; set; }

    [JsonPropertyName("effects")]
    [Key(2)]
    public string Effects { get; set; } = string.Empty;
}