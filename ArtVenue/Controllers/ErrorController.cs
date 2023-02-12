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
            //creates error with quote referencing the lack of art and data for the error depending on status code
            StatusCodeErrorViewModel error = new StatusCodeErrorViewModel(statusCode);
           
            return View(error);
        }
    }
}
