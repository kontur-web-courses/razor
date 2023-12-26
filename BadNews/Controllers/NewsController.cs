using System;
using System.Threading.Tasks;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BadNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;
        private readonly IMemoryCache memoryCache;

        public NewsController(INewsModelBuilder newsModelBuilder, IMemoryCache memoryCache)
        {
            this.newsModelBuilder = newsModelBuilder;
            this.memoryCache = memoryCache;
        }

        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, VaryByHeader = "Cookie")]
        public async Task<IActionResult> Index(int? year, int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex!, true, year);
            return View(model);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> FullArticle(Guid id)
        {
            if (memoryCache.TryGetValue(id, out var model)) return View(model);

            model = newsModelBuilder.BuildFullArticleModel(id);
            
            if (model is null) return NotFound();
            
            memoryCache.Set(id, model, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(30)
            });
            return View(model);
        }
    }
}