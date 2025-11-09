namespace plantita.ProjectPlantita.plantmanagment.domain.model.Commands;

public record UpdateMyPlantCommand(
    Guid MyPlantId,
    string CustomName,
    string Location,
    string Note,
    string CurrentStatus,
    string PhotoUrl
);
