using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
namespace ArtVenue.Controllers
{
    [Authorize(Roles = "admin, moderator")]
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
            var user = await _userManager.GetUserAsync(User);
            bool IsAdmin = await _userManager.IsInRoleAsync(user, "admin");

            //if admin show manage moderators button
            return View(IsAdmin);
        }

        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> Users()
        {
            AdminUsersViewModel data = new AdminUsersViewModel();
            data.Users = new List<MinifiedUser>();
            foreach (var user in _userManager.Users)
            {
                MinifiedUser userToModify = new MinifiedUser()
                {
                    Id = user.Id,
                    IsAdmin = await _userManager.IsInRoleAsync(user, "admin"),
                    IsModerator = await _userManager.IsInRoleAsync(user, "moderator"),
                    UserName = user.FirstName + " " + user.LastName
                };
                data.Users.Add(userToModify);
            }
            data.Users = data.Users.OrderByDescending(x => x.IsAdmin).ThenByDescending(x => x.IsModerator).ToList();
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id);
            _db.Publications.RemoveRange(_db.Publications.Where(x=>x.CreatorId == userToDelete.Id));
            await _userManager.DeleteAsync(userToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction("users");
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult Categories()
        {
            AdminCategoriesViewModel categories = new AdminCategoriesViewModel();
            categories.Categories = _db.Categories.ToList();
            return View(categories);
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult Category(int id)
        {
            return View(_db.Categories.Find(id));
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public IActionResult AddCategory(Category category)
        {
            try
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                return RedirectToAction("categories");
            }
            catch {
                return View(category);
            }

        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator")]
        public async Task<IActionResult> ModifyCategory(Category category)
        {
            _db.Update(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("categories");
        }

        [Authorize(Roles = "admin, moderator")]
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var categoryToDelete = _db.Categories.Find(id);
            if (categoryToDelete == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(categoryToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction("categories");
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult Groups()
        {
            AdminGroupsViewModel data = new AdminGroupsViewModel();
            data.Groups = new List<MinifiedGroup>();
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
            if (groupToDelete == null)
            {
                return NotFound();
            }
            _db.Groups.Remove(groupToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction("groups");
        }

        public async Task<IActionResult> Publications()
        {
            AdminPublicationsViewModel data = new AdminPublicationsViewModel();
            data.Publications = new List<MinfiedPublication>();

            Dictionary<string, string> UserNames = new Dictionary<string, string>();

            foreach (var publication in _db.Publications)
            {
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

            foreach (var user in _userManager.Users)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (!await _userManager.IsInRoleAsync(user, "administrator"))
                {
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
            data.UsersRoles = data.UsersRoles.OrderByDescending(x=>x.IsSelected).ToList();
            return View(data);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageModerators(AdminManageModeratorsViewModel model)
        {
            var role = await _roleManager.FindByNameAsync("moderator");

            for (int i = 0; i < model.UsersRoles.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model.UsersRoles[i].UserId);

                IdentityResult result = null;

                if (model.UsersRoles[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model.UsersRoles[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.UsersRoles.Count - 1))
                        continue;
                    else
                        return RedirectToAction("managemoderators");
                }
            }

            return RedirectToAction("managemoderators");
        }
    }
}
