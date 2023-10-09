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

    //Denna b�r h�mta en iot-unit och s�tta den ocks� fr�n id, detta �r en f�rsta draft
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