namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources
{
    public class SaveIoTDeviceResource
    {
        public string DeviceName { get; set; }
        public Guid MyPlantId { get; set; }
        public string ConnectionType { get; set; }
        public string Location { get; set; }
        public DateTime ActivatedAt { get; set; }
        public string Status { get; set; }
        public string FirmwareVersion { get; set; }
    }
}
