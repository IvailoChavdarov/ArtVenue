using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        public async Task<IActionResult> Search(string search, string searchType)
        {
            DiscoverSearchViewModel data = new DiscoverSearchViewModel();
            data.SearchType = searchType;
            data.SearchQuery = search;
            data.ResultItems = new List<SearchResultItem>();
            string[] searchQueries = search.Split().ToArray(); 
            switch (searchType)
            {
                case "categories":
                    //name and description
                    List<Category> categories = new List<Category>();
                    foreach (var query in searchQueries)
                    {
                        foreach (var category in _db.Categories.Where(x => x.CategoryName.Contains(query)||x.CategoryDescription.Contains(query)))
                        {
                            if (!categories.Contains(category))
                            {
                                categories.Add(category);
                            }
                        }
                    }
                    foreach (var category in categories)
                    {
                        data.ResultItems.Add(new SearchResultItem(category));
                    }
                    break;
                case "groups":
                    //name, description
                    List<Group> groups = new List<Group>();
                    foreach (var query in searchQueries)
                    {
                        foreach (var group in _db.Groups.Where(x => x.GroupName.Contains(query) ||x.Description.Contains(query)))
                        {
                            if (!groups.Contains(group))
                            {
                                groups.Add(group);
                            }
                        }
                    }
                    foreach (var group in groups)
                    {
                        data.ResultItems.Add(new SearchResultItem(group));
                    }
                    break;
                case "users":
                    //bio, name, arttype
                    List<AppUser> users = new List<AppUser>();
                    foreach (var query in searchQueries)
                    {
                        foreach (var user in _userManager.Users.Where(x => x.FirstName == query|| x.LastName==query || x.Bio.Contains(query)||x.ArtType.Contains(query)))
                        {
                            if (!users.Contains(user))
                            {
                                users.Add(user);
                            }
                        }
                    }
                    foreach (var user in users)
                    {
                        data.ResultItems.Add(new SearchResultItem(user));
                    }
                    break;
                default:
                    return NotFound();
            }
            //var publications = _db.Publications
            //    .Where(p => p.PublicationTitle.Contains(searchTerm) || p.PublicationText.Contains(searchTerm))
            //    .ToList();

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
