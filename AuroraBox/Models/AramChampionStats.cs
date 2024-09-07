using System.Text.Json.Serialization;
using System.Web;
using HtmlAgilityPack;
using MessagePack;

namespace AuroraBox.Models;

[MessagePackObject]
public class AramChampionStats
{
    [JsonPropertyName("name")]
    [Key(0)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    [Key(1)]
    public int Id { get; set; }

    [JsonPropertyName("damageDealt")]
    [Key(2)]
    public double DamageDealt { get; set; }

    [JsonPropertyName("damageReceived")]
    [Key(3)]
    public double DamageReceived { get; set; }

    [JsonPropertyName("effects")]
    [Key(4)]
    public string Effects { get; set; } = string.Empty;
}