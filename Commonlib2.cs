using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace LunnayalunaLotus;

[Injectable(TypePriority = OnLoadOrder.TraderRegistration + 3)]
public class Oni2(
    WTTServerCommonLib.WTTServerCommonLib wttCommon
) : IOnLoad
{
    public  async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Use WTT-CommonLib services
        await wttCommon.CustomQuestService.CreateCustomQuests(assembly);
        await wttCommon.CustomAssortSchemeService.CreateCustomAssortSchemes(assembly);
        await wttCommon.CustomQuestService.CreateCustomQuests(assembly);
        await wttCommon.CustomDialogueService.CreateCustomDialogues(assembly);
        await wttCommon.CustomQuestZoneService.CreateCustomQuestZones(assembly);
        await Task.CompletedTask;
    }
}