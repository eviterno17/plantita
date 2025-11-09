using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;

namespace plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;

public class Sensor
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

    public SensorConfig Configuration { get; set; }
    public IoTDevice Device { get; set; }

    public List<SensorReading> Readings { get; set; }
}
