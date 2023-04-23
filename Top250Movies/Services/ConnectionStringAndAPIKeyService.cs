using Microsoft.Extensions.Configuration;
using Top250Movies.Interfaces;

namespace Top250Movies.Services
{
    public class ConnectionStringAndAPIKeyService : IMovieDB
    {
        public string GetConnectionStringOrAPIKey(string condition)
        {
            if(condition == "APIKey")
            {
                return APIKey();
            }

            if(condition == "ConnectionString")
            {
                return ConnectionString();
            }

            return "";
        }
        static private string ConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            return connectionString;
        }

        static private string APIKey()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string apiKey = configuration.GetConnectionString("APIKey");

            return apiKey;
        }
    }
}
