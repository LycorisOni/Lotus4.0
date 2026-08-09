using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using System.Reflection;
using Path = System.IO.Path;
//Very important this is your namespace in all your .cs files or you break everything
namespace LunnayalunaLotus;

// This record holds the various properties for your mod
public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.Luna.LunnayalunaLotus";
    public override string Name { get; init; } = "Lotus";
    public override string Author { get; init; } = "LunnayalunaLotus";
    public override List<string>? Contributors { get; init; } = ["LycorisOni"];
    public override SemanticVersioning.Version Version { get; init; } = new("1.7.4");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; } = null;
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new()
    {
        { "com.wtt.commonlib", new SemanticVersioning.Range("~2.0") }
    };
    public override string? Url { get; init; } = null;
    public override bool? IsBundleMod { get; init; } = false;
    public override string? License { get; init; } = "MIT";
}

//This is the injectable. This determines load order. Usually don't ever need to mess with this for a Trader
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
//This is your main public class. Decides what you are doing basically. 
public class LunaLotusJsonLoad(
    ISptLogger<LunaLotusJsonLoad> logger,
    ModHelper modHelper,
    ImageRouter imageRouter,
    ConfigServer configServer,
    TimeUtil timeUtil,
    DatabaseService databaseService,
    AddCustomTraderHelper addCustomTraderHelper // This class is a custom one to be used as the main class for the mod. 
     
)
    : IOnLoad
//I would not worry about this leave it be. 
{
    private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();

//Your new public task this does some lovely grabbing of paths to make your life not difficult
    public Task OnLoad()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Make sure to check the Lotus modpage for gunsmith task solutions");
        Console.ResetColor();
        // A path to the mods files we use below
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        // A relative path to the trader icon to show
        var traderImagePath = Path.Combine(pathToMod, "res/Lotus.jpg");

        // The base json containing trader settings we will add to the server
        var traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "data/base.json");

        // Create a helper class and use it to register our traders image/icon + set its stock refresh time
        imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        addCustomTraderHelper.SetTraderUpdateTime(_traderConfig, traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

        // Adds the trader's configuration to the server to be loaded.
        _ragfairConfig.Traders.TryAdd(traderBase.Id, true);

        // This just uses the useful trader helper to not have a major headache doing it all in here.
        addCustomTraderHelper.AddTraderWithEmptyAssortToDb(traderBase);

        // For the trader this only affects the base really no quests you'll need to be careful with that part using wtt commonlib now.
        addCustomTraderHelper.AddTraderToLocales(traderBase, "Lotus", "A businesswoman who travels around conflict zones around the world.");

        // Grabs the assortment data so you have an assort. 
        var lotusassort = modHelper.GetJsonDataFromFile<TraderAssort>(pathToMod, "data/assort.json");
        
        addCustomTraderHelper.OverwriteTraderAssort(traderBase.Id, lotusassort);
        
        return Task.CompletedTask;
    }
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class EditDatabaseValues(
    DatabaseService databaseService)
    : IOnLoad
{
    public Task OnLoad()
    {
        EditLabs(databaseService);

        return Task.CompletedTask;
    }

    public void EditLabs(DatabaseService databaseService)
    {
        var locations = databaseService.GetLocations();
        var lab = locations.Laboratory;

        lab.Base.AccessKeys = lab.Base.AccessKeys.Append("6747b519aa6cb78b189e6081");
        lab.Base.AccessKeysPvE = lab.Base.AccessKeysPvE.Append("6747b519aa6cb78b189e6081");
    }

}
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public class Oni(
    WTTServerCommonLib.WTTServerCommonLib wttCommon
) : IOnLoad
{
    public async Task OnLoad()
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Use WTT-CommonLib services
        await wttCommon.CustomAssortSchemeService.CreateCustomAssortSchemes(assembly);
        await wttCommon.CustomQuestService.CreateCustomQuests(assembly);
        await wttCommon.CustomQuestZoneService.CreateCustomQuestZones(assembly);
        await wttCommon.CustomItemServiceExtended.CreateCustomItems(assembly);
        await wttCommon.CustomDialogueService.CreateCustomDialogues(assembly);
        await Task.CompletedTask;
    }
}