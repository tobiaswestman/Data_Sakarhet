using System.ComponentModel.DataAnnotations;

namespace Server_API.Models.Entities
{
    public class TemperatureDataEntity
    {
        [Key]
        public Guid Id { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid UnitId { get; set; }
        public UnitEntity Unit { get; set; }
    }
}
