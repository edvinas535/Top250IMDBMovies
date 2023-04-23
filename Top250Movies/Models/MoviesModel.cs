namespace Top250Movies.Models
{
    public class MoviesModel
    {
        public string Id { get; set; }

        public string Rank { get; set; }

        public string Title { set; get; }

        public string FullTitle { set; get; }

        public string Year { set; get; }

        public string Image { get; set; }

        public string Crew { get; set; }

        public string IMDbRating { get; set; }

        public string IMDbRatingCount { get; set; }
    }
}
