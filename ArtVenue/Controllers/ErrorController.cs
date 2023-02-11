using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArtVenue.Controllers
{
    public class ErrorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ServerError()
        {
            return View();
        }
        [Route("error/statuscode/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusCodeError(int statusCode)
        {
            StatusCodeErrorViewModel error = new StatusCodeErrorViewModel(statusCode);
           
            return View(error);
        }
    }
}
