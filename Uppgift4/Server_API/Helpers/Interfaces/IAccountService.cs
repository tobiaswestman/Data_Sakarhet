using Server_API.Models.Schemas;

namespace Server_API.Helpers.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterAsync(RegisterUserSchema schema);
        Task<string?> LoginAsync(LoginSchema schema);
        Task LogOutAsync();
    }
}
