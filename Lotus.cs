using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using SPTarkov.DI.Annotations;
using Path = System.IO.Path;

namespace LunnayalunaLotus;

[Injectable(TypePriority = OnLoadOrder.TraderRegistration + 1)]
public class AddTraderWithAssortJson(
    ModHelper modHelper,
    ImageRouter imageRouter,
    TraderConfig traderConfig,
    RagfairConfig ragfairConfig,
    TimeUtil timeUtil,
    AddCustomTraderHelper addCustomTraderHelper // This is a custom class we add for this mod, we made it injectable so it can be accessed like other classes here
)
    : IOnLoad
{
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
        // This just uses the useful trader helper to not have a major headache
        addCustomTraderHelper.AddTraderWithEmptyAssortToDb(traderBase);

        // Add localisation text for our trader to the database so it shows to people playing in different languages
        addCustomTraderHelper.AddTraderToLocales(traderBase, "Lotus", "A businesswoman who travels around conflict zones around the world.");

        // Grabs the assortment data so you have an assort.
        var assort = modHelper.GetJsonDataFromFile<TraderAssort>(pathToMod, "data/assort.json");

        // Save the data we loaded above into the trader we've made
        addCustomTraderHelper.OverwriteTraderAssort(traderBase.Id, assort);

        // Send back a success to the server to say our trader is good to go
        return Task.CompletedTask;
    }
}
