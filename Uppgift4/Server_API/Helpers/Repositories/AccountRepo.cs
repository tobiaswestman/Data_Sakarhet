using Server_API.Contexts;
using Server_API.Helpers.Repositories.BaseModels;
using Server_API.Models.Entities;

namespace Server_API.Helpers.Repositories
{
    public class AccountRepo : Repo<UserEntity>
    {
        public AccountRepo(DataContext context) : base(context)
        {
        }
    }
}
