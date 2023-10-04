using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server_API.Helpers.Hubs;
using Server_API.Helpers.Interfaces;
using Server_API.Models.Schemas;

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
        [Route("AddData")]
        [HttpPost]
        public async Task<IActionResult> PostTemperature([FromBody] TemperatureDataSchema schema)
        {
            await _temperatureDataService.SaveTemperatureDataAsync(schema);

            // Broadcast to frontend
            await _hubContext.Clients.All.SendAsync("ReceiveTemperature", schema.Value);

            return Ok();
        }
    }
}
