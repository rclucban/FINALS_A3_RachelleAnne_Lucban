// FILE: BudgetTracker/Models/Transaction.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        // ------------------ Data Properties ------------------
        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        // ------------------ Relational Properties ------------------

        // Foreign Key para sa Category
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; } // Navigation property

        // Foreign Key para sa User (IdentityUser)
        [Required]
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; } // Navigation property
    }
}