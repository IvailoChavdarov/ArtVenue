namespace ArtVenue.ViewModels
{
    public class AdminPublicationsViewModel
    {
        //collection of all posted publications' minified data
        public List<MinfiedPublication> Publications { get; set; }
    }
    public class MinfiedPublication
    {
        //minified version of publication and creator data
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatorName { get; set; }
        public string CreatorId { get; set; }
    }
}
