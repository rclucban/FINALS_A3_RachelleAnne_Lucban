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
        [StringLength(10)] // 10 characters ay sapat para sa "Savings" (7 characters)
        // Ito ang magiging basehan ng iyong Report logic (Values: "Income", "Expense", "Savings")
        public string Type { get; set; } = "Expense";

        // Navigation property (Opsyonal, pero madalas kailangan kung may Transaction model)
        // public virtual ICollection<Transaction> Transactions { get; set; }
    }
}