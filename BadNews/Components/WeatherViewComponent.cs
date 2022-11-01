using BadNews.Repositories.Weather;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BadNews.Components
{
    public class WeatherViewComponent : ViewComponent
    {
        private readonly IWeatherForecastRepository weatherRepository;

        public WeatherViewComponent(IWeatherForecastRepository weatherRepository)
        {
            this.weatherRepository = weatherRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var weather = await weatherRepository.GetWeatherForecastAsync();
            return View(weather);
        }
    }
}