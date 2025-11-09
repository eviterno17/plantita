namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources
{
    public class SaveSensorReadingResource
    {
        public Guid SensorId { get; set; }
        public decimal Value { get; set; }
        public DateTime Timestamp { get; set; }
  
    }
}
