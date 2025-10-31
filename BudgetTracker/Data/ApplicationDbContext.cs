// FILE: BudgetTracker/Data/ApplicationDbContext.cs

using BudgetTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace BudgetTracker.Data
{
    // Ginagamit ang IdentityDbContext<IdentityUser> para isama ang Identity tables
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 💾 DB Sets (Tables)
        // Ang mga ito ay nagko-connect sa Category at Transaction Models mo
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
    }
}