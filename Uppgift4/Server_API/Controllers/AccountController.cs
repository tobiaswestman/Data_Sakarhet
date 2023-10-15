
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Services;
using Server_API.Models.Schemas;

namespace Server_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        //Dependency Injection
        private readonly IAccountService _accountService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Initializes the account service and user & role managers
        public AccountController(IAccountService accountService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Registers a new user based on the provided schema
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
                return Ok();
            }

            return BadRequest(result);
        }

        // Authenticates and logs in a user, provides a JWT token upon success
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

        // Logs out the currently authenticated user
        [Authorize]
        [Route("LogOut")]
        [HttpPost]
        public async Task<IActionResult> LogOutAsync()
        {
            await _accountService.LogOutAsync();
            return Ok();
        }

        // Verifies the validity of the provided JWT token, if call is authorized token is valid
        [Authorize]
        [HttpGet("VerifyToken")]
        public IActionResult VerifyToken()
        {
            return Ok(new { Status = "Valid" });
        }

        // Retrieves the role of the currently authenticated user
        [Authorize]
        [HttpGet("userRole")]
        public async Task<IActionResult> GetUserRole()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            var role = roles.FirstOrDefault();

            return Ok(new { role = role });
        }
    }
}

