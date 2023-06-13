using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace SocialMix.DataLayer
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SocialMixDBConnection");
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
