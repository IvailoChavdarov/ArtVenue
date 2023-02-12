using ArtVenue.Data;
using ArtVenue.Models;
using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArtVenue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _logger = logger;
            _db = db;
        }

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