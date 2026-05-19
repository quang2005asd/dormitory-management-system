namespace RoomBuildingService.Core.Entities;
public class Room
{
    public Guid      Id                { get; set; }
    public Guid      BuildingId        { get; set; }
    public Guid      RoomTypeId        { get; set; }
    public string    RoomNumber        { get; set; } = null!;
    public int       FloorNumber       { get; set; }
    public int       CurrentOccupancy  { get; set; }
    public string    Status            { get; set; } = "AVAILABLE";  // AVAILABLE | FULL | UNDER_MAINTENANCE | INACTIVE
    public string?   MaintenanceReason { get; set; }
    public DateTime  CreatedAt         { get; set; }
    public DateTime? UpdatedAt         { get; set; }
    public Building             Building   { get; set; } = null!;
    public RoomType             RoomType   { get; set; } = null!;
    public ICollection<Bed>     Beds       { get; set; } = new List<Bed>();
    public ICollection<RoomEquipment> Equipments { get; set; } = new List<RoomEquipment>();
    
}