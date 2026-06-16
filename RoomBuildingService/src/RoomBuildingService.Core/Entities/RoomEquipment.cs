namespace RoomBuildingService.Core.Entities;
    public class RoomEquipment
    {
        public Guid      Id             { get; set; }
        public Guid      RoomId         { get; set; }
        public string    EquipmentName  { get; set; } = null!;  // 'Máy lạnh', 'Bình nóng lạnh'...
        public short     EquipmentIndex { get; set; } = 1;      // Máy lạnh số 1, số 2...
        public string    Status         { get; set; } = "ACTIVE";  // ACTIVE | UNDER_MAINTENANCE | BROKEN | RETIRED
        public DateTime  CreatedAt      { get; set; }
        public DateTime? UpdatedAt      { get; set; }
        // Navigation
        public Room Room { get; set; } = null!;
    }