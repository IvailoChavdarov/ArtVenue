using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace ArtVenue.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(ApplicationDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> Index()
        {
            //checks if user is admin
            var user = await _userManager.GetUserAsync(User);
            bool IsAdmin = await _userManager.IsInRoleAsync(user, "admin");

            //if admin show manage moderators button
            return View(IsAdmin);
        }

        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> Users()
        {
            //gets data for all registered users
            AdminUsersViewModel data = new AdminUsersViewModel();
            data.Users = new List<MinifiedUser>();

            foreach (var user in _userManager.Users)
            {
                MinifiedUser userToModify = new MinifiedUser()
                {
                    Id = user.Id,
                    //disables deleting of admins and moderators
                    IsAdmin = await _userManager.IsInRoleAsync(user, "admin"),
                    IsModerator = await _userManager.IsInRoleAsync(user, "moderator"),
                    //gets user full namer
                    UserName = user.FirstName + " " + user.LastName
                };
                data.Users.Add(userToModify);
            }

            //sorts data by roles
            data.Users = data.Users
                .OrderByDescending(x => x.IsAdmin)
                .ThenByDescending(x => x.IsModerator)
                .ToList();

            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id);

            //removes user's publications to clear data
            _db.Publications
                .RemoveRange(_db.Publications
                .Where(x=>x.CreatorId == userToDelete.Id));

            //deletes user
            await _userManager.DeleteAsync(userToDelete);
            await _db.SaveChangesAsync();

            return RedirectToAction("users");
        }

        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> Categories()
        {
            AdminCategoriesViewModel categories = new AdminCategoriesViewModel();
            //gets data for all categories
            categories.Categories = await _db.Categories.ToListAsync();
            return View(categories);
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult Category(int id)
        {
            //gets data for specific category
            return View(_db.Categories.Find(id));
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> AddCategory(Category category)
        {
            try
            {
                //adds category if it is valid
                _db.Categories.AddAsync(category);
                _db.SaveChangesAsync();

                //sends success notice to user view
                TempData["notice"] = $"Successfully added category!";
                TempData["noticeBackground"] = "bg-success";

                return RedirectToAction("categories");
            }
            catch {
                //returns to view with entered data if category is not valid for adding
                return View(category);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> ModifyCategory(Category category)
        {

            try
            {
                //updates category if it is valid
                _db.Update(category);
                await _db.SaveChangesAsync();

                //sends success notice to user view
                TempData["notice"] = $"Successfully updated category!";
                TempData["noticeBackground"] = "bg-success";

                return RedirectToAction("categories");
            }
            catch
            {
                //send fail notice if something went wrong
                TempData["notice"] = $"Could not update category!";
                TempData["noticeBackground"] = "bg-danger";

                //returns to view with entered data if category is not valid for adding
                return View(category);
            }
        }

        [Authorize(Roles = "admin, moderator")]
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var categoryToDelete = _db.Categories.Find(id);
            //validates that category exists
            if (categoryToDelete == null)
            {
                return NotFound();
            }
            try
            {
                _db.Categories.Remove(categoryToDelete);
                await _db.SaveChangesAsync();

                //sends success notice to user view
                TempData["notice"] = $"Successfully deleted category!";
                TempData["noticeBackground"] = "bg-success";
            }
            catch
            {
                //send fail notice if something went wrong
                TempData["notice"] = $"Could not delete category!";
                TempData["noticeBackground"] = "bg-danger";
            }
            return RedirectToAction("categories");
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult Groups()
        {
            AdminGroupsViewModel data = new AdminGroupsViewModel();
            data.Groups = new List<MinifiedGroup>();

            //gets data for all registered groups
            foreach (var group in _db.Groups)
            {
                MinifiedGroup userToModify = new MinifiedGroup()
                {
                    Id = group.Id,
                    GroupName = group.GroupName
                };
                data.Groups.Add(userToModify);
            }

            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var groupToDelete = _db.Groups.Find(id);

            //validates that group exists
            if (groupToDelete == null)
            {
                return NotFound();
            }


            try
            {
                _db.Groups.Remove(groupToDelete);
                await _db.SaveChangesAsync();

                //sends success notice to user view
                TempData["notice"] = $"Successfully deleted group!";
                TempData["noticeBackground"] = "bg-success";
            }
            catch
            {
                //send fail notice if something went wrong
                TempData["notice"] = $"Could not delete group!";
                TempData["noticeBackground"] = "bg-danger";
            }
            return RedirectToAction("groups");
        }

        public async Task<IActionResult> Publications()
        {
            AdminPublicationsViewModel data = new AdminPublicationsViewModel();
            data.Publications = new List<MinfiedPublication>();

            //optimizes getting user names for multiple posts created by same user
            Dictionary<string, string> UserNames = new Dictionary<string, string>();

            //gets data for all publications posted
            foreach (var publication in _db.Publications)
            {
                //connects publication to creator
                string creatorName;
                string creatorId = publication.CreatorId;
                if (UserNames.ContainsKey(creatorId))
                {
                    creatorName = UserNames[creatorId];
                }
                else
                {
                    var creator = await _userManager.FindByIdAsync(creatorId);
                    creatorName = creator.FirstName + " " + creator.LastName;
                    UserNames.Add(creatorId, creatorName);
                }

                //minifies post data
                MinfiedPublication post = new MinfiedPublication
                {
                    CreatorName = creatorName,
                    Id = publication.Id,
                    Title = publication.PublicationTitle,
                    CreatorId = creatorId
                };

                data.Publications.Add(post);
            }
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            var publicationToDelete = _db.Publications.Find(id);

            //validates that publication exists
            if (publicationToDelete == null)
            {
                return NotFound();
            }

            _db.Publications.Remove(publicationToDelete);
            await _db.SaveChangesAsync();

            return RedirectToAction("publications");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageModerators()
        {
            var data = new AdminManageModeratorsViewModel();
            data.UsersRoles = new List<UserRole>();

            //gets data for all users
            foreach (var user in _userManager.Users)
            {
                //creates connection between user and selected for role
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (!await _userManager.IsInRoleAsync(user, "administrator"))
                {
                    //checks if user is already moderator
                    if (await _userManager.IsInRoleAsync(user, "moderator"))
                    {
                        userRole.IsSelected = true;
                    }
                    else
                    {
                        userRole.IsSelected = false;
                    }
                    data.UsersRoles.Add(userRole);
                }

            }

            //orders users data by roles
            data.UsersRoles = data.UsersRoles
                .OrderByDescending(x=>x.IsSelected)
                .ToList();

            return View(data);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageModerators(AdminManageModeratorsViewModel model)
        {
            //gets moderator role data
            var role = await _roleManager.FindByNameAsync("moderator");

            //modifies selected users
            for (int i = 0; i < model.UsersRoles.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model.UsersRoles[i].UserId);

                IdentityResult result;

                //makes user moderator if user is selected
                if (model.UsersRoles[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                //removes user from moderator role if he was selected before, but not now
                else if (!model.UsersRoles[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                //ignore if no changes
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    //sends success notice to user view
                    TempData["notice"] = $"Successfully edited moderators!";
                    TempData["noticeBackground"] = "bg-success";

                    return RedirectToAction("managemoderators");
                }
            }

            return RedirectToAction("managemoderators");
        }
    }
}
