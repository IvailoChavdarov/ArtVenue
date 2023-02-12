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
            //relations between users and messages sent (1xM)
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Messages_Sent)
                .WithOne(m => m.Sender)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            //messages relations with direct and group chats (1xM)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.DirectChat)
                .WithMany(u => u.Messages)
                .IsRequired(false);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Group)
                .WithMany(u => u.Messages)
                .IsRequired(false);

            //relations between group and creator (1xM)
            modelBuilder.Entity<Group>()
                .HasOne(m => m.Creator)
                .WithMany(u => u.GroupsCreated);

            //connection table between groups and members (MxM)
            modelBuilder.Entity<Groups_Members>()
                .HasKey(gm => new { gm.MemberId, gm.GroupId });

            modelBuilder.Entity<Groups_Members>()
                .HasOne(user => user.Member)
                .WithMany(userGroups => userGroups.GroupsJoined)
                .HasForeignKey(gm => gm.MemberId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Groups_Members>()
                .HasOne(user => user.Group)
                .WithMany(groupMembers => groupMembers.Memberships)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            //connection table between groups and join requests (MxM)
            modelBuilder.Entity<Groups_Requests>()
               .HasKey(gm => new { gm.MemberId, gm.GroupId });

            modelBuilder.Entity<Groups_Requests>()
                .HasOne(user => user.Member)
                .WithMany(userGroups => userGroups.GroupsJoinRequested)
                .HasForeignKey(gm => gm.MemberId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Groups_Requests>()
                .HasOne(user => user.Group)
                .WithMany(groupMembers => groupMembers.Requests)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            //publication relations with creator and group (1xM)
            modelBuilder.Entity<Publication>()
                .HasOne(publication => publication.Creator)
                .WithMany(user => user.PublicationsPosted)
                .HasForeignKey(publication => publication.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Publication>()
                .HasOne(publication => publication.Group)
                .WithMany(group => group.Publications)
                .HasForeignKey(publication => publication.GroupId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            //publication images relation (1xM)
            modelBuilder.Entity<GalleryImage>()
                .HasOne(image => image.Publication)
                .WithMany(publication => publication.Gallery)
                .HasForeignKey(image => image.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            //connection table between publication and categories (MxM)
            modelBuilder.Entity<Publications_Categories>()
                .HasKey(pc => new { pc.PublicationId, pc.CategoryId });

            modelBuilder.Entity<Publications_Categories>()
                .HasOne(pc => pc.Publication)
                .WithMany(publication => publication.Categories)
                .HasForeignKey(gm => gm.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Publications_Categories>()
                .HasOne(pc => pc.Category)
                .WithMany(category => category.Publications)
                .HasForeignKey(gm => gm.CategoryId)
                .OnDelete(DeleteBehavior.ClientCascade);

            //connection table between user and interests(categories) (MxM)
            modelBuilder.Entity<Users_Interests>()
                .HasKey(ui => new { ui.UserId, ui.CategoryId });

            modelBuilder.Entity<Users_Interests>()
                .HasOne(ui => ui.User)
                .WithMany(user => user.Interests)
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Users_Interests>()
                .HasOne(ui => ui.Category)
                .WithMany(category => category.Interested)
                .HasForeignKey(ui => ui.CategoryId)
                .OnDelete(DeleteBehavior.ClientCascade);

            //connection table between user and publications saved (MxM)
            modelBuilder.Entity<Users_Saved>()
                .HasKey(us => new { us.UserId, us.PublicationId });

            modelBuilder.Entity<Users_Saved>()
                .HasOne(us => us.User)
                .WithMany(user => user.Saved)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Users_Saved>()
                .HasOne(us => us.Publication)
                .WithMany(publication => publication.SavedBy)
                .HasForeignKey(us => us.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            //comment relations with publication (Mx1)
            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Publication)
                .WithMany(publication => publication.Comments)
                .HasForeignKey(comment => comment.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            //connection table between users(direct chats) (MxM)
            modelBuilder.Entity<DirectChat>()
                .HasOne(us => us.FirstUser)
                .WithMany(user => user.DirectChats)
                .HasForeignKey(us => us.FirstUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DirectChat>()
                .HasOne(us => us.SecondUser)
                .WithMany(user => user.DirectChatsSecondUser)
                .HasForeignKey(us => us.SecondUserId)
                .OnDelete(DeleteBehavior.NoAction);

            //define non required properties
            modelBuilder.Entity<Group>()
                .Property(x => x.GroupBackground)
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
        public DbSet<Groups_Requests> Groups_Requests { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Users_Interests> Interests { get; set; }
        public DbSet<Publications_Categories> Publications_Categories { get; set; }
        public DbSet<Users_Saved> Saved { get; set; }
        public DbSet<DirectChat> DirectChats { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}