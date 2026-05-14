namespace RoomBuildingService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Core.Entities;
using RoomBuildingService.Core.Exceptions;
using RoomBuildingService.Core.Interfaces;
public class RoomRepository(AppDbContext db) : IRoomRepository
{
public async Task<IEnumerable<Room>> GetAllAsync(Guid? buildingId, int? floor, string? status)
{
var query = db.Rooms.AsNoTracking()
.Include(r => r.RoomType)
.AsQueryable();
    if (buildingId.HasValue) query = query.Where(r => r.BuildingId == buildingId);
    if (floor.HasValue)      query = query.Where(r => r.FloorNumber == floor);
    if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);

    return await query.ToListAsync();
}

public async Task<IEnumerable<Room>> GetByFloorAsync(Guid buildingId, int floor)
    => await db.Rooms.AsNoTracking()
               .Include(r => r.RoomType)
               .Where(r => r.BuildingId == buildingId && r.FloorNumber == floor)
               .OrderBy(r => r.RoomNumber)
               .ToListAsync();

public async Task<Room?> GetByIdAsync(Guid id)
    => await db.Rooms.AsNoTracking()
               .Include(r => r.RoomType).ThenInclude(rt => rt.Amenities)
               .Include(r => r.Beds)
               .Include(r => r.Equipments)
               .FirstOrDefaultAsync(r => r.Id == id);

public async Task<Room> CreateAsync(Room room)
{
    // Kiểm tra tòa nhà tồn tại
    var building = await db.Buildings.FindAsync(room.BuildingId)
        ?? throw new NotFoundException("Building", room.BuildingId);

    // Kiểm tra số tầng hợp lệ
    if (room.FloorNumber > building.TotalFloors)
        throw new BusinessRuleException(
            $"FloorNumber ({room.FloorNumber}) vượt quá TotalFloors ({building.TotalFloors}).");

    // Kiểm tra loại phòng tồn tại
    if (!await db.RoomTypes.AnyAsync(rt => rt.Id == room.RoomTypeId))
        throw new NotFoundException("RoomType", room.RoomTypeId);

    room.Id        = Guid.NewGuid();
    room.CreatedAt = DateTime.UtcNow;
    room.Status    = "AVAILABLE";

    db.Rooms.Add(room);
    await db.SaveChangesAsync();
    return room;
}

public async Task<Room> UpdateAsync(Room room)
{
    var existing = await db.Rooms.FindAsync(room.Id)
        ?? throw new NotFoundException("Room", room.Id);

    // Kiểm tra số tầng hợp lệ nếu thay đổi
    if (room.FloorNumber != existing.FloorNumber)
    {
        var building = await db.Buildings.FindAsync(existing.BuildingId);
        if (room.FloorNumber > building!.TotalFloors)
            throw new BusinessRuleException(
                $"FloorNumber ({room.FloorNumber}) vượt quá TotalFloors ({building.TotalFloors}).");
    }

    existing.RoomNumber  = room.RoomNumber;
    existing.RoomTypeId  = room.RoomTypeId;
    existing.FloorNumber = room.FloorNumber;
    existing.UpdatedAt   = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return existing;
}

public async Task UpdateStatusAsync(Guid id, string status, string? maintenanceReason)
{
    var existing = await db.Rooms.FindAsync(id)
        ?? throw new NotFoundException("Room", id);

    existing.Status            = status;
    existing.MaintenanceReason = maintenanceReason;
    existing.UpdatedAt         = DateTime.UtcNow;

    await db.SaveChangesAsync();
}

public async Task DeleteAsync(Guid id)
{
    var existing = await db.Rooms.FindAsync(id)
        ?? throw new NotFoundException("Room", id);

    if (existing.CurrentOccupancy > 0)
        throw new BusinessRuleException("Không thể xóa phòng đang có sinh viên ở.");

    db.Rooms.Remove(existing);
    await db.SaveChangesAsync();
}

public async Task<bool> ExistsAsync(Guid id)
    => await db.Rooms.AnyAsync(r => r.Id == id);
}