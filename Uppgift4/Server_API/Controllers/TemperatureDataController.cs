using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Server_API.Helpers.Hubs;
using Server_API.Helpers.Interfaces;
using Server_API.Models.Schemas;
using Server_API.Helpers.Encryption;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Server_API.Models;

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

        // API endpoint to post encrypted temperature data. Recieves data from IoT-unit, checks write permission of unit through policy,
        // saves data to database, broadcasts through signalR
        [Route("PostTemperature")]
        [HttpPost]
        [Authorize(Policy = "CanWriteTemperature")]
        public async Task<IActionResult> PostTemperature([FromBody] string encryptedData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string decryptedJson = EncryptionModule.Decrypt(encryptedData);

            TemperaturePayloadSchema payload = JsonConvert.DeserializeObject<TemperaturePayloadSchema>(decryptedJson);

            if (payload == null)
            {
                return BadRequest("Invalid data.");
            }

            var deviceId = User.FindFirstValue("deviceId");

            if (string.IsNullOrEmpty(deviceId))
            {
                return Unauthorized("No deviceId found in token.");
            }

            TemperatureDataSchema schema = new TemperatureDataSchema
            {
                Value = payload.Value,
                DeviceId = deviceId
            };

            await _temperatureDataService.SaveTemperatureDataAsync(schema);
            await _hubContext.Clients.All.SendAsync("ReceiveTemperature", schema.Value);

            return Ok();
        }
    }
}
