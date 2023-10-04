using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server_API.Helpers.Services;
using Server_API.Models.Schemas;

namespace Server_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AccountService _accountService;

        public UsersController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserSchema schema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.RegisterAsync(schema);

            if (result)
            {
                return Ok(); // eller någon annan lämplig respons
            }

            // Hantera fel som uppstod under skapandet av användaren
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginSchema schema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _accountService.LoginAsync(schema);
            if (token == null) 
                return BadRequest("Invalid login attempt");

            return Ok(new { Token = token });
        }

        [Authorize]
        [Route("LogOut")]
        [HttpPost]
        public async Task<IActionResult> LogOutAsync()
        {
            await _accountService.LogOutAsync();
            return Ok();
        }
    }

}
