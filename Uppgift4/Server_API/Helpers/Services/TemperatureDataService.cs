using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Repositories;
using Server_API.Models.Entities;
using Server_API.Models.Schemas;

namespace Server_API.Helpers.Services;

public class TemperatureDataService : ITemperatureDataService
{
    private readonly TemperatureDataRepo _temperatureDataRepo;

    public TemperatureDataService(TemperatureDataRepo temperatureDataRepo)
    {
        _temperatureDataRepo = temperatureDataRepo;
    }

    //Denna bör hämta en iot-unit och sätta den också från id, detta är en första draft
    public async Task<bool> SaveTemperatureDataAsync(TemperatureDataSchema schema)
    {
        var temperatureData = new TemperatureDataEntity
        {
            Value = schema.Value,
            Timestamp = DateTime.UtcNow,
            UnitId = schema.UnitId
        };

        //await _temperatureDataRepo.AddAsync(temperatureData);
        return true;

    }
}