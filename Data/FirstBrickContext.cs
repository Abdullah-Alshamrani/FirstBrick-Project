/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using Microsoft.EntityFrameworkCore; // Needed tools to work with a database in an EF Core.
using FirstBrickAPI.Models;

namespace FirstBrickAPI.Data
{
    public class FirstBrickContext : DbContext
    {
        public FirstBrickContext(DbContextOptions<FirstBrickContext> options) : base(options) { }

        // DbSet Properties
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserBalance> UserBalances { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        // Configures database schema and relationships.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table mapping to their respective databse table 
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Project>().ToTable("projects");
            modelBuilder.Entity<Investment>().ToTable("investments");
            modelBuilder.Entity<Transaction>().ToTable("transactions");
            modelBuilder.Entity<UserBalance>().ToTable("user_balances");
            modelBuilder.Entity<Portfolio>().ToTable("portfolios");

            // Column mappings !! case sensetive.
            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectId)
                .HasColumnName("ProjectId"); 

            modelBuilder.Entity<Portfolio>()
                .Property(p => p.ProjectId)
                .HasColumnName("project_id"); 

            modelBuilder.Entity<Portfolio>()
                .Property(p => p.UserId)
                .HasColumnName("user_id"); 

            modelBuilder.Entity<Portfolio>()
                .Property(p => p.Amount)
                .HasColumnName("amount"); 

            modelBuilder.Entity<Portfolio>()
                .Property(p => p.InvestedAt)
                .HasColumnName("invested_at"); 

            modelBuilder.Entity<Portfolio>()
                .Property(p => p.UpdatedAt)
                .HasColumnName("updated_at"); 

            //  relationships configiration.
            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Investments)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserBalance)
                .WithOne(ub => ub.User)
                .HasForeignKey<UserBalance>(ub => ub.UserId);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Investments)
                .WithOne(i => i.Project)
                .HasForeignKey(i => i.ProjectId);

            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.Project)
                .WithMany()
                .HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            // DateTime fields config.
            modelBuilder.Entity<Transaction>()
                .Property(t => t.CreatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Investment>()
                .Property(i => i.InvestedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Investment>()
                .Property(i => i.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Project>()
                .Property(p => p.CreatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Project>()
                .Property(p => p.UpdatedAt)
                .HasColumnType("timestamp with time zone");
        }
    }
}
