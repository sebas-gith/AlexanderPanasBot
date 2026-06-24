using System.ComponentModel.DataAnnotations;

namespace Alexander.Models
{
    public class EconomyProfile
    {
        [Key]
        public ulong UserId { get; set; }
        public float SmashCoins { get; set; } = 1000;
        public DateTime LastDailyClaim { get; set; } = DateTime.MinValue;
        public EconomyProfile() { }

        public EconomyProfile(ulong userId, float initialCoins = 1000)
        {
            UserId = userId;
            SmashCoins = initialCoins;
        }
    }
    
}