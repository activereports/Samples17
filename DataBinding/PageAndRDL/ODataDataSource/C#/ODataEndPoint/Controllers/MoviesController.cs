using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataEndPoint.Models;

namespace ODataEndPoint.Controllers
{
	public class MoviesController : ODataController
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _env;

		public MoviesController(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
			_env = env;
		}
		
		public IList<Movie> Get()
		{
			var movies = new List<Movie>();

			var connStr = _configuration.GetSection("ConnectionStrings")["Reels"].Replace("$appPath$", _env.ContentRootPath);
			var conn = new SqliteConnection(connStr);
			conn.Open();
			var cmd = new SqliteCommand("SELECT Movie.MovieID, Movie.Title, Movie.MPAA, Movie.YearReleased FROM Movie ORDER BY Movie.YearReleased", conn);
			var dataReader = cmd.ExecuteReader();
			while (dataReader.Read())
			{
				movies.Add(new Movie
				{
					Id = Convert.ToInt32(dataReader.GetValue(0)),
					Title = dataReader.GetValue(1).ToString()!,
					MPAA = dataReader.GetValue(2).ToString()!,
					YearReleased = Convert.ToInt32(dataReader.GetValue(3))
				});
			}
			conn.Close();
			return movies;
		}
	}
}