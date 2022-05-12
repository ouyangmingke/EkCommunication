using Microsoft.Extensions.Configuration;

namespace EkTools
{
    public class ConfigHelper
    {

        private static IConfigurationRoot BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine("Config", "appsettings.json"))
            .Build();
            return configuration;
        }
    }
}