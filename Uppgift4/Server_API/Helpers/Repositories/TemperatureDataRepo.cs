using Server_API.Contexts;
using Server_API.Helpers.Repositories.BaseModels;
using Server_API.Models.Entities;

namespace Server_API.Helpers.Repositories;

public class TemperatureDataRepo : Repo<TemperatureDataEntity>
{
    //Temperature data repo, inherits from base repo
    public TemperatureDataRepo(DataContext context) : base(context)
    {
    }
}