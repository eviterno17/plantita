namespace plantita.ProjectPlantita.plantmanagment.domain.model.Commands;

public record LogPlantHealthStatusCommand(
    Guid MyPlantId,
    string HealthStatus,
    string Notes,
    string Source
);
