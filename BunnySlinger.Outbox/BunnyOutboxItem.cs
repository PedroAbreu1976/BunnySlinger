using System.ComponentModel.DataAnnotations;

namespace BunnySlinger.Outbox
{
    public class BunnyOutboxItem
    {
        [Key]
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public string Type { get; init; }
        public string Payload { get; init; }
        public string? DispatchError { get; set; }
        public int RetryCount { get; set; }
    }
}
