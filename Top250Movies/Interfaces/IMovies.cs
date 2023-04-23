using Top250Movies.Models;

namespace Top250Movies.Interfaces
{
    public interface IMovies
    {
        Task<List<MoviesModel>> GetMoviesAsync(string genre);
    }
}
