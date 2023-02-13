using ArtVenue.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArtVenue.Controllers
{
    //reroutes to this controller if something throws an uncaught exception, or had invalid request
    public class ErrorController : Controller
    {
        //when the app throws an uncaught exception
        //returns view saying something went wrong
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ServerError()
        {
            return View();
        }

        //when the user sends invalid HTTP request and response status code error is returned
        //returns view with quote talking about the lack of art and data for the error in their request
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
