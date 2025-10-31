// FILE: BudgetTracker/Models/Category.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        // Tiyakin na ang Icon column ay hindi NULL (tulad ng nakita sa error mo)
        public string Icon { get; set; } = string.Empty;

        [Required]
        [StringLength(10)] // Para sa "Income" o "Expense"
        // Ito ang magiging basehan ng iyong Report logic
        public string Type { get; set; } = "Expense";
    }
}