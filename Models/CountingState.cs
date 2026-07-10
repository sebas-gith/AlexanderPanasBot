using System.ComponentModel.DataAnnotations;

namespace Alexander.Models
{
    public class CountingState
    {
        [Key]
        public int Id { get; set; }
        public ulong ChannelId { get; set; }
        public int CurrentCount { get; set; } = 0;
        public ulong LastUserId { get; set; } = 0;
    }
}