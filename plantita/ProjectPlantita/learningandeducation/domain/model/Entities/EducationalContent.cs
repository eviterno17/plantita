namespace plantita.ProjectPlantita.learningandeducation.domain.model.Entities;

public class EducationalContent
{
    public Guid ContentId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }  // e.g., article, video, guide
    public string Level { get; set; } // basic, intermediate, advanced
    public string Url { get; set; }
    public DateTime PublishedAt { get; set; }
}