using Microsoft.AspNetCore.Mvc;
using Server_API.Contexts;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System;
using Server_API.Models.Entities;
using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Jwt;

namespace Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;
        private readonly JwtToken _jwtToken;

        public UnitController( JwtToken jwtToken, IUnitService unitService)
        {
            _jwtToken = jwtToken;
            _unitService = unitService;
        }

        // API endpoint to register a new device unit. Registers new IoT-unit, generates device specific JWT-token
        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice()
        {
            var unit = await _unitService.RegisterNewUnitAsync();

            var token = _jwtToken.GenerateDeviceToken(unit.UnitId.ToString(), unit.DeviceId);

            return Ok(token);
        }
    }
}
