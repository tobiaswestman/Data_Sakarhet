using Server_API.Contexts;
using Server_API.Helpers.Repositories.BaseModels;
using Server_API.Models.Entities;

namespace Server_API.Helpers.Repositories;

public class TemperatureRepo : Repo<TemperatureDataEntity>
{
    public TemperatureRepo(DataContext context) : base(context)
    {
    }
}