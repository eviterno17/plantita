using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;

namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Transform;

public static class PlantTransform
{
    public static PlantResource ToResource(Plant model)
    {
        return new PlantResource
        {
            PlantId = model.PlantId,
            ScientificName = model.ScientificName,
            CommonName = model.CommonName,
            Description = model.Description,
            Watering = model.Watering,
            Sunlight = model.Sunlight,
            WikiUrl = model.WikiUrl,
            ImageUrl = model.ImageUrl
        };
    }

    public static Plant ToModel(SavePlantResource resource)
    {
        return new Plant
        {
            ScientificName = resource.ScientificName,
            CommonName = resource.CommonName,
            Description = resource.Description,
            Watering = resource.Watering,
            Sunlight = resource.Sunlight,
            WikiUrl = resource.WikiUrl,
            ImageUrl = resource.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}