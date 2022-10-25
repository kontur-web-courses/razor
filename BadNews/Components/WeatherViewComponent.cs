namespace BadNews.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Repositories.Weather;

    public class WeatherViewComponent : ViewComponent
    {
        private readonly IWeatherForecastRepository weatherForecastRepository;

        public WeatherViewComponent(IWeatherForecastRepository weatherForecastRepository)
        {
            this.weatherForecastRepository = weatherForecastRepository;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var weatherForecast = await weatherForecastRepository.GetWeatherForecastAsync();
            return View(weatherForecast);
        }
    }
}