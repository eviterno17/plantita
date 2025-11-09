

namespace plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID
{
    public class Plant
    {
        public Guid PlantId { get; set; }

        // Nombre científico de la planta (único por especie)
        public string ScientificName { get; set; }

        // Nombre común principal (puede tomar el primero de la lista de la API)
        public string CommonName { get; set; }

        // Descripción proveniente de Wikipedia u otra fuente
        public string Description { get; set; }

        // Nivel de riego recomendado (ej. "Average", "Frequent", etc.)
        public string Watering { get; set; }

        // Requerimientos de luz (ej. "Full sun, Partial shade")
        public string Sunlight { get; set; }

        // URL a fuente externa (Wikipedia o Plant.id)
        public string? WikiUrl { get; set; }

        // Imagen representativa de la especie (puede ser de Plant.id o propia)
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relaciones
        public List<MyPlant> MyPlants { get; set; }
    }
}
