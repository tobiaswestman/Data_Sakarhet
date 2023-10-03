using System.ComponentModel.DataAnnotations;

namespace Server_API.Models.Entities
{
    public class UnitEntity
    {
        [Key]
        public Guid UnitId { get; set; }
        public string UnitName { get; set; }
        
    }
}
