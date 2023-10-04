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

        public async Task<bool> RegisterAsync(RegisterUserSchema schema)
        {
            try
            {
                // Kontrollera om roller existerar, om inte skapa dem.
                if (!await _roleManager.RoleExistsAsync("admin"))
                    await _roleManager.CreateAsync(new IdentityRole("admin"));

                if (!await _roleManager.RoleExistsAsync("user"))
                    await _roleManager.CreateAsync(new IdentityRole("user"));

                var identityResult = await _userManager.CreateAsync(new IdentityUser { Email = schema.Email, UserName = schema.Email }, schema.Password);

                if (identityResult.Succeeded)
                {
                    var identityUser = await _userManager.FindByEmailAsync(schema.Email);

                    // Om det inte finns några användare, sätt rollen till "admin". Annars, sätt den till "user".
                    var role = !await _userManager.Users.AnyAsync() ? "admin" : "user";

                    var roleResult = await _userManager.AddToRoleAsync(identityUser!, role);

                    if (roleResult.Succeeded)
                    {
                        UserEntity userEntity = new UserEntity
                        {
                            UserId = identityUser!.Id,
                            User = identityUser
                        };

                        await _accountRepo.AddAsync(userEntity);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //logga error
            }

            return false;
        }

        public async Task<string?> LoginAsync(LoginSchema schema)
        {
            var user = await _userManager.FindByEmailAsync(schema.Email);
            if (user == null) return null;

            var passwordValid = await _userManager.CheckPasswordAsync(user, schema.Password);
            if (!passwordValid) return null;

            var claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        });

            return _jwt.GenerateToken(claimsIdentity, DateTime.UtcNow.AddHours(1));
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
