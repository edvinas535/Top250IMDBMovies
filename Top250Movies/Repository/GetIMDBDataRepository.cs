using Top250Movies.Interfaces;
using Top250Movies.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Top250Movies.Repository
{
    public class GetIMDBDataRepository : IMovies
    {
        private readonly IMovieDB _db;

        public GetIMDBDataRepository(IMovieDB db)
        {
            _db = db;
        }

        public async Task<List<MoviesModel>> GetMoviesAsync(string genre)
        {
            List<MoviesModel> movies = new List<MoviesModel>();
            try
            {
                string connectionString = _db.GetConnectionStringOrAPIKey("ConnectionString");

                string query = @"SELECT movies.*
                        FROM moviegenres
                        INNER JOIN movies ON movies.MovieId = moviegenres.MoviesId
                        INNER JOIN genres ON genres.Id = moviegenres.GenreId
                        WHERE genres.Genre = @genre
                        ORDER BY movies.IMDbRating DESC";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@genre", genre);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Retrieve data from movies table columns
                                string MovieId = Convert.ToString(reader["MovieId"]);
                                string Rank = Convert.ToString(reader["Rank"]);
                                string Title = Convert.ToString(reader["Title"]);
                                string FullTitle = Convert.ToString(reader["FullTitle"]);
                                string Year = Convert.ToString(reader["Year"]);
                                string Image = Convert.ToString(reader["Image"]);
                                string Crew = Convert.ToString(reader["Crew"]);
                                string IMDbRating = Convert.ToString(reader["IMDbRating"]);
                                string IMDbRatingCount = Convert.ToString(reader["IMDbRatingCount"]);

                                MoviesModel newMovie = new MoviesModel { Id = MovieId, Rank = Rank, Title = Title, FullTitle = FullTitle, Year = Year, Image = Image, Crew = Crew, IMDbRating = IMDbRating, IMDbRatingCount = IMDbRatingCount};
                                movies.Add(newMovie);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return movies;
        }
    }
}
