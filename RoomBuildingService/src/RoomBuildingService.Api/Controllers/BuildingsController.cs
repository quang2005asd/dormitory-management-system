namespace RoomBuildingService.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Api.DTOs.Building;
using RoomBuildingService.Core.Entities;
using RoomBuildingService.Core.Interfaces;
[ApiController]
[Route("api/[controller]")]
public class BuildingsController(IBuildingRepository repo) : ControllerBase
{
[HttpGet]
public async Task<IActionResult> GetAll()
{
var buildings = await repo.GetAllAsync();
var result = buildings.Select(b => new BuildingResponse
{
Id          = b.Id,
Name        = b.Name,
TotalFloors = b.TotalFloors,
GenderType  = b.GenderType,
Status      = b.Status,
Description = b.Description,
CreatedAt   = b.CreatedAt,
UpdatedAt   = b.UpdatedAt
});
return Ok(result);
}
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    var b = await repo.GetByIdAsync(id);
    if (b is null) return NotFound();
    return Ok(new BuildingResponse
    {
        Id          = b.Id,
        Name        = b.Name,
        TotalFloors = b.TotalFloors,
        GenderType  = b.GenderType,
        Status      = b.Status,
        Description = b.Description,
        CreatedAt   = b.CreatedAt,
        UpdatedAt   = b.UpdatedAt
    });
}

[HttpPost]
public async Task<IActionResult> Create([FromBody] BuildingCreateRequest req)
{
    var building = new Building
    {
        Name        = req.Name,
        TotalFloors = req.TotalFloors,
        GenderType  = req.GenderType,
        Description = req.Description
    };
    var created = await repo.CreateAsync(building);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}

[HttpPut("{id}")]
public async Task<IActionResult> Update(Guid id, [FromBody] BuildingUpdateRequest req)
{
    var building = new Building
    {
        Id          = id,
        Name        = req.Name,
        TotalFloors = req.TotalFloors,
        GenderType  = req.GenderType,
        Status      = req.Status,
        Description = req.Description
    };
    var updated = await repo.UpdateAsync(building);
    return Ok(updated);
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(Guid id)
{
    await repo.DeleteAsync(id);
    return NoContent();
}
}