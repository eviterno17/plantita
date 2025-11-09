namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources
{
    public class SensorResource
    {
        public Guid SensorId { get; set; }
        public Guid DeviceId { get; set; }
        public string SensorType { get; set; }
        public string Unit { get; set; }
        public decimal RangeMin { get; set; }
        public decimal RangeMax { get; set; }
        public string Model { get; set; }
        public DateTime InstalledAt { get; set; }
        public bool IsActive { get; set; }
    }
}
