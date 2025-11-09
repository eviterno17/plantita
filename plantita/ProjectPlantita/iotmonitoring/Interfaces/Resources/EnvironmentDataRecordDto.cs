namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources;

public class EnvironmentDataRecordDto 
{
    public string CustomDeviceId { get; set; } // 👈 en vez de DeviceId GUID
    public decimal? Light { get; set; }
    public decimal? SoilMoisture { get; set; }
    public decimal? AirTemperature { get; set; }
    public decimal? AirHumidity { get; set; }
    public DateTime CreatedAt { get; set; }
} 