namespace plantita.ProjectPlantita.plantmanagment.domain.model.Entities;

public class CareTask
{
    public Guid TaskId { get; set; }
    public Guid MyPlantId { get; set; }
    public string TaskType { get; set; }
    public DateTime ScheduledFor { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
}