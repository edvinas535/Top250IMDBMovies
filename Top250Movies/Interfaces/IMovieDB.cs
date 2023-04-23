namespace Top250Movies.Interfaces
{
    public interface IMovieDB
    {
        string GetConnectionStringOrAPIKey(string condition);
    }
}
