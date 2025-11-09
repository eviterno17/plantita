namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;

public class SaveMyPlantResource
{
    //public IFormFile Photo { get; set; } = default!;
    //public Guid PlantId { get; set; }
    public string CustomName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    //public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    //public string CurrentStatus { get; set; } = "Healthy"; // por defecto
}