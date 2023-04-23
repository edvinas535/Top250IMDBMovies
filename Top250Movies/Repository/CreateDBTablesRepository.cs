using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Top250Movies.Models;
using System.Net.Http;
using HtmlAgilityPack;

using Top250Movies.Interfaces;
using System.Text.RegularExpressions;

namespace Top250Movies.Repository
{
    public class CreateDBTablesRepository : IDBTables
    {
        private readonly IMovieDB _db;
        private readonly HttpClient _httpClient;

        public CreateDBTablesRepository(IMovieDB db, HttpClient httpClient)
        {
            _db = db;
            _httpClient = httpClient;
        }

        public async Task CreateDBAsync()
        {
            string connectionString = _db.GetConnectionStringOrAPIKey("ConnectionString");

            try
            {
                // Create Movies table
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;//Id INT PRIMARY KEY AUTO_INCREMENT,
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Movies (                                   
                                    MovieId VARCHAR(50) PRIMARY KEY,
                                    Rank INT,
                                    Title VARCHAR(50),
                                    FullTitle VARCHAR(150),
                                    Year INT,
                                    Image VARCHAR(200),
                                    Crew VARCHAR(200),
                                    IMDbRating DOUBLE,
                                    IMDbRatingCount INT);";
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("Movies table created successfully!");
                    connection.Close();
                }

                // Create Genres table
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;// AUTO_INCREMENT
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Genres (
                                    Id INT PRIMARY KEY,
                                    Genre VARCHAR(100) NOT NULL);";
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("Genres table created successfully!");
                    connection.Close();
                }

                // Create MovieGenres table
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS MovieGenres (
                                    MoviesId VARCHAR(50),
                                    GenreId INT,
                                    FOREIGN KEY (MoviesId) REFERENCES Movies (MovieId),
                                    FOREIGN KEY (GenreId) REFERENCES Genres (Id),
                                    PRIMARY KEY (MoviesId, GenreId));";
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("MovieGenres table created successfully!");


                }

                bool hasData = await CheckTableHasDataAsync(); //Check if DB table "movies" has data
                if (!hasData)
                {
                    (List<MoviesModel> moviesModelsList, List<GenresModel> genresList, List<MovieGenresModel> movieGenresList) = await GetIMDBDataAsync();
                    await FillUpDBTablesAsync(moviesModelsList, genresList, movieGenresList);
                }
            }
            catch(Exception ex)
            {

            }     
        }

        private async Task<bool> CheckTableHasDataAsync()
        {
            string connectionString = _db.GetConnectionStringOrAPIKey("ConnectionString");
            bool hasData = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand($"SELECT COUNT(*) FROM movies", connection))
                    {
                        int rowCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (rowCount > 0)
                        {
                            hasData = true;
                        }
                    }

                    connection.Close();
                }

                return hasData;
            }
            catch(Exception ex)
            {
                return hasData;
            }

            
        }

        public async Task FillUpDBTablesAsync(List<MoviesModel> moviesModelsList, List<GenresModel> genresList, List<MovieGenresModel> movieGenresList)
        {            
            string connectionString = _db.GetConnectionStringOrAPIKey("ConnectionString");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var data in moviesModelsList)
                    {
                        string insertQuery = "INSERT INTO movies (MovieId, Rank, Title, FullTitle, Year, Image, Crew, IMDbRating, IMDbRatingCount) " +
                                                "VALUES (@MovieId, @Rank, @Title, @FullTitle, @Year, @Image, @Crew, @IMDbRating, @IMDbRatingCount);";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@MovieId", data.Id);
                            command.Parameters.AddWithValue("@Rank", Int32.Parse(data.Rank));
                            command.Parameters.AddWithValue("@Title", data.Title);
                            command.Parameters.AddWithValue("@FullTitle", data.FullTitle);
                            command.Parameters.AddWithValue("@Year", Int32.Parse(data.Year));
                            command.Parameters.AddWithValue("@Image", data.Image);
                            command.Parameters.AddWithValue("@Crew", data.Crew);
                            command.Parameters.AddWithValue("@IMDbRating", Convert.ToDouble(data.IMDbRating));
                            command.Parameters.AddWithValue("@IMDbRatingCount", Int32.Parse(data.IMDbRatingCount));
                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (var data in genresList)
                    {
                        string insertQuery = "INSERT INTO genres (Id, Genre) VALUES (@Id, @Genre);";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Id", data.Id);
                            command.Parameters.AddWithValue("@Genre", data.Genre);
                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (var data in movieGenresList)
                    {
                        string insertQuery = "INSERT INTO moviegenres (MoviesId, GenreId) VALUES (@MoviesId, @GenreId);";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@MoviesId", data.MovieId);
                            command.Parameters.AddWithValue("@GenreId", data.GenreId);
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }

            
        }

        public async Task<(List<MoviesModel>,List<GenresModel>, List<MovieGenresModel>)> GetIMDBDataAsync()
        {
            List<MoviesModel> moviesModels = new List<MoviesModel>();
            List<GenresModel> genresList = new List<GenresModel>();
            List<MovieGenresModel> movieGenresList = new List<MovieGenresModel>();
            try
            {
                // Make GET request to API
                HttpResponseMessage response = await _httpClient.GetAsync("https://imdb-api.com/API/Top250Movies/" + _db.GetConnectionStringOrAPIKey("APIKey"));

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIDataModel top250DataDetails = JsonConvert.DeserializeObject<APIDataModel>(json);

                    (genresList, movieGenresList) = await GetGenresAsync(top250DataDetails.items);
                    moviesModels = top250DataDetails.items;
                }
            }
            catch(Exception ex) { }
            
            return (moviesModels, genresList, movieGenresList);
        }

        private async Task<(List<GenresModel>, List<MovieGenresModel>)> GetGenresAsync(List<MoviesModel> movies)
        {
            List<GenresModel> genresList = new List<GenresModel>();
            List<MovieGenresModel> movieGenresList = new List<MovieGenresModel>();
            
            foreach(var movie in movies)
            {
                string id = movie.Id;
                string url = "https://www.imdb.com/title/" + id + "/";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                // Specify the XPath expression to select the element with data-testid='genres'
                string xpath = "//*[@data-testid='genres']";

                // Select the element with the specified XPath expression
                HtmlNode genreNode = doc.DocumentNode.SelectSingleNode(xpath);
                if (genreNode != null)
                {
                    string genres = genreNode.InnerText.Trim();

                    // Use regular expression to split string into words
                    string[] splittedGenres = Regex.Split(genres, @"(?<!^)(?=[A-Z0-9])(?<!Sci-)(?<!Film-)(?<!Game-)(?<!Reality-)(?<!Talk-)");

                    foreach (string gen in splittedGenres)
                    {
                        (int index, List<GenresModel> updatedList) = await AddIfNotExistAsync(genresList, gen);
                        genresList = updatedList; // Update the genresList with the updatedList
                        MovieGenresModel movieGenre = new MovieGenresModel { MovieId = id, GenreId = index};
                        movieGenresList.Add(movieGenre);
                    }
                }
            }
            return (genresList, movieGenresList);
        }

        static async Task<(int, List<GenresModel>)> AddIfNotExistAsync(List<GenresModel> dataList, string newGenre)
        {
            for (int i = 0; i < dataList.Count; i++)//
            {
                if (dataList[i].Genre.Equals(newGenre))
                {
                    // If the genre already exists in the list, return its index + 1 (to start from 1)
                    return (i + 1, dataList);
                }
            }

            GenresModel newData = new GenresModel { Id = dataList.Count + 1, Genre = newGenre };
            dataList.Add(newData);
            return (dataList.Count, dataList);
        }
    }
}
