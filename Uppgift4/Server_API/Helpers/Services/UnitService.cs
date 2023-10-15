using Server_API.Contexts;
using Server_API.Helpers.Interfaces;
using Server_API.Models.Entities;

namespace Server_API.Helpers.Services
{
    public class UnitService : IUnitService
    {
        private readonly DataContext _context;

        public UnitService(DataContext context)
        {
            _context = context;
        }

        //Register new IoT-unit, generate deviceId as Device1, Device2 etc based on already existing devices in db
        public async Task<UnitEntity> RegisterNewUnitAsync()
        {
            int existingDevicesCount = _context.Units.Count();
            string newDeviceId = $"Device{existingDevicesCount + 1}";

            var unit = new UnitEntity { DeviceId = newDeviceId };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return unit;
        }
    }

}
