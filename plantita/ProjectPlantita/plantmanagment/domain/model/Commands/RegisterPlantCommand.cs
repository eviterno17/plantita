namespace plantita.ProjectPlantita.plantmanagment.domain.model.Commands;

public record RegisterPlantCommand(IFormFile Image, Guid UserId);
