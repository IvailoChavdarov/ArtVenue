using Microsoft.AspNetCore.Http;

namespace ArtVenue.ViewModels
{
    public class StatusCodeErrorViewModel
    {
        public StatusCodeErrorViewModel(int statusCode)
        {
            //gets random quote for the lack of art
            Random random = new Random();
            string[] quotes = new string[]
            {
                "The absence of good art is a void that can never be filled. - Banksy",
                "The absence of creativity is like a blank canvas, waiting to be filled with inspiration. - Unknown",
                "The absence of art is like a garden without flowers. - Pablo Picasso",
                "When art is missing, life becomes dull and uninspiring. - Vincent van Gogh",
                "The absence of art is like a world without color. - Henri Matisse",
                "The absence of beauty is a tragedy, but the absence of art is a crisis. - Unknown",
                "Without art, life would be a mistake. - Friedrich Nietzsche",
                "Art is the missing piece in a world without creativity. - Oscar Wilde"
            };
            int randomQuoteIndex = random.Next(0, quotes.Length);
            this.Quote = quotes[randomQuoteIndex];

            //sets the given status code
            this.ErrorCode = statusCode;

            //selects data with description and corresponding image for the given status code error
            switch (statusCode)
            {
                case 400:
                    this.ErrorMessage = "Bad Request";
                    this.ErrorDescription = "The request for this artwork was not quite up to the gallery's standards. Please refine your search criteria.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_broken_server_on_a_white_background_and_paint_spla_ff29f670-13d3-4d8d-8546-65c140b8202f_baofyj.png";
                    break;
                case 404:
                    this.ErrorMessage = "Not Found";
                    this.ErrorDescription = "Our apologies, but it seems the artwork you're looking for has gone missing. Perhaps it's gone on a wanderlust adventure?";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676129446/404_jkzsm2.png";
                    break;
                case 401:
                    this.ErrorMessage = "Unauthorized";
                    this.ErrorDescription = "Access to this artwork is restricted to authorized admirers only. Please gain proper clearance before viewing.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_artwork_protected_with_lock_and_chains_on_white_ba_df1c1a8b-e60d-403b-8a14-78664af7c8c1_ykrgia.png";
                    break;
                case 403:
                    this.ErrorMessage = "Forbidden";
                    this.ErrorDescription = "The client is authenticated but does not have access to the requested resource.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_artwork_protected_with_lock_and_chains_on_white_ba_df1c1a8b-e60d-403b-8a14-78664af7c8c1_ykrgia.png";
                    break;
                case 405:
                    this.ErrorMessage = "Method Not Allowed";
                    this.ErrorDescription = "Just like a canvas, this request won't let you add any new brush strokes.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_artwork_protected_with_lock_and_chains_on_white_ba_df1c1a8b-e60d-403b-8a14-78664af7c8c1_ykrgia.png";
                    break;
                case 500:
                    this.ErrorMessage = "Internal Server Error";
                    this.ErrorDescription = "Our servers are experiencing a bit of artist's block. Please try again later.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_broken_server_on_a_white_background_and_paint_spla_ff29f670-13d3-4d8d-8546-65c140b8202f_baofyj.png";
                    break;
                case 505:
                    this.ErrorMessage = "HTTP Version Not Suppported";
                    this.ErrorDescription = "Looks like the server is lost in its own world, creating masterpieces. Please try again later.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_broken_server_on_a_white_background_and_paint_spla_ff29f670-13d3-4d8d-8546-65c140b8202f_baofyj.png";
                    break;
                default:
                    this.ErrorMessage = "An unknown error happened while processing your request";
                    this.ErrorDescription = "Just like the mystery of the Mona Lisa's smile, this error remains unsolved. Let's keep exploring the world of art together.";
                    this.ImageUrl = "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1676109327/DragoDragiev_broken_server_on_a_white_background_and_paint_spla_ff29f670-13d3-4d8d-8546-65c140b8202f_baofyj.png";
                    break;
            }
        }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string Quote { get; set; }
        public string ImageUrl { get; set; }
    }
}
