using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        [Authorize]
        public async Task<IActionResult> Index(int page = 1)
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsIndexViewModel data = new PostsIndexViewModel();
            int pageSize = 25;
            page -= 1;
            AppUser currentUser = await _userManager.GetUserAsync(User);
            List<int> categoriesInterestedInId = _db.Interests.Where(x => x.UserId == currentUser.Id).Select(x => x.CategoryId).ToList();
            List<Publication> publications = _db.Publications.OrderByDescending(x => x.PostedTime).ToList();
            for (int i = 0; i < publications.Count; i++)
            {
                publications[i].CategoriesIds = _db.Publications_Categories
                    .Where(x => x.PublicationId == publications[i].Id)
                    .Select(x => x.CategoryId)
                    .ToList();
            }

            List<Publication> filteredPublications = new List<Publication>();
            foreach (var publication in publications)
            {
                foreach (int categoryId in publication.CategoriesIds)
                {
                    if (categoriesInterestedInId.Contains(categoryId))
                    {
                        filteredPublications.Add(publication);
                        break;
                    }
                }
            }

            List<Publication> sortedPublications = filteredPublications.Skip(page * pageSize).Take(pageSize).ToList();
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
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
            data.PrevPage = page > 0;
            data.NextPage = filteredPublications.Count() > (page + 1) * pageSize;
            data.CurrentPage = page + 1;
            data.UserId = currentUser.Id;
            List<Group> groups = GetUserGroups();
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                data.Groups.Add(group);
            }
            return View(data);
        }

        public async Task<IActionResult> Category(int id, int page = 1)
        {
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();
            PostsCategoryViewModel data = new PostsCategoryViewModel();
            int pageSize = 25;
            page -= 1;

            List<Publication> publications = _db.Publications.OrderByDescending(x => x.PostedTime).ToList();
            for (int i = 0; i < publications.Count; i++)
            {
                publications[i].CategoriesIds = _db.Publications_Categories
                    .Where(x => x.PublicationId == publications[i].Id)
                    .Select(x => x.CategoryId)
                    .ToList();
            }
            publications = publications.Where(x => x.CategoriesIds.Contains(id)).ToList();
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
            data.IsLoggedIn = User.Identity.IsAuthenticated;
            if (data.IsLoggedIn)
            {
                AppUser currentUser = await _userManager.GetUserAsync(User);
                data.UserProfilePicture = currentUser.GetProfileImage();
                data.UserName = currentUser.FirstName;
                data.IsInterestedIn = _db.Interests.Where(x => x.UserId == currentUser.Id && x.CategoryId == id).Any();
                data.UserId = currentUser.Id;
            }

            data.PrevPage = page > 0;
            data.NextPage = publications.Count() > (page + 1) * pageSize;
            data.CurrentPage = page + 1;
            data.Category = _db.Categories.Where(x => x.Id == id).FirstOrDefault();
            if (data.Category == null)
            {
                return NotFound();
            }
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
            List<int> savedPublicationsIds = _db.Saved.Where(x => x.UserId == _userManager.GetUserId(User)).Select(x => x.PublicationId).Skip(page * pageSize).Take(pageSize).ToList();
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
            data.UserId = currentUser.Id;
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
            AppUser currentUser = await _userManager.GetUserAsync(User);
            data.Group = _db.Groups.Where(x => x.Id == id).First();
            data.Group.GroupPicture = data.Group.GetGroupPicture();
            data.IsInGroup = _db.Groups_Members.Where(x => x.GroupId == id && x.MemberId == currentUser.Id).Any();
            data.HasAccess = data.IsInGroup || !data.Group.IsPrivate;
            data.PrevPage = false;
            data.NextPage = false;
            data.UserId = currentUser.Id;
            data.HasRequestedToJoin = _db.Groups_Requests.Where(x => x.GroupId == id && x.MemberId == currentUser.Id).Any();
            if (data.HasAccess)
            {
                int pageSize = 25;
                page -= 1;
                List<Publication> publications = _db.Publications.Where(x => x.GroupId == id).OrderByDescending(x => x.PostedTime).ToList();
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
                data.PrevPage = page > 0;
                data.NextPage = publications.Count() > (page + 1) * pageSize;
            }

            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
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
            data.UserId = currentUser.Id;
            data.IsTheUser = userToFind.Id == currentUser.Id;
            List<Group> groups = GetUserGroups();
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                data.Groups.Add(group);
            }
            return View(data);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            PostsCreateViewModel data = new PostsCreateViewModel();
            data.Categories = _db.Categories.ToList();
            data.UserGroups = GetUserGroups();
            return View(data);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            PostDetailsViewModel data = new PostDetailsViewModel();
            data.Post = _db.Publications.Where(x => x.Id == id).FirstOrDefault();
            if (data.Post == null)
            {
                return NotFound();
            }
            else
            {
                data.Creator = await _userManager.FindByIdAsync(data.Post.CreatorId);
            }
            return View(data);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PostsCreateViewModel data)
        {
            data.PublicationToPost.CreatorId = _userManager.GetUserId(User);
            data.PublicationToPost.PostedTime = DateTime.Today.Day.ToString();
            data.PublicationToPost.Categories = new HashSet<Publications_Categories>();
            try
            {
                foreach (var categoryId in data.PublicationToPostCategoriesIds.Where(x=>x>=0))
                {
                    Publications_Categories connection = new Publications_Categories();
                    connection.PublicationId = data.PublicationToPost.Id;
                    connection.CategoryId = categoryId;
                    data.PublicationToPost.Categories.Add(connection);
                }
                Publication publicationToAdd = data.PublicationToPost;
                _db.Publications.Add(publicationToAdd);
                if (data.PublicationToPost.HasManyImages)
                {
                    foreach (string imgUrl in data.PublicationToPostImages)
                    {
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            _db.GalleryImages.Add(new GalleryImage()
                            {
                                ImageLink = imgUrl,
                                Publication = publicationToAdd
                            });
                        }
                    }
                }
                await _db.SaveChangesAsync();
                TempData["notice"] = $"Successfully posted publication!";
                TempData["noticeBackground"] = "bg-success";
                return RedirectToAction("users");
            }
            catch (Exception)
            {
                data.Categories = _db.Categories.ToList();
                data.UserGroups = GetUserGroups();
                TempData["notice"] = $"Couldn't post publication!";
                TempData["noticeBackground"] = "bg-danger";
                return View(data);
            }
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
        public async Task<IActionResult> DeletePublication(int id)
        {
            string userId = _userManager.GetUserId(User);
            Publication publicationToDelete = _db.Publications.Where(x => x.Id == id).First();
            if (userId == publicationToDelete.CreatorId)
            {
                _db.Publications.Remove(publicationToDelete);
                await _db.SaveChangesAsync();
                TempData["notice"] = $"Successfully removed publication!";
                TempData["noticeBackground"] = "bg-success";
            }
            else
            {
                return Unauthorized();
            }
            return RedirectToAction("users", "posts");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SavePublication(int id)
        {
            string userId = _userManager.GetUserId(User);
            Users_Saved connection = new Users_Saved();
            connection.PublicationId = id;
            connection.UserId = userId;
            bool isRemoveAction = _db.Saved.Contains(connection);

            if (isRemoveAction)
            {
                _db.Saved.Remove(connection);
            }
            else
            {
                await _db.Saved.AddAsync(connection);
            }
            await _db.SaveChangesAsync();
            if (isRemoveAction)
            {
                TempData["notice"] = $"Successfully removed publication from saved!";
                TempData["noticeBackground"] = "bg-success";
            }
            else
            {
                TempData["notice"] = $"Successfully added publication to saved!";
                TempData["noticeBackground"] = "bg-success";
            }
            return RedirectToAction("index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToGroup(int groupId)
        {
            string userId = _userManager.GetUserId(User);
            if (_db.Groups.Where(x => x.Id == groupId).FirstOrDefault().IsPrivate)
            {
                Groups_Requests request = new Groups_Requests();
                request.MemberId = userId;
                request.GroupId = groupId;
                _db.Groups_Requests.Add(request);
                TempData["notice"] = $"Sent request to join group!";
                TempData["noticeBackground"] = "bg-success";
            }
            else
            {
                Groups_Members connection = new Groups_Members();
                connection.GroupId = groupId;
                connection.MemberId = userId;
                connection.JoinedDate = DateTime.Today.ToString();
                await _db.Groups_Members.AddAsync(connection);
                TempData["notice"] = $"Successfully joined group!";
                TempData["noticeBackground"] = "bg-success";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("group", new { Id=groupId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            string userId = _userManager.GetUserId(User);

            Groups_Members connection = _db.Groups_Members.Where(x=>x.GroupId==groupId&&x.MemberId==userId).First();
            _db.Groups_Members.Remove(connection);
            await _db.SaveChangesAsync();
            TempData["notice"] = $"Successfully left group!";
            TempData["noticeBackground"] = "bg-success";
            return RedirectToAction("group", new { Id = groupId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CancelJoinRequest(int groupId)
        {
            string userId = _userManager.GetUserId(User);

            Groups_Requests connection = _db.Groups_Requests.Where(x => x.GroupId == groupId && x.MemberId == userId).First();
            _db.Groups_Requests.Remove(connection);
            await _db.SaveChangesAsync();
            TempData["notice"] = $"Successfully canceled join request!";
            TempData["noticeBackground"] = "bg-success";
            return RedirectToAction("group", new { Id = groupId });
        }

        //copy to manage controller
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AcceptAddToGroup(int groupId, string userId)
        {
            Groups_Members connection = new Groups_Members();
            connection.GroupId = groupId;
            connection.MemberId = userId;
            connection.JoinedDate = DateTime.Today.ToString();
            await _db.Groups_Members.AddAsync(connection);
            await _db.SaveChangesAsync();
            return RedirectToAction("group", new { Id = groupId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromGroup(int groupId, string userId)
        {
            Groups_Members connection = new Groups_Members();
            connection.GroupId = groupId;
            connection.MemberId = userId;
            connection.JoinedDate = DateTime.Today.ToString();
            _db.Groups_Members.Remove(connection);
            await _db.SaveChangesAsync();
            return RedirectToAction("group", new { Id = groupId });
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