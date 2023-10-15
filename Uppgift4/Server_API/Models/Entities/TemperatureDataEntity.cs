using System.ComponentModel.DataAnnotations;

namespace Server_API.Models.Entities
{
    public class TemperatureDataEntity
    {
        [Key]
        public Guid Id { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceId { get; set; }
    }
}
