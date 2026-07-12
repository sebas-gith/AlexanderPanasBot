using System.ComponentModel.DataAnnotations;

namespace Alexander.Models
{
    // Definimos los tipos de acciones automáticas que Alexander puede hacer
    public enum RewardActionType
    {
        None = 0,
        GiveRole = 1
    }

    public class Reward
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public float Price { get; set; }

        public RewardActionType ActionType { get; set; } = RewardActionType.None;
        
        public string ActionData { get; set; } = string.Empty; 
    }
}