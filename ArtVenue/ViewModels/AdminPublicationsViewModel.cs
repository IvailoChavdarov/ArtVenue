namespace ArtVenue.ViewModels
{
    public class AdminPublicationsViewModel
    {
        public List<MinfiedPublication> Publications { get; set; }
    }
    public class MinfiedPublication
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatorName { get; set; }
    }
}
