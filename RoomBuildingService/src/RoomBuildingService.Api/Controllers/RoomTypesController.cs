namespace RoomBuildingService.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Api.DTOs.RoomType;
using RoomBuildingService.Core.Entities;
using RoomBuildingService.Core.Interfaces;
[ApiController]
[Route("api/[controller]")]
public class RoomTypesController(IRoomTypeRepository repo) : ControllerBase
{
[HttpGet]
public async Task<IActionResult> GetAll()
{
var types = await repo.GetAllAsync();
var result = types.Select(rt => new RoomTypeResponse
{
Id          = rt.Id,
TypeName    = rt.TypeName,
Capacity    = rt.Capacity,
BasePrice   = rt.BasePrice,
Description = rt.Description,
Amenities   = rt.Amenities.Select(a => a.AmenityName).ToList(),
CreatedAt   = rt.CreatedAt,
UpdatedAt   = rt.UpdatedAt
});
return Ok(result);
}
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    var rt = await repo.GetByIdAsync(id);
    if (rt is null) return NotFound();
    return Ok(new RoomTypeResponse
    {
        Id          = rt.Id,
        TypeName    = rt.TypeName,
        Capacity    = rt.Capacity,
        BasePrice   = rt.BasePrice,
        Description = rt.Description,
        Amenities   = rt.Amenities.Select(a => a.AmenityName).ToList(),
        CreatedAt   = rt.CreatedAt,
        UpdatedAt   = rt.UpdatedAt
    });
}

[HttpPost]
public async Task<IActionResult> Create([FromBody] RoomTypeCreateRequest req)
{
    var roomType = new RoomType
    {
        TypeName    = req.TypeName,
        Capacity    = req.Capacity,
        BasePrice   = req.BasePrice,
        Description = req.Description,
        Amenities   = req.Amenities
                         .Select(a => new RoomAmenity { AmenityName = a })
                         .ToList()
    };
    var created = await repo.CreateAsync(roomType);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}

[HttpPut("{id}")]
public async Task<IActionResult> Update(Guid id, [FromBody] RoomTypeUpdateRequest req)
{
    var roomType = new RoomType
    {
        Id          = id,
        TypeName    = req.TypeName,
        Capacity    = req.Capacity,
        BasePrice   = req.BasePrice,
        Description = req.Description,
        Amenities   = req.Amenities
                         .Select(a => new RoomAmenity { AmenityName = a })
                         .ToList()
    };
    var updated = await repo.UpdateAsync(roomType);
    return Ok(updated);
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(Guid id)
{
    await repo.DeleteAsync(id);
    return NoContent();
}
}