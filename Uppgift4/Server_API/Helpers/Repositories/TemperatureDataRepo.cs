using Server_API.Contexts;
using Server_API.Helpers.Repositories.BaseModels;
using Server_API.Models.Entities;

namespace Server_API.Helpers.Repositories;

public class TemperatureDataRepo : Repo<TemperatureDataEntity>
{
    public TemperatureDataRepo(DataContext context) : base(context)
    {
    }
}