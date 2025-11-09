    using System.Net.Mime;
    using Microsoft.AspNetCore.Mvc;
    using plantita.ProjectPlantita.plantmanagment.domain.Services;
    using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
    using plantita.ProjectPlantita.plantmanagment.Interfaces.Transform;

    namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Controllers;

    [ApiController]
    [Route("plantita/v1/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class PlantController(
        IPlantCommandService plantCommandService,
        IPlantQueryService plantQueryService,
        IPlantIdentificationService plantIdentificationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plants = await plantQueryService.GetAllPlantsAsync();
            var resources = plants.Select(PlantTransform.ToResource);
            return Ok(resources);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var plant = await plantQueryService.GetByIdAsync(id);
            return plant is null ? NotFound() : Ok(PlantTransform.ToResource(plant));
        }

        [HttpGet("common/{name}")]
        public async Task<IActionResult> GetByCommonName(string name)
        {
            var plant = await plantQueryService.GetByCommonNameAsync(name);
            return plant is null ? NotFound() : Ok(PlantTransform.ToResource(plant));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavePlantResource resource)
        {
            var model = PlantTransform.ToModel(resource);
            var created = await plantCommandService.RegisterPlantAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.PlantId }, PlantTransform.ToResource(created));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SavePlantResource resource)
        {
            var model = PlantTransform.ToModel(resource);
            var updated = await plantCommandService.UpdatePlantAsync(id, model);
            return Ok(PlantTransform.ToResource(updated));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await plantCommandService.DeletePlantAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        
        [HttpPost("identify-and-save")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> IdentifyAndSavePlant([FromForm] PlantImageUploadResource form)
        {
            if (form.Image == null || form.Image.Length == 0)
                return BadRequest("No se ha proporcionado una imagen válida.");

            var result = await plantCommandService.IdentifyAndRegisterPlantAsync(form.Image);
            return result == null
                ? BadRequest("No se pudo identificar ni guardar la planta.")
                : Ok(PlantTransform.ToResource(result));
        }
    }
