namespace RoomBuildingService.Api.DTOs.Room;
using RoomBuildingService.Api.DTOs.RoomType;
using RoomBuildingService.Api.DTOs.Bed;
using RoomBuildingService.Api.DTOs.Equipment;
public class RoomResponse
{
    public Guid              Id                { get; set; }
    public Guid              BuildingId        { get; set; }
    public string            RoomNumber        { get; set; } = null!;
    public int               FloorNumber       { get; set; }
    public int               CurrentOccupancy  { get; set; }
    public string            Status            { get; set; } = null!;
    public string?           MaintenanceReason { get; set; }
    public RoomTypeResponse? RoomType          { get; set; }
    public List<BedResponse> Beds              { get; set; } = new();
    public List<EquipmentResponse> Equipments  { get; set; } = new();
    public DateTime          CreatedAt         { get; set; }
    public DateTime?         UpdatedAt         { get; set; }
}