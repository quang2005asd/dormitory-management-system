namespace RoomBuildingService.Core.Entities;
    public class OutboxEvent
        {
            public Guid      Id          { get; set; }
            public string    EventType   { get; set; } = null!;  // 'room.status.changed'
            public Guid      AggregateId { get; set; }           // RoomId
            public string    Payload     { get; set; } = null!;  // JSON string
            public string    Status      { get; set; } = "PENDING";  // PENDING | SENT | FAILED
            public short     RetryCount  { get; set; }
            public DateTime  CreatedAt   { get; set; }
            public DateTime? SentAt      { get; set; }
        }