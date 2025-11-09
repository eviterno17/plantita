namespace plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;

public class SensorConfig
{
    public Guid ConfigId { get; set; }
    public Guid SensorId { get; set; }
    public decimal ThresholdMin { get; set; }
    public decimal ThresholdMax { get; set; }
    public int FrequencyMinutes { get; set; }
    public bool AutoNotify { get; set; }
    public DateTime ConfiguredAt { get; set; }
}
