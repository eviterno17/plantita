namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources
{
    public class IoTDeviceResource
    {
        public Guid DeviceId { get; set; }
        public Guid AuthUserId { get; set; }
        public Guid MyPlantId { get; set; }
        public string DeviceName { get; set; }
        public string ConnectionType { get; set; }
        public string Location { get; set; }
        public DateTime ActivatedAt { get; set; }
        public string Status { get; set; }
        public string FirmwareVersion { get; set; }
        
    }
}
