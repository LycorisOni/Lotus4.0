using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace LunnayalunaLotus;

[Injectable(TypePriority = OnLoadOrder.PostLoad+ 1)]
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
        var locations = locationTable.Laboratory;
        var lab = locations;

        lab.Base.AccessKeys = lab.Base.AccessKeys.Append("6747b519aa6cb78b189e6081");
        lab.Base.AccessKeysPvE = lab.Base.AccessKeysPvE.Append("6747b519aa6cb78b189e6081");
    }

}