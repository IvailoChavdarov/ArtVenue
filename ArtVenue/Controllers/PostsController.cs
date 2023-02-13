using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace ArtVenue.Controllers
{
    //responsible for showing lists of publications and publications' editing and creating
    public class PostsController : Controller
    {
        //dependency injection
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;

        //sets how many publications will be displayed on a page
        private readonly int _pageSize = 25;

        public PostsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        //returns view with data for publications suggested for current user from their selected interests
        //homepage for logged user
        [Authorize]
        public async Task<IActionResult> Index(int page = 1)
        {
            PostsIndexViewModel data = new PostsIndexViewModel();

            //decrements page num by 1 to fix start counting from 1
            page -= 1;

            //gets current user data
            AppUser currentUser = await _userManager.GetUserAsync(User);

            // Get the list of category ids that the current user is interested in.
            List<int> categoriesInterestedInId = await _db.Interests
                .Where(x => x.UserId == currentUser.Id)
                .Select(x => x.CategoryId)
                .ToListAsync();

            // Get all Publications that are not associated with a group
            List<Publication> publications = await _db.Publications
                .Where(x=>x.GroupId==null)
                .OrderByDescending(x => x.PostedTime)
                .ToListAsync();

            //connects the publications with their categories
            for (int i = 0; i < publications.Count; i++)
            {
                publications[i].CategoriesIds = await _db.Publications_Categories
                    .Where(x => x.PublicationId == publications[i].Id)
                    .Select(x => x.CategoryId)
                    .ToListAsync();
            }

            //filters the publications to match the user's interests
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

            //sorts pages by items count and page num 
            List<Publication> sortedPublications = filteredPublications
                .Skip(page * _pageSize)
                .Take(_pageSize)
                .ToList();

            //gets publications and their related data
            data.Publications = await GetPublicationsData(sortedPublications);

            //sets data for sidenav and page number
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;

            data.PrevPage = page > 0;
            data.NextPage = filteredPublications.Count() > (page + 1) * _pageSize;
            data.CurrentPage = page + 1;

            data.UserId = currentUser.Id;

            //gets groups user is in
            data.Groups = await GetUserGroups();

            return View(data);
        }

        //returns view with data for  _pageSize publications of latest publications in specific category and category data by category id
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            PostsCategoryViewModel data = new PostsCategoryViewModel();

            //gets searched category
            data.Category = _db.Categories
                .Where(x => x.Id == id)
                .FirstOrDefault();

            //checks if category is valid
            if (data.Category == null)
            {
                return NotFound();
            }

            //decrements page num by 1 to fix start counting from 1
            page -= 1;

            //get all publications that are not associated with a group
            List<Publication> publications = await _db.Publications
                .Where(x => x.GroupId == null)
                .OrderByDescending(x => x.PostedTime)
                .ToListAsync();

            //connects publications with their categories
            for (int i = 0; i < publications.Count; i++)
            {
                publications[i].CategoriesIds = _db.Publications_Categories
                    .Where(x => x.PublicationId == publications[i].Id)
                    .Select(x => x.CategoryId)
                    .ToList();
            }

            //gets publications where category is the searched one
            publications = publications
                .Where(x => x.CategoriesIds
                    .Contains(id))
                .ToList();

            //sorts pages by items count and page num 
            List<Publication> sortedPublications = publications
                .Skip(page * _pageSize)
                .Take(_pageSize)
                .ToList();

            //gets publications and their related data
            data.Publications = await GetPublicationsData(sortedPublications);

            //checks if user is logged in
            data.IsLoggedIn = User.Identity.IsAuthenticated;

            //adds sidenav data and checks if user is interested in category if user is logged in
            if (data.IsLoggedIn)
            {
                AppUser currentUser = await _userManager.GetUserAsync(User);
                data.UserProfilePicture = currentUser.GetProfileImage();
                data.UserName = currentUser.FirstName;
                data.IsInterestedIn = _db.Interests
                    .Where(x => x.UserId == currentUser.Id && x.CategoryId == id)
                    .Any();
                data.UserId = currentUser.Id;
            }

            //sets page number
            data.PrevPage = page > 0;
            data.NextPage = publications.Count() > (page + 1) * _pageSize;
            data.CurrentPage = page + 1;

            //sets groups sidenav data
            data.Groups = await GetUserGroups();

            return View(data);
        }

        //returns view with data for _pageSize publications of the ones the current user has saved
        [Authorize]
        public async Task<IActionResult> Saved(int page = 1)
        {
            PostsIndexViewModel data = new PostsIndexViewModel();

            //gets current user
            AppUser currentUser = await _userManager.GetUserAsync(User);
            string userId = currentUser.Id;

            //decrements page num by 1 to fix start counting from 1
            page -= 1;

            //gets user's saved publications ids by latest adding to saved
            List<int> savedPublicationsIds = await _db.Saved
                .Where(x => x.UserId == _userManager.GetUserId(User))
                .Reverse()
                .Select(x => x.PublicationId)
                .Skip(page * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            //gets publication saved by user
            List<Publication> savedPublications = new List<Publication>();
            foreach (var publicationId in savedPublicationsIds)
            {
                savedPublications.Add(_db.Publications.Where(x => x.Id == publicationId).FirstOrDefault());
            }

            //gets publications and their related data
            data.Publications = await GetPublicationsData(savedPublications);

            //sets sidenav data
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;

            //sets page number
            data.PrevPage = page > 0;
            data.NextPage = _db.Saved.Count() > (page + 1) * _pageSize;
            data.CurrentPage = page + 1;

            data.UserId = currentUser.Id;

            //sets groups sidenav data
            data.Groups = await GetUserGroups();
            return View(data);
        }

        //returns view with data for _pageSize publications in specific group the current user has access to see and group data by group id
        [Authorize]
        public async Task<IActionResult> Group(int id, int page = 1)
        {
            PostsGroupViewModel data = new PostsGroupViewModel();

            //gets current user data
            AppUser currentUser = await _userManager.GetUserAsync(User);

            //gets group data
            data.Group = _db.Groups
                .Where(x => x.Id == id)
                .FirstOrDefault();

            //checks if group is valid
            if (data.Group == null)
            {
                return NotFound();
            }

            //gets group picture
            data.Group.GroupPicture = data.Group.GetGroupPicture();

            data.IsInGroup = _db.Groups_Members.Where(x => x.GroupId == id && x.MemberId == currentUser.Id).Any();

            //checks if user has access to group publications
            data.HasAccess = data.IsInGroup || !data.Group.IsPrivate;

            //sets initial data for pages for if user does not have access to group publications
            data.PrevPage = false;
            data.NextPage = false;

            //checks if current user has requested to join the given group
            data.HasRequestedToJoin = _db.Groups_Requests
                .Where(x => x.GroupId == id && x.MemberId == currentUser.Id)
                .Any();

            //checks if user is creator of the given group
            data.IsGroupCreator = currentUser.Id == data.Group.CreatorId;

            //gets publications data if user has access to see them
            if (data.HasAccess)
            {
                //decrements page num by 1 to fix start counting from 1
                page -= 1;

                //gets publications related to searched group
                List<Publication> publications = await _db.Publications
                    .Where(x => x.GroupId == id)
                    .OrderByDescending(x => x.PostedTime)
                    .ToListAsync();

                //gets needed publications depending on page
                List<Publication> sortedPublications = publications
                    .Skip(page * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                //gets publications and related data
                data.Publications = await GetPublicationsData(sortedPublications);

                //sets page depending on access to group
                data.PrevPage = page > 0;
                data.NextPage = publications.Count() > (page + 1) * _pageSize;
            }

            //sets groups sidenav data
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;
            data.UserId = currentUser.Id;

            //sets page number
            data.CurrentPage = page + 1;

            //sets groups sidenav data
            data.Groups = await GetUserGroups();

            return View(data);
        }

        [Authorize]
        //returns view with data for _pageSize publications of specific user and data for the user by user's id
        //and data for if the user searched is the current user (for displaying manage account link)
        public async Task<IActionResult> Users(string id, int page = 1)
        {
            PostsUserViewModel data = new PostsUserViewModel();

            //decrements page num by 1 to fix start counting from 1
            page -= 1;

            //gets user if user is valid
            AppUser userToFind;
            if (string.IsNullOrEmpty(id))
            {
                userToFind = await _userManager.GetUserAsync(User);
            }
            else
            {
                userToFind = await _userManager.FindByIdAsync(id);
            }

            //gets given user's publications
            List<Publication> publications = await _db.Publications
                .Where(x => x.CreatorId == userToFind.Id && x.GroupId==null)
                .OrderByDescending(x => x.PostedTime)
                .ToListAsync();

            //selects publications depending on page num and page item size
            List<Publication> sortedPublications = publications
                .Skip(page * _pageSize)
                .Take(_pageSize)
                .ToList();

            data.Publications = await GetPublicationsData(sortedPublications);

            //gets current user data and sets sidenav data and page number
            AppUser currentUser = await _userManager.GetUserAsync(User);
            data.UserProfilePicture = currentUser.GetProfileImage();
            data.UserName = currentUser.FirstName;


            data.PrevPage = page > 0;
            data.NextPage = publications.Count() > (page + 1) * _pageSize;
            data.CurrentPage = page + 1;

            userToFind.ProfileImage = userToFind.GetProfileImage();
            data.User = userToFind;
            data.UserId = currentUser.Id;

            //checks if current user searched for himself
            data.IsTheUser = userToFind.Id == currentUser.Id;

            data.Groups = await GetUserGroups();
            return View(data);
        }

        [Authorize]
        [HttpGet]
        //returns view with form for creating new publication
        public async Task<IActionResult> Create()
        {
            PostsCreateViewModel data = new PostsCreateViewModel();
            //gets ArtVenue art categories
            data.Categories = _db.Categories.ToList();
            //gets groups in which current user is member
            data.UserGroups = await GetUserGroups();
            return View(data);
        }

        [Authorize]
        [HttpGet]
        //returns view with data for specific publication and it's related data if the current user has access to see publication by publication id
        public async Task<IActionResult> Details(int id)
        {
            PostDetailsViewModel data = new PostDetailsViewModel();

            //gets data of searched publication
            data.Post = _db.Publications
                .Where(x => x.Id == id)
                .FirstOrDefault();

            //validates publication's existance
            if (data.Post == null)
            {
                return NotFound();
            }
            else
            {
                //checks if publication is connected to group
                if (data.Post.GroupId.HasValue)
                {
                    //checks if user has access to group and publication
                    Group group = await _db.Groups.FindAsync(data.Post.GroupId.Value);
                    if (group != null)
                    {
                        return NotFound();
                    }
                    if (!(_db.Groups_Members.Where(x => x.GroupId == group.Id && x.MemberId == _userManager.GetUserId(User)).Any() || !group.IsPrivate))
                    {
                        return Forbid();
                    }
                }

                //gets publication images if publication has multiple images
                if (data.Post.HasManyImages)
                {
                    data.Post.Images = await _db.GalleryImages
                        .Where(x=>x.PublicationId==data.Post.Id)
                        .Select(x=>x.ImageLink)
                        .ToListAsync();
                }

                //gets publication creator data
                data.Creator = await _userManager.FindByIdAsync(data.Post.CreatorId);
            }
            return View(data);
        }

        [Authorize]
        [HttpPost]
        //gets data from view and adds the new publication to the database and returns to current user's page
        public async Task<IActionResult> Create(PostsCreateViewModel data)
        {
            //sets publication creator to current user
            data.PublicationToPost.CreatorId = _userManager.GetUserId(User);

            //sets publication date to today
            data.PublicationToPost.PostedTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture);

            data.PublicationToPost.Categories = new HashSet<Publications_Categories>();

            //checks if the new publication is related to group
            if (data.PublicationToPost.GroupId < 0)
            {
                data.PublicationToPost.GroupId = null;
            }

            try
            {
                //sets publication categories
                foreach (var categoryId in data.PublicationToPostCategoriesIds.Where(x=>x>=0))
                {
                    Publications_Categories connection = new Publications_Categories();
                    connection.PublicationId = data.PublicationToPost.Id;
                    connection.CategoryId = categoryId;
                    data.PublicationToPost.Categories.Add(connection);
                }

                //adds publication to database
                Publication publicationToAdd = data.PublicationToPost;
                _db.Publications.Add(publicationToAdd);

                //adds publication images to database if publication has multiple images
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

                //saves changes
                await _db.SaveChangesAsync();

                //if the operation goes well shows success notice in the view
                TempData["notice"] = $"Successfully posted publication!";
                TempData["noticeBackground"] = "bg-success";

                return RedirectToAction("users");
            }
            catch (Exception)
            {
                //if the operation is not successful returns to page with enered data and failed notice
                data.Categories = _db.Categories.ToList();
                data.UserGroups = await GetUserGroups();

                TempData["notice"] = $"Couldn't post publication!";
                TempData["noticeBackground"] = "bg-danger";

                return View(data);
            }
        }
    
        [HttpPost]
        [Authorize]
        //post comment under specific publication and returns to suggested publication page
        public async Task<IActionResult> PostComment(ViewModelWithCommentInput data)
        {
            Comment comment = new Comment();

            //sets new comment creator to current user
            string userId = _userManager.GetUserId(User);
            comment.UserId = userId;

            //sets comment content and related publication
            comment.CommentContent = data.CommentInput.CommentContent;
            comment.PublicationId = data.CommentInput.PostId;

            //sets comment send date to today
            comment.PostedTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture);

            //adds and saves changes
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpPost]
        [Authorize]
        //deletes publication from database if current user is creator of the specific publication by publication id
        //returns to current user's page
        public async Task<IActionResult> DeletePublication(int id)
        {
            //validates if user is creator of publication and has access to delete publication
            string userId = _userManager.GetUserId(User);
            Publication publicationToDelete = await _db.Publications
                .Where(x => x.Id == id)
                .FirstAsync();

            if (userId == publicationToDelete.CreatorId)
            {
                //removes publication from database
                _db.Publications.Remove(publicationToDelete);
                await _db.SaveChangesAsync();

                //sends success notice to view
                TempData["notice"] = $"Successfully removed publication!";
                TempData["noticeBackground"] = "bg-success";
            }
            else
            {
                return Forbid();
            }

            return RedirectToAction("users", "posts");
        }

        [HttpPost]
        [Authorize]
        //add publication to user's saved by publication id and returns to user's suggested publicaitons page
        public async Task<IActionResult> SavePublication(int id)
        {
            //creates conection between current user and publication
            string userId = _userManager.GetUserId(User);
            Users_Saved connection = new Users_Saved();
            connection.PublicationId = id;
            connection.UserId = userId;

            //checks if connection exists
            bool isRemoveAction = _db.Saved.Contains(connection);
            
            if (isRemoveAction)
            {
                //removes connection from database if it already exists
                _db.Saved.Remove(connection);
            }
            else
            {
                //adds connection to database
                await _db.Saved.AddAsync(connection);
            }

            //saves changes
            await _db.SaveChangesAsync();

            //sends notice to view depending on if connection is removed or added
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
        //adds current user to group if group is public or sends join request if group is private by group id
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            string userId = _userManager.GetUserId(User);

            //checks if group is private
            if (_db.Groups.Where(x => x.Id == groupId).FirstOrDefault().IsPrivate)
            {
                //sends request to join the group if group is private
                Groups_Requests request = new Groups_Requests();
                request.MemberId = userId;
                request.GroupId = groupId;

                await _db.Groups_Requests.AddAsync(request);

                //sends success notice to view
                TempData["notice"] = $"Sent request to join group!";
                TempData["noticeBackground"] = "bg-success";
            }
            else
            {
                //adds member to group if group is public
                Groups_Members connection = new Groups_Members();
                connection.GroupId = groupId;
                connection.MemberId = userId;

                //sets join date to today
                connection.JoinedDate = DateTime.Now.ToString("dd MMMM yyyy");

                await _db.Groups_Members.AddAsync(connection);

                //sends success notice to view
                TempData["notice"] = $"Successfully joined group!";
                TempData["noticeBackground"] = "bg-success";
            }
            //saves changes to database and returns to group page
            await _db.SaveChangesAsync();

            return RedirectToAction("group", new { Id=groupId });
        }

        [Authorize]
        [HttpPost]
        //removes current user from specific group by group id
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            //gets current user id
            string userId = _userManager.GetUserId(User);

            //validates current user is member of group
            Groups_Members connection = _db.Groups_Members
                .Where(x=>x.GroupId==groupId&&x.MemberId==userId)
                .First();

            if (connection == null)
            {
                return NotFound();
            }

            //removes connection between group and member from database
            _db.Groups_Members.Remove(connection);

            //saves changes to database
            await _db.SaveChangesAsync();

            //sends success notice to view
            TempData["notice"] = $"Successfully left group!";
            TempData["noticeBackground"] = "bg-success";

            //returns to group page
            return RedirectToAction("group", new { Id = groupId });
        }

        [Authorize]
        [HttpPost]
        //removes join request of current user for specific group by group id
        public async Task<IActionResult> CancelJoinRequest(int groupId)
        {
            //gets current user id
            string userId = _userManager.GetUserId(User);

            //validates current user has sent request to join group
            Groups_Requests connection = await _db.Groups_Requests
                .Where(x => x.GroupId == groupId && x.MemberId == userId)
                .FirstOrDefaultAsync();

            if (connection == null)
            {
                return NotFound();
            }

            //removes member request from database
            _db.Groups_Requests.Remove(connection);

            //saves changes to database
            await _db.SaveChangesAsync();

            //sends success notice to view
            TempData["notice"] = $"Successfully canceled join request!";
            TempData["noticeBackground"] = "bg-success";

            //returns to group page
            return RedirectToAction("group", new { Id = groupId });
        }

        //gets data for the groups the current user has joined
        private async Task<List<Group>> GetUserGroups()
        {
            //gets current user id
            string userId = _userManager.GetUserId(User);

            List<Group> groups = new List<Group>();

            //gets current user groups
            List<Groups_Members> memberships = await _db.Groups_Members
                .Where(x => x.MemberId == userId)
                .ToListAsync();

            List<Models.Group> groupsWithImages = new List<Group>();

            //gets user 
            foreach (var membership in memberships)
            {
                groups.Add(await _db.Groups
                    .Where(x => x.Id == membership.GroupId)
                    .FirstAsync());
            }

            //connects groups and group image
            foreach (var group in groups)
            {
                group.GroupPicture = group.GetGroupPicture();
                groupsWithImages.Add(group);
            }

            return groupsWithImages;
        }

        //connects publications in the collection to their related data from the database and returns the data
        private async Task<List<Publication>> GetPublicationsData(List<Publication> publications)
        {
            //used to optimize getting the user if one user appears multiple times
            Dictionary<string, PostCreator> userNames = new Dictionary<string, PostCreator>();

            List<Publication> result = new List<Publication>();

            //connects publications with their related data
            foreach (var publication in publications)
            {
                //connects publications with their creator
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

                //connects publication with it's images if there is more than one image connected to that publication
                if (publication.HasManyImages)
                {
                    publication.Images = await _db.GalleryImages
                        .Where(x => x.PublicationId == publication.Id)
                        .Select(x => x.ImageLink)
                        .ToListAsync<string>();
                }

                //gets publication's comments
                List<Comment> comments = await _db.Comments
                    .Where(x => x.PublicationId == publication.Id)
                    .ToListAsync();

                //connects comment with comment creator
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

                //connects publication with comments
                publication.PostComments = comments.ToList();

                //checks if publication is saved by current user
                publication.IsSavedByUser = _db.Saved
                    .Any(x => x.UserId == _userManager.GetUserId(User) && x.PublicationId == publication.Id);

                //adds publication to the data sent to view
                result.Add(publication);
            }

            return result;
        }
    }
}