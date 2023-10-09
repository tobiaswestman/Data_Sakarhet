using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTUnit_Console.Models
{
    internal class TemperatureDataSchema
    {
        public double Value { get; set; }
        public Guid UnitId { get; set; }
    }
}
