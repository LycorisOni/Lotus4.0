namespace LunnayalunaLotus;

using SPTarkov.Server.Core.Models.Spt.Mod;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.Luna.LunnayalunaLotus";
    public string Name { get; init; } = "Lotus";
    public string Author { get; init; } = "LunnayalunaLotus";
    public List<string>? Contributors { get; init; } = ["LycorisOni"];
    public SemanticVersioning.Version Version { get; init; } = new("1.7.6");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.6");
    public List<string>? Incompatibilities { get; init; } = null;
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new()
    {
        { "com.wtt.commonlib", new SemanticVersioning.Range("~3.0.6") }
    };
    public string? Url { get; init; } = null;
    public string? License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}