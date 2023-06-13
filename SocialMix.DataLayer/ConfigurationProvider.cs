using Microsoft.Extensions.Configuration;

namespace SocialMix.DataLayer
{
    // Define an interface for configuration access
    public interface IConfigurationProvider
    {
        string GetConnectionString(string name);
    }

    // Implement the IConfigurationProvider interface
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IConfiguration _configuration;

        public ConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }
    }
}
