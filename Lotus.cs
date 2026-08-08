using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using Path = System.IO.Path;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;
//Very important this is your namespace in all your .cs files or you break everything
namespace LunnayalunaLotus;

// This record holds the various properties for your mod
public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.Luna.LunnayalunaLotus";
    public string Name { get; init; } = "Lotus";
    public string Author { get; init; } = "LunnayalunaLotus";
    public List<string>? Contributors { get; init; } = ["LycorisOni"];
    public Version Version { get; init; } = new("1.8.0");
    public Range SptVersion { get; init; } = new("~4.1.0");
    public bool HasPrepatcher { get; init; } = false;
    public List<string>? Incompatibilities { get; init; } = null;
    public Dictionary<string, Range>? ModDependencies { get; init; } = new()
    {
        { "com.wtt.commonlib", new Range("~3.0") }
    };
    public string? Url { get; init; } = null;
    public string License { get; init; } = "MIT";
}

//This is the injectable. This determines load order. Usually don't ever need to mess with this for a Trader
[Injectable(TypePriority = OnLoadOrder.TraderRegistration + 1)]
//This is your main public class. Decides what you are doing basically. 
public class LunaLotusJsonLoad(
    ISptLogger<LunaLotusJsonLoad> logger,
    ModHelper modHelper,
    ImageRouter imageRouter,
    TraderConfig traderConfig,
    RagfairConfig ragfairConfig,
    TimeUtil timeUtil,
    AddCustomTraderHelper addCustomTraderHelper // This class is a custom one to be used as the main class for the mod. 
     
)
    : IOnLoad
//I would not worry about this leave it be. 
{
//Your new public task this does some lovely grabbing of paths to make your life not difficult
    public Task OnLoadAsync(CancellationToken cancellationToken)
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
        addCustomTraderHelper.SetTraderUpdateTime(traderConfig, traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

        // Adds the trader's configuration to the server to be loaded.
        ragfairConfig.Traders.TryAdd(traderBase.Id, true);

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

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class EditDatabaseValues(
    LocationTable locationTable)
    : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        EditLabs(locationTable);

        return Task.CompletedTask;
    }

    public void EditLabs(LocationTable locationTable)
    {
        var lab = locationTable.Laboratory;

        lab.Base.AccessKeys = lab.Base.AccessKeys.Append("6747b519aa6cb78b189e6081");
        lab.Base.AccessKeysPvE = lab.Base.AccessKeysPvE.Append("6747b519aa6cb78b189e6081");
    }

}
[Injectable(TypePriority = OnLoadOrder.PostLoad + 2)]
public class Oni(
    WTTServerCommonLib.WTTServerCommonLib wttCommon
) : IOnLoad
{
    public async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Use WTT-CommonLib services
        await wttCommon.CustomAssortSchemeService.CreateCustomAssortSchemes(assembly);
        await wttCommon.CustomQuestService.CreateCustomQuests(assembly);
        await wttCommon.CustomQuestZoneService.CreateCustomQuestZones(assembly);
        await wttCommon.CustomItemServiceExtended.CreateCustomItems(assembly);    
    }
}
