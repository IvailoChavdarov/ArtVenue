using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsIndexViewModel data = new PostsIndexViewModel();
            int pageSize = 25;
            page -= 1;
            List<Publication> publications = _db.Publications.OrderByDescending(x=>x.PostedTime).ToList();
            List<Publication> sortedPublications = publications.Skip(page * pageSize).Take(pageSize).ToList();
            foreach (var publication in sortedPublications)
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
                publication.IsSavedByUser = _db.Saved.Where(x => x.UserId == _userManager.GetUserId(User) && x.PublicationId == publication.Id).Any();
                data.Publications.Add(publication);
            }
            AppUser currentUser = await _userManager.GetUserAsync(User);
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
            data.PrevPage = page > 0;
            data.NextPage = publications.Count() > (page+1) * pageSize;
            data.CurrentPage = page+1;
            List<Group> groups = GetUserGroups();
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                data.Groups.Add(group);
            }
            return View(data);
        }

        [Authorize]
        public async Task<IActionResult> Saved(int page = 1)
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsIndexViewModel data = new PostsIndexViewModel();
            AppUser currentUser = await _userManager.GetUserAsync(User);
            string userId = currentUser.Id;
            int pageSize = 25;
            page -= 1;
            List<int> savedPublicationsIds = _db.Saved.Where(x => x.UserId == _userManager.GetUserId(User)).Select(x=>x.PublicationId).Skip(page * pageSize).Take(pageSize).ToList();
            List<Publication> savedPublications = new List<Publication>();
            foreach (var publicationId in savedPublicationsIds)
            {
                savedPublications.Add(_db.Publications.Where(x => x.Id == publicationId).FirstOrDefault());
            }
            foreach (var publication in savedPublications)
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
                    publication.Images = _db.GalleryImages.Where(x => x.PublicationId == publication.Id).Select(x => x.ImageLink).ToList<string>();
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
                publication.IsSavedByUser = _db.Saved.Where(x => x.UserId == _userManager.GetUserId(User) && x.PublicationId == publication.Id).Any();
                data.Publications.Add(publication);
            }
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
            data.PrevPage = page > 0;
            data.NextPage = _db.Saved.Count() > (page + 1) * pageSize;
            data.CurrentPage = page + 1;
            List<Group> groups = GetUserGroups();
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                data.Groups.Add(group);
            }
            return View(data);
        }

        [Authorize]
        public async Task<IActionResult> Group(int id, int page = 1)
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsGroupViewModel data = new PostsGroupViewModel();
            int pageSize = 25;
            page -= 1;
            List<Publication> publications = _db.Publications.Where(x=>x.GroupId==id).OrderByDescending(x => x.PostedTime).ToList();
            List<Publication> sortedPublications = publications.Skip(page * pageSize).Take(pageSize).ToList();
            foreach (var publication in sortedPublications)
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
                    publication.Images = _db.GalleryImages.Where(x => x.PublicationId == publication.Id).Select(x => x.ImageLink).ToList<string>();
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
                publication.IsSavedByUser = _db.Saved.Where(x => x.UserId == _userManager.GetUserId(User) && x.PublicationId == publication.Id).Any();
                data.Publications.Add(publication);
            }
            AppUser currentUser = await _userManager.GetUserAsync(User);
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
            data.PrevPage = page > 0;
            data.NextPage = publications.Count() > (page + 1) * pageSize;
            data.CurrentPage = page + 1;
            data.Group = _db.Groups.Where(x => x.Id == id).First();
            List<Group> groups = GetUserGroups();
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                data.Groups.Add(group);
            }
            return View(data);
        }

        [Authorize]
        public async Task<IActionResult> Users(string id, int page = 1)
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsUserViewModel data = new PostsUserViewModel();
            int pageSize = 25;
            page -= 1;
            AppUser userToFind;
            if (string.IsNullOrEmpty(id))
            {
                userToFind = await _userManager.GetUserAsync(User);
            }
            else
            {
                userToFind = await _userManager.FindByIdAsync(id);
            }
            List<Publication> publications = _db.Publications.Where(x => x.CreatorId == userToFind.Id).OrderByDescending(x => x.PostedTime).ToList();
            List<Publication> sortedPublications = publications.Skip(page * pageSize).Take(pageSize).ToList();
            foreach (var publication in sortedPublications)
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
                    publication.Images = _db.GalleryImages.Where(x => x.PublicationId == publication.Id).Select(x => x.ImageLink).ToList<string>();
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
                publication.IsSavedByUser = _db.Saved.Where(x => x.UserId == _userManager.GetUserId(User) && x.PublicationId == publication.Id).Any();
                data.Publications.Add(publication);
            }
            AppUser currentUser = await _userManager.GetUserAsync(User);
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
            data.PrevPage = page > 0;
            data.NextPage = publications.Count() > (page + 1) * pageSize;
            data.CurrentPage = page + 1;
            userToFind.ProfileImage = userToFind.GetProfileImage();
            data.User = userToFind;
            data.IsTheUser = userToFind.Id == currentUser.Id;
            List<Group> groups = GetUserGroups();
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                data.Groups.Add(group);
            }
            return View(data);
        }

        [HttpPost]
        [Authorize]
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SavePublication(int id)
        {
            string userId = _userManager.GetUserId(User);
            Users_Saved connection = new Users_Saved();
            connection.PublicationId = id;
            connection.UserId = userId;
            if (_db.Saved.Contains(connection))
            {
                _db.Saved.Remove(connection);
            }
            else
            {
                await _db.Saved.AddAsync(connection);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private List<Group> GetUserGroups()
        {
            string userId = _userManager.GetUserId(User);
            List<Group> groups = new List<Group>();
            List<Groups_Members> memberships = _db.Groups_Members.Where(x => x.MemberId == userId).ToList();
            foreach (var membership in memberships)
            {
                groups.Add(_db.Groups.Where(x => x.Id == membership.GroupId).First());
            }
            return groups;
        }
    }
}