namespace RoomBuildingService.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Api.DTOs.Room;
using RoomBuildingService.Api.DTOs.RoomType;
using RoomBuildingService.Api.DTOs.Bed;
using RoomBuildingService.Api.DTOs.Equipment;
using RoomBuildingService.Core.Entities;
using RoomBuildingService.Core.Exceptions;
using RoomBuildingService.Core.Interfaces;
[ApiController]
[Route("api/[controller]")]
public class RoomsController(IRoomRepository repo) : ControllerBase
{
// GET /api/rooms?buildingId=...&floor=2&status=AVAILABLE
[HttpGet]
public async Task<IActionResult> GetAll(
[FromQuery] Guid?   buildingId,
[FromQuery] int?    floor,
[FromQuery] string? status)
{
var rooms = await repo.GetAllAsync(buildingId, floor, status);
return Ok(rooms.Select(MapToResponse));
}
// GET /api/rooms/{id}
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    var room = await repo.GetByIdAsync(id);
    if (room is null) return NotFound();
    return Ok(MapToResponse(room));
}

// GET /api/rooms/floormap?buildingId=...&floor=2
[HttpGet("floormap")]
public async Task<IActionResult> GetFloorMap(
    [FromQuery] Guid buildingId,
    [FromQuery] int  floor)
{
    var rooms = await repo.GetByFloorAsync(buildingId, floor);
    return Ok(rooms.Select(MapToResponse));
}

// POST /api/rooms
[HttpPost]
public async Task<IActionResult> Create([FromBody] RoomCreateRequest req)
{
    var room = new Room
    {
        BuildingId  = req.BuildingId,
        RoomTypeId  = req.RoomTypeId,
        RoomNumber  = req.RoomNumber,
        FloorNumber = req.FloorNumber
    };
    var created = await repo.CreateAsync(room);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}

// PUT /api/rooms/{id}
[HttpPut("{id}")]
public async Task<IActionResult> Update(Guid id, [FromBody] RoomUpdateRequest req)
{
    var room = new Room
    {
        Id          = id,
        RoomTypeId  = req.RoomTypeId,
        RoomNumber  = req.RoomNumber,
        FloorNumber = req.FloorNumber
    };
    var updated = await repo.UpdateAsync(room);
    return Ok(MapToResponse(updated));
}

// PATCH /api/rooms/{id}/status
[HttpPatch("{id}/status")]
public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] RoomStatusUpdateRequest req)
{
    // Bắt buộc có lý do khi chuyển sang UNDER_MAINTENANCE
    if (req.Status == "UNDER_MAINTENANCE" && string.IsNullOrWhiteSpace(req.MaintenanceReason))
        throw new BusinessRuleException("Vui lòng nhập lý do khi chuyển phòng sang trạng thái bảo trì.");

    await repo.UpdateStatusAsync(id, req.Status, req.MaintenanceReason);
    return NoContent();
}

// DELETE /api/rooms/{id}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(Guid id)
{
    await repo.DeleteAsync(id);
    return NoContent();
}

// ── Helper map entity → response ───────────────────────────
private static RoomResponse MapToResponse(Room r) => new()
{
    Id               = r.Id,
    BuildingId       = r.BuildingId,
    RoomNumber       = r.RoomNumber,
    FloorNumber      = r.FloorNumber,
    CurrentOccupancy = r.CurrentOccupancy,
    Status           = r.Status,
    MaintenanceReason = r.MaintenanceReason,
    CreatedAt        = r.CreatedAt,
    UpdatedAt        = r.UpdatedAt,
    RoomType = r.RoomType is null ? null : new RoomTypeResponse
    {
        Id        = r.RoomType.Id,
        TypeName  = r.RoomType.TypeName,
        Capacity  = r.RoomType.Capacity,
        BasePrice = r.RoomType.BasePrice,
        Amenities = r.RoomType.Amenities?.Select(a => a.AmenityName).ToList() ?? new()
    },
    Beds = r.Beds.Select(b => new BedResponse
    {
        Id        = b.Id,
        RoomId    = b.RoomId,
        BedNumber = b.BedNumber,
        Status    = b.Status,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    }).ToList(),
    Equipments = r.Equipments.Select(e => new EquipmentResponse
    {
        Id             = e.Id,
        RoomId         = e.RoomId,
        EquipmentName  = e.EquipmentName,
        EquipmentIndex = e.EquipmentIndex,
        Status         = e.Status,
        CreatedAt      = e.CreatedAt,
        UpdatedAt      = e.UpdatedAt
    }).ToList()
};
}