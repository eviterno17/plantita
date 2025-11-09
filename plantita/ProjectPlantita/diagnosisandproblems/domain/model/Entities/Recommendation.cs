namespace plantita.ProjectPlantita.diagnosisandproblems.domain.model.Entities;

public class Recommendation
{
    public Guid RecommendationId { get; set; }
    public Guid ProblemId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
}