namespace RoomBuildingService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Core.Entities;
using RoomBuildingService.Core.Exceptions;
using RoomBuildingService.Core.Interfaces;
public class RoomTypeRepository(AppDbContext db) : IRoomTypeRepository
{
public async Task<IEnumerable<RoomType>> GetAllAsync()
=> await db.RoomTypes.AsNoTracking()
.Include(rt => rt.Amenities)
.ToListAsync();
public async Task<RoomType?> GetByIdAsync(Guid id)
    => await db.RoomTypes.AsNoTracking()
               .Include(rt => rt.Amenities)
               .FirstOrDefaultAsync(rt => rt.Id == id);

public async Task<RoomType> CreateAsync(RoomType roomType)
{
    // Kiểm tra trùng tên loại phòng
    if (await db.RoomTypes.AnyAsync(rt => rt.TypeName == roomType.TypeName))
        throw new BusinessRuleException($"Loại phòng '{roomType.TypeName}' đã tồn tại.");

    roomType.Id        = Guid.NewGuid();
    roomType.CreatedAt = DateTime.UtcNow;

    foreach (var amenity in roomType.Amenities)
    {
        amenity.Id         = Guid.NewGuid();
        amenity.RoomTypeId = roomType.Id;
        amenity.CreatedAt  = DateTime.UtcNow;
    }

    db.RoomTypes.Add(roomType);
    await db.SaveChangesAsync();
    return roomType;
}

public async Task<RoomType> UpdateAsync(RoomType roomType)
{
    var existing = await db.RoomTypes
                           .Include(rt => rt.Amenities)
                           .FirstOrDefaultAsync(rt => rt.Id == roomType.Id)
        ?? throw new NotFoundException("RoomType", roomType.Id);

    existing.TypeName    = roomType.TypeName;
    existing.Capacity    = roomType.Capacity;
    existing.BasePrice   = roomType.BasePrice;
    existing.Description = roomType.Description;
    existing.UpdatedAt   = DateTime.UtcNow;

    // Xóa tiện nghi cũ, thêm lại tiện nghi mới
    db.RoomAmenities.RemoveRange(existing.Amenities);
    foreach (var amenity in roomType.Amenities)
    {
        amenity.Id         = Guid.NewGuid();
        amenity.RoomTypeId = existing.Id;
        amenity.CreatedAt  = DateTime.UtcNow;
    }
    existing.Amenities = roomType.Amenities;

    await db.SaveChangesAsync();
    return existing;
}

public async Task DeleteAsync(Guid id)
{
    var existing = await db.RoomTypes.FindAsync(id)
        ?? throw new NotFoundException("RoomType", id);

    // Không xóa nếu còn phòng đang dùng loại này
    if (await db.Rooms.AnyAsync(r => r.RoomTypeId == id))
        throw new BusinessRuleException("Không thể xóa loại phòng đang được sử dụng.");

    db.RoomTypes.Remove(existing);
    await db.SaveChangesAsync();
}

public async Task<bool> ExistsAsync(Guid id)
    => await db.RoomTypes.AnyAsync(rt => rt.Id == id);
}