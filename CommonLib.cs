using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace LunnayalunaLotus;

[Injectable(TypePriority = OnLoadOrder.Preload + 2)]
public class Oni(
    WTTServerCommonLib.WTTServerCommonLib wttCommon
) : IOnLoad
{
    public  async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Use WTT-CommonLib services
        await wttCommon.CustomAssortSchemeService.CreateCustomAssortSchemes(assembly);
        await wttCommon.CustomQuestService.CreateCustomQuests(assembly);
        await wttCommon.CustomQuestZoneService.CreateCustomQuestZones(assembly);
        await wttCommon.CustomItemServiceExtended.CreateCustomItems(assembly);
        await wttCommon.CustomDialogueService.CreateCustomDialogues(assembly);
        await wttCommon.CustomLootspawnService.CreateCustomLootSpawns(assembly);
        await Task.CompletedTask;
    }
}