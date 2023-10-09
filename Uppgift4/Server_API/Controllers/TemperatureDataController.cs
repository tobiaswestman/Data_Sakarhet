using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Server_API.Helpers.Hubs;
using Server_API.Helpers.Interfaces;
using Server_API.Models.Schemas;
using Server_API.Helpers.Encryption;

namespace Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureDataController : ControllerBase
    {
        private readonly IHubContext<TemperatureDataHub> _hubContext;
        private readonly ITemperatureDataService _temperatureDataService;

        public TemperatureDataController(IHubContext<TemperatureDataHub> hubContext, ITemperatureDataService temperatureDataService)
        {
            _hubContext = hubContext;
            _temperatureDataService = temperatureDataService;
        }

        //placeholder metod, måste uppdateras
        [Route("PostTemperature")]
        [HttpPost]
        public async Task<IActionResult> PostTemperature([FromBody] EncryptedPayload payload)
        {

            // Steg 1: Dekryptera data
            string decryptedJson = EncryptionModule.Decrypt(payload.Data);

            // Steg 2: Konvertera till TemperatureDataSchema
            TemperatureDataSchema schema = JsonConvert.DeserializeObject<TemperatureDataSchema>(decryptedJson);

            // Kontrollera om schema är null eller inte giltigt (kan lägga till ytterligare valideringslogik här)
            if (schema == null)
            {
                return BadRequest("Invalid data.");
            }

            await _temperatureDataService.SaveTemperatureDataAsync(schema);

            // Broadcast to frontend
            await _hubContext.Clients.All.SendAsync("ReceiveTemperature", schema.Value);

            return Ok();
        }
    }
}
