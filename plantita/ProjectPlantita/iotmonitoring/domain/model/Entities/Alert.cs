using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

namespace plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;

public class Alert
{
    public Guid AlertId { get; set; }                  // id_alerta
    public Guid? SensorId { get; set; }                // id_sensor (nullable en DB)
    public Guid? PlantInstanceId { get; set; }         // id_mi_planta (nullable)
    public string AlertType { get; set; }             // tipo_alerta
    public string Message { get; set; }               // mensaje
    public AlertLevel? Level { get; set; }            // nivel (ENUM)
    public DateTime? GeneratedAt { get; set; }        // fecha_generada
    public bool? Seen { get; set; } = false;          // visto (TINYINT(1))

    // Relaciones de navegación
    public Sensor Sensor { get; set; }
    public MyPlant PlantInstance { get; set; }
}

public enum AlertLevel
{
    info,
    advertencia,
    crítica
}