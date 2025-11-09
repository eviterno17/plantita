using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.User.Domain.Model.Aggregates;

namespace plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

public class MyPlant
{
    public Guid MyPlantId { get; set; }
    public AuthUser AuthUser { get; set; }
    public Guid UserId { get; set; }
    public Guid PlantId { get; set; }
    public string CustomName { get; set; }
    public DateTime AcquiredAt { get; set; }
    public string Location { get; set; }
    public string Note { get; set; }
    public string PhotoUrl { get; set; }
    public string CurrentStatus { get; set; }

    public List<PlantHealthLog> HealthLogs { get; set; }
    public List<CareTask> CareTasks { get; set; }
    public List<Alert> Alerts { get; set; }  // Relación con Alert
    public List<IoTDevice> IoTDevices { get; set; }


}


