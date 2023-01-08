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

            modelBuilder.Entity<Publication>()
                .HasOne(publication => publication.Creator)
                .WithMany(user => user.PublicationsPosted)
                .HasForeignKey(publication => publication.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Publication>()
                .HasOne(publication => publication.Group)
                .WithMany(group => group.Publications)
                .HasForeignKey(publication => publication.GroupId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GalleryImage>()
                .HasOne(image => image.Publication)
                .WithMany(publication => publication.Gallery)
                .HasForeignKey(image => image.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .Property(x=>x.GroupBackground)
                .IsRequired(false);

            modelBuilder.Entity<Group>()
                .Property(x => x.GroupPicture)
                .IsRequired(false);

            modelBuilder.Entity<Publication>()
                .Property(publication => publication.ImageLink)
                .IsRequired(false);

            modelBuilder.Entity<Publication>()
                .Property(publication => publication.VideoLink)
                .IsRequired(false);

            modelBuilder.Entity<Publication>()
                .Property(publication => publication.OutsideLink)
                .IsRequired(false);

            modelBuilder.Entity<Publication>()
                .Property(publication => publication.PublicationText)
                .IsRequired(false);

            modelBuilder.Entity<Publication>()
                .Property(publication => publication.EmbeddedVideoLink)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Groups_Members> Groups_Members { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
    }
}