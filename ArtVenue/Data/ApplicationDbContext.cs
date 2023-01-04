using ArtVenue.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Metrics;

namespace ArtVenue.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasMany(u => u.Messages_Sent).WithOne(m => m.Sender).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Message>()
            .HasOne(m => m.Reciever)
            .WithMany(u => u.Messages_Recieved)
            .IsRequired(false);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Message> Messages { get; set; }
    }
}