using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BadNews.Repositories.Weather
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private const string defaultWeatherImageUrl = "/images/cloudy.png";

        private readonly Random random = new Random();

        private readonly OpenWeatherClient weatherClient;

        public WeatherForecastRepository(IOptions<OpenWeatherOptions> weatherOptions)
        {
            weatherClient = new OpenWeatherClient(weatherOptions?.Value.ApiKey);
        }
        public async Task<WeatherForecast> GetWeatherForecastAsync()
        {
            if (weatherClient == null) 
                return BuildRandomForecast();
            var forecast = await weatherClient.GetWeatherFromApiAsync();
            return WeatherForecast.CreateFrom(forecast);
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
