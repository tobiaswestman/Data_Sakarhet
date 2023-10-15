using Server_API.Models.Entities;

namespace Server_API.Helpers.Interfaces
{
    public interface IUnitService
    {
        Task<UnitEntity> RegisterNewUnitAsync();
    }
}
