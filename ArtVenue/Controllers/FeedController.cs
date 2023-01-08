using ArtVenue.Data;
using ArtVenue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArtVenue.Controllers
{
    public class FeedController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public FeedController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> userNames = new Dictionary<string, string>();
            List<Publication> data = new List<Publication>();
            foreach (var publication in _db.Publications)
            {
                if (userNames.ContainsKey(publication.CreatorId))
                {
                    publication.CreatorName = userNames[publication.CreatorId];
                }
                else
                {
                    AppUser creator = await _userManager.FindByIdAsync(publication.CreatorId);
                    string creatorFullName = creator.FirstName + " " + creator.LastName;
                    userNames.Add(publication.CreatorId, creatorFullName);
                    publication.CreatorName = creatorFullName;
                }
                data.Add(publication);
            }
            return View(data);
        }
    }
}
