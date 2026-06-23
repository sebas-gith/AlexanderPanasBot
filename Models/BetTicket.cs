using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alexander.Models
{
    public class BetTicket
    {
        [Key]
        public int Id { get; set; }
        public int BetEventId { get; set; }
        public ulong UserId { get; set; }
        public int Amount { get; set; }
        public bool IsBetAFavor { get; set; } 
        [ForeignKey("BetEventId")]
        public BetEvent BetEvent { get; set; }
    }
}