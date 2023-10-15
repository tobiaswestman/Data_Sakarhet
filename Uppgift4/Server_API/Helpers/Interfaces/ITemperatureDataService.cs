using Server_API.Models.Schemas;

namespace Server_API.Helpers.Interfaces
{
    public interface ITemperatureDataService
    {
        Task<bool> SaveTemperatureDataAsync(TemperatureDataSchema schema);
    }
}
