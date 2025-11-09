namespace plantita.ProjectPlantita.learningandeducation.domain.model.Entities;

public class UserContentProgress
{
    public Guid UserId { get; set; }
    public Guid ContentId { get; set; }
    public DateTime ViewedAt { get; set; }
    public int ProgressPercent { get; set; }
}