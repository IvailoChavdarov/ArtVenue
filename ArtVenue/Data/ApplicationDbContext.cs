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
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Messages_Sent)
                .WithOne(m => m.Sender)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Reciever)
                .WithMany(u => u.Messages_Recieved)
                .IsRequired(false);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Group)
                .WithMany(u => u.Messages)
                .IsRequired(false);

            modelBuilder.Entity<Group>()
                .HasOne(m => m.Creator)
                .WithMany(u => u.GroupsCreated);

            modelBuilder.Entity<Groups_Members>()
                .HasKey(gm => new { gm.MemberId, gm.GroupId });

            modelBuilder.Entity<Groups_Members>()
                .HasOne(user => user.Member)
                .WithMany(userGroups => userGroups.GroupsJoined)
                .HasForeignKey(gm => gm.MemberId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Groups_Members>()
                .HasOne(user => user.Group)
                .WithMany(groupMembers => groupMembers.Memberships)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Group>().Property(x=>x.GroupBackground).IsRequired(false);
            modelBuilder.Entity<Group>().Property(x => x.GroupPicture).IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}