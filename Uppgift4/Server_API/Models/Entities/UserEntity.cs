using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server_API.Models.Entities
{
    public class UserEntity
    {
        [Key, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

    }
}
