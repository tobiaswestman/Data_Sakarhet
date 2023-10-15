using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Jwt;
using Server_API.Helpers.Repositories;
using Server_API.Models.Entities;
using Server_API.Models.Schemas;
using System.Net;
using System.Security.Claims;

namespace Server_API.Helpers.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly AccountRepo _accountRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtToken _jwt;

        public AccountService(JwtToken jwt, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, AccountRepo accountRepo, IConfiguration config)
        {
            _jwt = jwt;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _accountRepo = accountRepo;
            _configuration = config;
        }

        // Registers a new user based on the provided schema.
        public async Task<bool> RegisterAsync(RegisterUserSchema schema)
        {
            try
            {
                // Check if any users are already registered.
                var isUserAlreadyPresent = await _userManager.Users.AnyAsync();

                var identityResult = await _userManager.CreateAsync(new IdentityUser { Email = schema.Email, UserName = schema.Email }, schema.Password);

                if (identityResult.Succeeded)
                {
                    var identityUser = await _userManager.FindByEmailAsync(schema.Email);

                    if (identityUser == null)
                    {
                        return false;
                    }

                    // If no users exist, set the role to "admin". Otherwise, set it to "user".
                    var role = !isUserAlreadyPresent ? "admin" : "user";

                    var roleResult = await _userManager.AddToRoleAsync(identityUser, role);

                    if (roleResult.Succeeded)
                    {
                        UserEntity userEntity = new UserEntity
                        {
                            UserId = identityUser.Id,
                            User = identityUser
                        };

                        await _accountRepo.AddAsync(userEntity);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
            }

            return false;
        }

        // Authenticates and logs in a user. Returns a JWT token upon successful login.
        public async Task<string?> LoginAsync(LoginSchema schema)
        {
            var user = await _userManager.FindByEmailAsync(schema.Email);
            if (user == null) return null;

            var passwordValid = await _userManager.CheckPasswordAsync(user, schema.Password);
            if (!passwordValid) return null;

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, userRoles.FirstOrDefault() ?? string.Empty) // Inkludera användarens roll
            };

            var claimsIdentity = new ClaimsIdentity(claims);

            return _jwt.GenerateToken(claimsIdentity, DateTime.UtcNow.AddHours(1));
        }

        // Logs out the currently authenticated user.
        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            
        }
    }
}
