namespace RoomBuildingService.Core.Entities;
public class RoomAmenity
{
    public Guid     Id          { get; set; }
    public Guid     RoomTypeId  { get; set; }
    public string   AmenityName { get; set; } = null!;  // 'Máy lạnh', 'WC riêng'...
    public DateTime CreatedAt   { get; set; }
    // Navigation
    public RoomType RoomType { get; set; } = null!;
}