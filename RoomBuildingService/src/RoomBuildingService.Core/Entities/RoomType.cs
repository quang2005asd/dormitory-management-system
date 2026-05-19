namespace RoomBuildingService.Core.Entities;
    public class RoomType
    {
        public Guid        Id          { get; set; }
        public string      TypeName    { get; set; } = null!;  // 'Phòng 4 người'
        public int         Capacity    { get; set; }
        public decimal     BasePrice   { get; set; }
        public string?     Description { get; set; }
        public DateTime    CreatedAt   { get; set; }
        public DateTime?   UpdatedAt   { get; set; }
        // Navigation
        public ICollection<RoomAmenity> Amenities { get; set; } = new List<RoomAmenity>();
        public ICollection<Room>        Rooms     { get; set; } = new List<Room>();
        
    }   