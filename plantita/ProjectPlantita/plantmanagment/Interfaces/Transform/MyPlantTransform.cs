using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;

namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Transform;

public static class MyPlantTransform
{
    public static MyPlantResource ToResource(MyPlant model)
    {
        return new MyPlantResource
        {
            MyPlantId = model.MyPlantId,
            UserId = model.UserId,
            PlantId = model.PlantId,
            CustomName = model.CustomName,
            AcquiredAt = model.AcquiredAt,
            Location = model.Location,
            Note = model.Note,
            PhotoUrl = model.PhotoUrl,
            CurrentStatus = model.CurrentStatus
        };
    }

    public static MyPlant ToModel(SaveMyPlantResource resource, Guid plantId,string photoUrl)
    {
        return new MyPlant
        {
            PlantId = plantId,
            CustomName = resource.CustomName,
            AcquiredAt = DateTime.UtcNow,
            Location = resource.Location,
            Note = resource.Note,
            PhotoUrl = photoUrl,
            CurrentStatus = "Healthy"
        };
    }
}