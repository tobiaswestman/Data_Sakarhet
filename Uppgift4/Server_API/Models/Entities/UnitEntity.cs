using System.ComponentModel.DataAnnotations;

namespace Server_API.Models.Entities
{
    public class UnitEntity
    {
        [Key]
        public int UnitId { get; set; }
        public string DeviceId { get; set; }
        
    }
}
