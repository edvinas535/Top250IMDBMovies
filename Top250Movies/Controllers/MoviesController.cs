using Microsoft.AspNetCore.Mvc;
using Top250Movies.Interfaces;

namespace Top250Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MoviesController : Controller
    {
        private readonly IMovies _getMovies;

        public MoviesController(IMovies movies)
        {
            _getMovies = movies;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies(string genre)
        {

            var movies = await _getMovies.GetMoviesAsync(genre);
            if (movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
    }
}
