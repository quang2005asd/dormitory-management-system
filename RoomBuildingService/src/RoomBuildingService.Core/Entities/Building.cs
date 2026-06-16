namespace RoomBuildingService.Core.Entities;
        public class Building
        {
            public Guid        Id          { get; set; }
            public string      Name        { get; set; } = null!;
            public int         TotalFloors { get; set; }
            public string      GenderType  { get; set; } = null!;  // MALE | FEMALE | MIXED
            public string      Status      { get; set; } = "ACTIVE";
            public string?     Description { get; set; }
            public DateTime    CreatedAt   { get; set; }
            public DateTime?   UpdatedAt   { get; set; }
            // Navigation
            public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }