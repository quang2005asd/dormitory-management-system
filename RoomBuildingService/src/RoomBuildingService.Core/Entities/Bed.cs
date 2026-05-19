namespace RoomBuildingService.Core.Entities;
public class Bed
    {
        public Guid      Id        { get; set; }
        public Guid      RoomId    { get; set; }
        public string    BedNumber { get; set; } = null!;
        public string    Status    { get; set; } = "AVAILABLE";  // AVAILABLE | OCCUPIED | UNDER_MAINTENANCE | INACTIVE
        public DateTime  CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Navigation
        public Room Room { get; set; } = null!;
    }