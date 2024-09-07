using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using AuroraBox.CommunityDragon;
using AuroraBox.Models;
using HtmlAgilityPack;
using McMaster.Extensions.CommandLineUtils;
using MessagePack;

namespace AuroraBox;

public partial class CommandLineInterface
{
    [Option(ShortName = "f", LongName = "format")]
    public DataFormat Format { get; } = DataFormat.Json;

    public HttpClient HttpClient { get; } = new();

    public static HtmlWeb Web { get; } = new();

    public Lazy<HtmlNode> AramDocumentNode { get; } = new(() => Web.Load("https://leagueoflegends.fandom.com/wiki/ARAM").DocumentNode);

    public Lazy<HtmlNode> ArenaDocumentNode { get; } = new(() => Web.Load("https://leagueoflegends.fandom.com/wiki/Arena_(League_of_Legends)").DocumentNode);

    public async Task OnExecuteAsync()
    {   
        var summaries = await HttpClient.GetFromJsonAsync<List<ChampionSummary>>("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json");
        var items = await HttpClient.GetFromJsonAsync<List<Item>>("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/items.json");
        var perks = await HttpClient.GetFromJsonAsync<List<Perk>>("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/perks.json");

        var aramChampionStats = GetAramChampionStats(summaries!);
        var aramItemStats = GetAramItemStats(items!);
        var aramPerkStats = GetAramPerkStats(perks!);
        var arenaChampionStats = GetArenaChampionStats(summaries!);

        switch (Format)
        {
            case DataFormat.Json:
                var options = new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                File.WriteAllText("aram-champions.json", JsonSerializer.Serialize(aramChampionStats, options));
                File.WriteAllText("aram-items.json", JsonSerializer.Serialize(aramItemStats, options));
                File.WriteAllText("aram-perks.json", JsonSerializer.Serialize(aramPerkStats, options));
                File.WriteAllText("arena-champions.json", JsonSerializer.Serialize(arenaChampionStats, options));
                break;
            case DataFormat.MessagePack:
                File.WriteAllBytes("aram-champions.msgpack", MessagePackSerializer.Serialize(aramChampionStats));
                File.WriteAllBytes("aram-items.msgpack", MessagePackSerializer.Serialize(aramItemStats));
                File.WriteAllBytes("aram-perks.msgpack", MessagePackSerializer.Serialize(aramPerkStats));
                File.WriteAllBytes("arena-champions.msgpack", MessagePackSerializer.Serialize(arenaChampionStats));
                break;
        }
    }

    private SortedDictionary<int, AramPerkStats> GetAramPerkStats(List<Perk> perks)
    {
        return AramDocumentNode.Value
            .SelectNodes("//div[contains(@class, 'tabber')]/div[contains(@class, 'wds-tab__content')]")[2].ChildNodes
            .Skip(1)
            .Where(x => x.NodeType == HtmlNodeType.Element)
            .Chunk(2)
            .Select(x =>
            {
                var stats = new AramPerkStats()
                {
                    Name = x[0].InnerText.Trim(),
                    Effects = ParseEffects(x[1].InnerHtml),
                };
                var perk = perks.Find(p => p.Name == stats.Name)!;
                stats.Id = perk.Id;
                return (Id: stats.Id, Stats: stats);
            })
            .Aggregate(new SortedDictionary<int, AramPerkStats>(), (a, b) =>
            {
                a[b.Id] = b.Stats;
                return a;
            });
    }

    private SortedDictionary<int, AramItemStats> GetAramItemStats(List<Item> items)
    {
        return AramDocumentNode.Value
            .SelectNodes("//div[contains(@class, 'tabber')]/div[contains(@class, 'wds-tab__content')]")[1].ChildNodes
            .Skip(1)
            .Where(x => x.NodeType == HtmlNodeType.Element)
            .Chunk(2)
            .Select(x => 
            {
                var stats = new AramItemStats()
                {
                    Name = x[0].InnerText.Trim(),
                    Effects = ParseEffects(x[1].InnerHtml),
                };
                var item = items.Find(p => p.Name == stats.Name)!;
                stats.Id = item.Id;
                return (Id: stats.Id, Stats: stats);
            })
            .Aggregate(new SortedDictionary<int, AramItemStats>(), (a, b) => 
            {
                a[b.Id] = b.Stats;
                return a;
            });
    }

    public SortedDictionary<string, AramChampionStats> GetAramChampionStats(List<ChampionSummary> summaries)
    {
        static double ParseDamageStat(string innerText)
        {
            if (string.IsNullOrEmpty(innerText)) return 100.0;

            return 100.0 + double.Parse(innerText.Replace("%", string.Empty));
        }
    
       return AramDocumentNode.Value
            .SelectNodes("//div[contains(@class, 'tabber')]/div[contains(@class, 'wds-tab__content')]/table/tbody/tr")
            .Skip(1)
            .Select(x =>
            {
                var stats = new AramChampionStats()
                {
                    Name = HttpUtility.HtmlDecode(x.ChildNodes[0].Attributes["data-sort-value"].Value),
                    DamageDealt = ParseDamageStat(x.ChildNodes[1].InnerText),
                    DamageReceived = ParseDamageStat(x.ChildNodes[2].InnerText),
                    Effects = ParseEffects(x.ChildNodes[3].InnerHtml)
                };
                var summary = summaries.Find(p => p.Name == stats.Name && !p.Alias.Contains("Strawberry"))!;
                stats.Id = summary.Id;
                return (Alias: summary.Alias, Stats: stats);
            })
            .Aggregate(new SortedDictionary<string, AramChampionStats>(), (a, b) => 
            {
                a[b.Alias] = b.Stats;
                return a;
            });
    }

    public SortedDictionary<string, ArenaChampionStats> GetArenaChampionStats(List<ChampionSummary> summaries)
    {
        return ArenaDocumentNode.Value
            .SelectNodes("//div[contains(@class, 'tabber')]//table[contains(@class, 'article-table') and contains(@class, 'sortable')]/tbody/tr")
            .Skip(1)
            .Select(x => 
            {
                var stats = new ArenaChampionStats()
                {
                    Name = HttpUtility.HtmlDecode(x.ChildNodes[0].Attributes["data-sort-value"].Value),
                    Effects = ParseEffects(x.ChildNodes[3].InnerHtml),
                };
                var summary = summaries.Find(p => p.Name == stats.Name && !p.Alias.Contains("Strawberry"))!;
                stats.Id = summary.Id;
                return (Alias: summary.Alias, Stats: stats);
            })
            .Aggregate(new SortedDictionary<string, ArenaChampionStats>(), (a, b) =>
            {
                a[b.Alias] = b.Stats;
                return a;
            });
    }

    private static string ParseEffects(string html)
    {
        return StripHtmlAttributesRegex()
            .Replace(html.Trim(), "<$1$2>")
            .Replace("<img>", string.Empty)
            .Replace("<a>", string.Empty)
            .Replace("</a>", string.Empty);
    }

    [GeneratedRegex("<([a-z][a-z0-9]*)[^>]*?(/?)>")]
    private static partial Regex StripHtmlAttributesRegex();
}