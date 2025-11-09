using Microsoft.AspNetCore.Mvc;
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Controllers
{
    [ApiController]
    [Route("plantita/v1/[controller]")]
    public class SensorReadingController : ControllerBase
    {
        private readonly ISensorReadingService _sensorReadingService;
        public SensorReadingController(ISensorReadingService sensorReadingService)
        {
            _sensorReadingService = sensorReadingService;
        }

        [HttpGet]
        public async Task<IEnumerable<SensorReadingResource>> GetAll()
        {
            var readings = await _sensorReadingService.ListAsync();
            var resources = new List<SensorReadingResource>();
            foreach (var r in readings)
            {
                resources.Add(new SensorReadingResource { ReadingId = r.ReadingId, SensorId = r.SensorId, Value = r.Value, Timestamp = r.Timestamp });
            }
            return resources;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorReadingResource>> GetById(Guid id)
        {
            var reading = await _sensorReadingService.GetByIdAsync(id);
            if (reading == null) return NotFound();
            return new SensorReadingResource { ReadingId = reading.ReadingId, SensorId = reading.SensorId, Value = reading.Value, Timestamp = reading.Timestamp };
        }
        
        [HttpGet("{id}/sensorID")]
        public async Task<IEnumerable<SensorReadingResource>> GetBySensorId(Guid id)
        {
            var readings = await _sensorReadingService.GetBySensorIdAsync(id);
            var resources = new List<SensorReadingResource>();
            foreach (var r in readings)
            {
                resources.Add(new SensorReadingResource { ReadingId = r.ReadingId, SensorId = r.SensorId, Value = r.Value, Timestamp = r.Timestamp });
            }
            return resources;
        }

        [HttpPost]
        public async Task<ActionResult<SensorReadingResource>> Create([FromBody] SaveSensorReadingResource resource)
        {
            var reading = new SensorReading { SensorId = resource.SensorId, Value = resource.Value, Timestamp = resource.Timestamp };
            var result = await _sensorReadingService.CreateAsync(reading);
            return CreatedAtAction(nameof(GetById), new { id = result.ReadingId }, new SensorReadingResource { ReadingId = result.ReadingId, SensorId = result.SensorId, Value = result.Value, Timestamp = result.Timestamp });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SensorReadingResource>> Update(Guid id, [FromBody] SaveSensorReadingResource resource)
        {
            var reading = new SensorReading { SensorId = resource.SensorId, Value = resource.Value, Timestamp = resource.Timestamp };
            var result = await _sensorReadingService.UpdateAsync(id, reading);
            if (result == null) return NotFound();
            return new SensorReadingResource { ReadingId = result.ReadingId, SensorId = result.SensorId, Value = result.Value, Timestamp = result.Timestamp };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sensorReadingService.DeleteAsync(id);
            return NoContent();
        }
    }
}
