using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Transform;

namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Controllers;


[Authorize]
[ApiController]
[Route("plantita/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MyPlantController : ControllerBase
{
    private readonly IMyPlantCommandService _myPlantCommandService;
    private readonly IPlantQueryService _plantQueryService;
    private readonly IMyPlantQueryService _myPlantQueryService;

    public MyPlantController(IMyPlantCommandService myPlantCommandService,IMyPlantQueryService myPlantQueryService, IPlantQueryService plantQueryService)
    {
        _myPlantCommandService = myPlantCommandService;
        _plantQueryService = plantQueryService;
        _myPlantQueryService = myPlantQueryService;
    }

    // Explicación:
// - El endpoint POST recibe solo name, location y note en el body.
// - El PlantId se recibe como parámetro en la URL.
// - Se obtiene la imagen de la planta base usando PlantId.

    // plantita/ProjectPlantita/plantmanagment/Interfaces/Controllers/MyPlantController.cs
    // MyPlantController.cs
    // MyPlantController.cs
    [HttpPost("{plantId:guid}")]
    public async Task<IActionResult> Create(Guid plantId, [FromBody] SaveMyPlantResource resource)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("No se pudo obtener el usuario autenticado.");

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("El identificador de usuario no es válido.");

        var created = await _myPlantCommandService.RegisterMyPlantAsync(userId, plantId, resource);
        return CreatedAtAction(nameof(Create), new { id = created.MyPlantId }, MyPlantTransform.ToResource(created));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = Guid.Parse(userIdClaim.Value);
        var myPlants = await _myPlantQueryService.GetAllByUserIdAsync(userId);
    
        var resources = myPlants.Select(MyPlantTransform.ToResource);
        return Ok(resources);
    }

    [HttpGet("{myPlantId:guid}")]
    public async Task<IActionResult> GetMyPlantById(Guid myPlantId)
    {
        var myPlant = await _myPlantQueryService.GetByIdAsync(myPlantId);
        if (myPlant == null) return NotFound();
        var myPlantTransform = MyPlantTransform.ToResource(myPlant);
        
        return Ok(myPlantTransform);
    }


}