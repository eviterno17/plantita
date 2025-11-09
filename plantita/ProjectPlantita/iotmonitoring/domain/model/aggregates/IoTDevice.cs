using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

namespace plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;

public class IoTDevice
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

    public List<Sensor> Sensors { get; set; }
    public MyPlant MyPlant { get; set; }
}



