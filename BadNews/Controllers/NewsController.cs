using System;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BadNews.Controllers
{
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, VaryByHeader = "Cookie")]
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;
        private readonly IMemoryCache memoryCache;

        public NewsController(INewsModelBuilder newsModelBuilder, IMemoryCache memoryCache)
        {
            this.newsModelBuilder = newsModelBuilder;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index(int? year)
        {
            var pageIndex = 0;
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(model);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle(Guid id)
        {
            if (memoryCache.TryGetValue(id, out var model)) return View(model);
            model = newsModelBuilder.BuildFullArticleModel(id);
            if (model != null)
            {
                memoryCache.Set(id, model, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                });
            }

            else
            {
                return NotFound();
            }


            return View(model);
        }
    }
}