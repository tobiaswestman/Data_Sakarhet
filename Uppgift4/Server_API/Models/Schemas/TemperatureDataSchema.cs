﻿namespace Server_API.Models.Schemas
{
    public class TemperatureDataSchema
    {
        public double Value { get; set; }
        public Guid UnitId { get; set; }
    }
}
