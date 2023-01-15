using System.ComponentModel.DataAnnotations;
namespace ArtVenue.ViewModels
{
    public class ViewModelWithCommentInput
    {
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
