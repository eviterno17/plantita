using plantita.ProjectPlantita.diagnosisandproblems.domain.model.Entities;

namespace plantita.ProjectPlantita.diagnosisandproblems.domain.model.aggregates;

public class ProblemPlants
{
    public Guid  ProblemId { get; set; }
    public string ProblemName { get; set; }
    public string Category { get; set; }
    public string Symptoms { get; set; }
    public string Causes { get; set; }
    public string Treatment { get; set; }
    public string ReferenceImageUrl { get; set; }

    public List<Recommendation> Recommendations { get; set; }
}

