using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArtVenue.Controllers
{
    //responsible for rerouting to the main info pages of ArtVenue
    public class HomeController : Controller
    {
        //dependency injection
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _logger = logger;
            _db = db;
        }

        //returns view with data for some categories to display as advertisement on the homepage
        public IActionResult Index()
        {
            //redirects to suggested publication page if user is logged in
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "posts");
            }

            //sends some categories data to view
            HomeIndexViewModel data = new HomeIndexViewModel();
            data.MainCategories = _db.Categories.Take(8).ToArray();
            return View(data);
        }

        //return views with more info about ArtVenue
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Technologies()
        {
            return View();
        }
        public IActionResult Embedding()
        {
            return View();
        }

    }
}