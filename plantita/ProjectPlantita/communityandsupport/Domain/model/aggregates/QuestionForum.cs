using plantita.ProjectPlantita.communityandsupport.Domain.model.Entities;

namespace plantita.ProjectPlantita.communityandsupport.Domain.model.aggregates;

public class QuestionForum
{
    public Guid QuestionId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Category { get; set; }

    public List<AnswerForum> Answers { get; set; }
}

