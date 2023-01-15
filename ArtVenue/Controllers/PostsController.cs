using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArtVenue.Controllers
{
    public class PostsController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public PostsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsIndexViewModel data = new PostsIndexViewModel();
            foreach (var publication in _db.Publications)
            {
                if (userNames.ContainsKey(publication.CreatorId))
                {
                    publication.PostedBy = userNames[publication.CreatorId];
                }
                else
                {
                    AppUser user = await _userManager.FindByIdAsync(publication.CreatorId);
                    PostCreator createdBy = new PostCreator(user);
                    userNames.Add(publication.CreatorId, createdBy);
                    publication.PostedBy = createdBy;
                }
                if (publication.HasManyImages)
                {
                    publication.Images = _db.GalleryImages.Where(x => x.PublicationId == publication.Id).Select(x=>x.ImageLink).ToList<string>();
                }

                List<Comment> comments = _db.Comments.Where(x => x.PublicationId == publication.Id).ToList();
                foreach (Comment comment in comments)
                {
                    if (userNames.ContainsKey(comment.UserId))
                    {
                        comment.Sender = userNames[comment.UserId];
                    }
                    else
                    {
                        AppUser user = await _userManager.FindByIdAsync(comment.UserId);
                        PostCreator createdBy = new PostCreator(user);
                        userNames.Add(comment.UserId, createdBy);
                        comment.Sender = createdBy;
                    }
                    
                }
                publication.PostComments = comments.ToList();
                data.Publications.Add(publication);
            }
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> PostComment(ViewModelWithCommentInput data)
        {
            string userId = _userManager.GetUserId(User);
            Comment comment = new Comment();
            comment.UserId = userId;
            comment.CommentContent = data.CommentInput.CommentContent;
            comment.PublicationId = data.CommentInput.PostId;
            comment.PostedTime = DateTime.UtcNow.ToString();
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}