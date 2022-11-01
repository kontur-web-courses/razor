using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";
        private readonly OpenWeatherClient weatherClient;

        private readonly Random random = new Random();

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            weatherClient = new OpenWeatherClient(weatherOptions.Value.ApiKey);
        }
        
        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            try
            {
                return WeatherForecast.CreateFrom(await weatherClient.GetWeatherFromApiAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BuildRandomForecast();
            }
        }

        private WeatherForecast BuildRandomForecast()
        {
            var temperature = random.Next(-20, 20 + 1);
            return new WeatherForecast
            {
                TemperatureInCelsius = temperature,
                IconUrl = defaultWeatherImageUrl
            };
        }
    }
}
