using Top250Movies.Interfaces;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Top250Movies.Models;

namespace Top250Movies.Repository
{
    public class UpdateDataRepository :IUpdateData
    {
        private readonly IMovieDB _db;
        private readonly IDBTables _dbTables;

        public UpdateDataRepository(IMovieDB db, IDBTables dbTables)
        {
            _db = db;
            _dbTables = dbTables;
        }

        public async Task UpdateDataAsync()
        {
            (List<MoviesModel> moviesModelsList, List<GenresModel> genresList, List<MovieGenresModel> movieGenresList) = await _dbTables.GetIMDBDataAsync();

            bool isDeleted = await DeleteOldDataAsync();
            if (isDeleted)
            {
                await _dbTables.FillUpDBTablesAsync(moviesModelsList, genresList, movieGenresList);
            }
        }

        private async Task<bool> DeleteOldDataAsync()
        {
            string connectionString = _db.GetConnectionStringOrAPIKey("ConnectionString");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // List of table names from which you want to delete all data
                    List<string> tableNames = new List<string> { "moviegenres", "movies", "genres" };

                    foreach (string tableName in tableNames)
                    {
                        using (MySqlCommand command = new MySqlCommand($"DELETE FROM {tableName}", connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                    Debug.WriteLine("All data deleted from the tables.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
