using System.ComponentModel.DataAnnotations;
namespace ArtVenue.ViewModels
{
    public class ViewModelWithCommentInput
    {
        //used to pass data between view and controller for posting comment to publication
        public CommentInput CommentInput { get; set; }
    }
    public class CommentInput
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public string CommentContent { get; set; }
    }
}
