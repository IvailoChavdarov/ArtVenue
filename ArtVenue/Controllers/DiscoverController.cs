using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            DiscoverIndexViewModel data = new DiscoverIndexViewModel();
            if (User.Identity.IsAuthenticated)
            {
                //sets if category is in user interests if user is logged in
                string userId = _userManager.GetUserId(User);
                List<Category> allCategories = await _db.Categories.ToListAsync();
                data.Categories = new List<Category>();

                foreach (var category in allCategories)
                {
                    //checks if user is interested in category
                    category.UserIsInterested = _db.Interests
                        .Where(x => x.CategoryId == category.Id && x.UserId == userId)
                        .Any();

                    data.Categories.Add(category);
                }
            }
            else
            {
                //gets existing categories
                data.Categories = await _db.Categories.ToListAsync();
            }
            
            return View(data);
        }


        public async Task<IActionResult> Search(string search, string searchType)
        {
            DiscoverSearchViewModel data = new DiscoverSearchViewModel();

            //sets search data
            data.SearchType = searchType;
            data.SearchQuery = search;

            data.ResultItems = new List<SearchResultItem>();

            //splits search query to different words
            string[] searchQueries = search.Split().ToArray(); 

            //checks what is the type of search
            switch (searchType)
            {
                case "categories":
                    //searches categories by name and description
                    List<Category> categories = new List<Category>();
                    foreach (var query in searchQueries)
                    {
                        foreach (var category in _db.Categories
                            .Where(x => x.CategoryName.Contains(query)||
                                x.CategoryDescription
                                .Contains(query)))
                        {
                            if (!categories.Contains(category))
                            {
                                categories.Add(category);
                            }
                        }
                    }

                    //adds results to data model
                    foreach (var category in categories)
                    {
                        data.ResultItems.Add(new SearchResultItem(category));
                    }
                    break;
                case "groups":
                    //searches groups by name, description
                    List<Group> groups = new List<Group>();
                    foreach (var query in searchQueries)
                    {
                        foreach (var group in _db.Groups
                            .Where(x => x.GroupName.Contains(query) || x.Description.Contains(query)))
                        {
                            if (!groups.Contains(group))
                            {
                                groups.Add(group);
                            }
                        }
                    }

                    //adds results to data model
                    foreach (var group in groups)
                    {
                        data.ResultItems.Add(new SearchResultItem(group));
                    }
                    break;
                case "users":
                    //searches users by bio, name, arttype
                    List<AppUser> users = new List<AppUser>();
                    foreach (var query in searchQueries)
                    {
                        foreach (var user in _userManager.Users
                            .Where(x => x.FirstName == query|| x.LastName==query || x.Bio.Contains(query)||x.ArtType.Contains(query)))
                        {
                            if (!users.Contains(user))
                            {
                                users.Add(user);
                            }
                        }
                    }

                    //adds results to data model
                    foreach (var user in users)
                    {
                        data.ResultItems.Add(new SearchResultItem(user));
                    }
                    break;

                    //validates the search type
                default:
                    return NotFound();
            }

            //returns search data and results
            return View(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeInterest(int id, string controllerName, string actionName)
        {
            //gets current user
            string userId = _userManager.GetUserId(User);

            //creates connection between current use and category
            Users_Interests connection = new Users_Interests();
            connection.CategoryId = id;
            connection.UserId = userId;

            //checks if connection already exists in database
            if (_db.Interests.Contains(connection))
            {
                //removes connection if it already exists
                _db.Interests.Remove(connection);
            }
            else
            {
                //adds connection if it doesnt exist yet
                await _db.Interests.AddAsync(connection);
            }

            await _db.SaveChangesAsync();

            //returns to the page from which the action was called
            if (actionName=="category")
            {
                return RedirectToAction(actionName, controllerName, new {Id=id});
            }
            return RedirectToAction(actionName, controllerName);
        }
    }
}
