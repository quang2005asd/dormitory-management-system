namespace RoomBuildingService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Core.Entities;
using RoomBuildingService.Core.Exceptions;
using RoomBuildingService.Core.Interfaces;
public class BedRepository(AppDbContext db) : IBedRepository
{
public async Task<IEnumerable<Bed>> GetByRoomAsync(Guid roomId)
=> await db.Beds.AsNoTracking()
.Where(b => b.RoomId == roomId)
.OrderBy(b => b.BedNumber)
.ToListAsync();
public async Task<Bed?> GetByIdAsync(Guid id)
    => await db.Beds.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

public async Task<Bed> CreateAsync(Bed bed)
{
    if (!await db.Rooms.AnyAsync(r => r.Id == bed.RoomId))
        throw new NotFoundException("Room", bed.RoomId);

    // Kiểm tra trùng số giường trong phòng
    if (await db.Beds.AnyAsync(b => b.RoomId == bed.RoomId && b.BedNumber == bed.BedNumber))
        throw new BusinessRuleException($"Giường '{bed.BedNumber}' đã tồn tại trong phòng này.");

    bed.Id        = Guid.NewGuid();
    bed.CreatedAt = DateTime.UtcNow;
    bed.Status    = "AVAILABLE";

    db.Beds.Add(bed);
    await db.SaveChangesAsync();
    return bed;
}

public async Task<Bed> UpdateAsync(Bed bed)
{
    var existing = await db.Beds.FindAsync(bed.Id)
        ?? throw new NotFoundException("Bed", bed.Id);

    existing.BedNumber = bed.BedNumber;
    existing.Status    = bed.Status;
    existing.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return existing;
}

public async Task DeleteAsync(Guid id)
{
    var existing = await db.Beds.FindAsync(id)
        ?? throw new NotFoundException("Bed", id);

    if (existing.Status == "OCCUPIED")
        throw new BusinessRuleException("Không thể xóa giường đang có người ở.");

    db.Beds.Remove(existing);
    await db.SaveChangesAsync();
}

public async Task<bool> ExistsAsync(Guid id)
    => await db.Beds.AnyAsync(b => b.Id == id);
}