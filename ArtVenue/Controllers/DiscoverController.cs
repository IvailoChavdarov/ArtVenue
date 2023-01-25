using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtVenue.Controllers
{
    public class DiscoverController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public DiscoverController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            DiscoverIndexViewModel data = new DiscoverIndexViewModel();
            if (User.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(User);
                List<Category> allCategories = _db.Categories.ToList();
                data.Categories = new List<Category>();
                foreach (var category in allCategories)
                {
                    category.UserIsInterested = _db.Interests.Where(x => x.CategoryId == category.Id && x.UserId == userId).Any();
                    data.Categories.Add(category);
                }
            }
            else
            {
                data.Categories = _db.Categories.ToList();
            }
            
            return View(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeInterest(int id, string controllerName, string actionName)
        {
            string userId = _userManager.GetUserId(User);
            Users_Interests connection = new Users_Interests();
            connection.CategoryId = id;
            connection.UserId = userId;
            if (_db.Interests.Contains(connection))
            {
                _db.Interests.Remove(connection);
            }
            else
            {
                await _db.Interests.AddAsync(connection);
            }
            await _db.SaveChangesAsync();
            if (actionName=="category")
            {
                return RedirectToAction(actionName, controllerName, new {Id=id});
            }
            return RedirectToAction(actionName, controllerName);
        }
    }
}
