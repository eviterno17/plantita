namespace plantita.ProjectPlantita.plantmanagment.domain.model.Entities;

public class PlantHealthLog
{
    public Guid HealthLogId { get; set; }
    public Guid MyPlantId { get; set; }
    public DateTime Timestamp { get; set; }
    public string HealthStatus { get; set; }
    public string Notes { get; set; }
    public string Source { get; set; }
}
