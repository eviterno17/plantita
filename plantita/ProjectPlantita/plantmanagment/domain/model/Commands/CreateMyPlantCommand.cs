namespace plantita.ProjectPlantita.plantmanagment.domain.model.Commands;

public record CreateMyPlantCommand(
    Guid UserId,
    Guid PlantId,
    string CustomName,
    DateTime AcquiredAt,
    string Location,
    string Note,
    string PhotoUrl,
    string CurrentStatus
);