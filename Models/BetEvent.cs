using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Alexander.Models
{
    public class BetEvent
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public ulong CreatorId { get; set; }
        public bool IsClosed { get; set; } = false;
        
        public bool IsLocked { get; set; } = false; 
        
        public bool? IsWinnerAFavor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<BetTicket> Tickets { get; set; } = new();
    }
}