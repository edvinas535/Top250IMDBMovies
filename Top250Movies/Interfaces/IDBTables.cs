using Top250Movies.Models;

namespace Top250Movies.Interfaces
{
    public interface IDBTables
    {
        Task CreateDBAsync();
        Task<(List<MoviesModel>, List<GenresModel>, List<MovieGenresModel>)> GetIMDBDataAsync();
        Task FillUpDBTablesAsync(List<MoviesModel> moviesModelsList, List<GenresModel> genresList, List<MovieGenresModel> movieGenresList);
    }
}
