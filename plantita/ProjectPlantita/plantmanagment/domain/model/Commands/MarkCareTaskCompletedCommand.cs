namespace plantita.ProjectPlantita.plantmanagment.domain.model.Commands;

public record MarkCareTaskCompletedCommand(Guid TaskId, DateTime CompletedAt);
