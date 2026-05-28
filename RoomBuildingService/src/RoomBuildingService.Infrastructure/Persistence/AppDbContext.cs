
namespace RoomBuildingService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Core.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Building>      Buildings      => Set<Building>();
    public DbSet<RoomType>      RoomTypes      => Set<RoomType>();
    public DbSet<RoomAmenity>   RoomAmenities  => Set<RoomAmenity>();
    public DbSet<Room>          Rooms          => Set<Room>();
    public DbSet<Bed>           Beds           => Set<Bed>();
    public DbSet<RoomEquipment> RoomEquipments => Set<RoomEquipment>();
    public DbSet<OutboxEvent>   OutboxEvents   => Set<OutboxEvent>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ── BUILDING ────────────────────────────────────────────
        mb.Entity<Building>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.Property(x => x.GenderType).HasMaxLength(20).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("ACTIVE");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasCheckConstraint("CHK_Building_GenderType",
                "GenderType IN ('MALE', 'FEMALE', 'MIXED')");
            e.HasCheckConstraint("CHK_Building_Status",
                "Status IN ('ACTIVE', 'INACTIVE', 'UNDER_MAINTENANCE')");
            e.HasCheckConstraint("CHK_Building_TotalFloors",
                "TotalFloors > 0");
        });

        // ── ROOM TYPE ───────────────────────────────────────────
        mb.Entity<RoomType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.TypeName).HasMaxLength(50).IsRequired();
            e.Property(x => x.BasePrice).HasColumnType("DECIMAL(18,2)");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasIndex(x => x.TypeName).IsUnique();
            e.HasCheckConstraint("CHK_RoomType_Capacity",   "Capacity > 0");
            e.HasCheckConstraint("CHK_RoomType_BasePrice",  "BasePrice >= 0");
        });

        // ── ROOM AMENITY ────────────────────────────────────────
        mb.Entity<RoomAmenity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.AmenityName).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasIndex(x => new { x.RoomTypeId, x.AmenityName }).IsUnique();

            e.HasOne(x => x.RoomType)
             .WithMany(x => x.Amenities)
             .HasForeignKey(x => x.RoomTypeId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ROOM ────────────────────────────────────────────────
        mb.Entity<Room>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("AVAILABLE");
            e.Property(x => x.CurrentOccupancy).HasDefaultValue(0);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasIndex(x => new { x.BuildingId, x.RoomNumber }).IsUnique();
            e.HasIndex(x => new { x.BuildingId, x.Status });
            e.HasIndex(x => new { x.BuildingId, x.FloorNumber });

            e.HasCheckConstraint("CHK_Room_Status",
                "Status IN ('AVAILABLE', 'FULL', 'UNDER_MAINTENANCE', 'INACTIVE')");
            e.HasCheckConstraint("CHK_Room_FloorNumber",    "FloorNumber > 0");
            e.HasCheckConstraint("CHK_Room_Occupancy",      "CurrentOccupancy >= 0");

            e.HasOne(x => x.Building)
             .WithMany(x => x.Rooms)
             .HasForeignKey(x => x.BuildingId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.RoomType)
             .WithMany(x => x.Rooms)
             .HasForeignKey(x => x.RoomTypeId)
             .OnDelete(DeleteBehavior.Restrict);  // Không xóa RoomType khi còn Room dùng
        });

        // ── BED ─────────────────────────────────────────────────
        mb.Entity<Bed>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.BedNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("AVAILABLE");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasIndex(x => new { x.RoomId, x.BedNumber }).IsUnique();
            e.HasIndex(x => new { x.RoomId, x.Status });

            e.HasCheckConstraint("CHK_Bed_Status",
                "Status IN ('AVAILABLE', 'OCCUPIED', 'UNDER_MAINTENANCE', 'INACTIVE')");

            e.HasOne(x => x.Room)
             .WithMany(x => x.Beds)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ROOM EQUIPMENT ──────────────────────────────────────
        mb.Entity<RoomEquipment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.EquipmentName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Status).HasMaxLength(30).HasDefaultValue("ACTIVE");
            e.Property(x => x.EquipmentIndex).HasDefaultValue((short)1);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasIndex(x => new { x.RoomId, x.EquipmentName, x.EquipmentIndex }).IsUnique();
            e.HasIndex(x => new { x.RoomId, x.Status });

            e.HasCheckConstraint("CHK_Equipment_Status",
                "Status IN ('ACTIVE', 'UNDER_MAINTENANCE', 'BROKEN', 'RETIRED')");
            e.HasCheckConstraint("CHK_Equipment_Index", "EquipmentIndex > 0");

            e.HasOne(x => x.Room)
             .WithMany(x => x.Equipments)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── OUTBOX EVENT ────────────────────────────────────────
        mb.Entity<OutboxEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            e.Property(x => x.EventType).HasMaxLength(100).IsRequired();
            e.Property(x => x.Payload).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("PENDING");
            e.Property(x => x.RetryCount).HasDefaultValue((short)0);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            e.HasCheckConstraint("CHK_Outbox_Status",
                "Status IN ('PENDING', 'SENT', 'FAILED')");

            // Filtered index — chỉ index các event chưa gửi
            e.HasIndex(x => x.Status)
             .HasFilter("Status = 'PENDING'")
             .HasDatabaseName("IX_OutboxEvents_Status");
        });
    }
}