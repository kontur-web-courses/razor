using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    using System;
    using Microsoft.Extensions.Caching.Memory;

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

        public IActionResult Index([FromQuery] int? year, [FromQuery] int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(model);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle([FromRoute] Guid id)
        {
            var cacheKey = $"{nameof(NewsController)}_{id}";

            if (!memoryCache.TryGetValue(cacheKey, out var article))
            {
                article = newsModelBuilder.BuildFullArticleModel(id);
                if (article != null)
                {
                    memoryCache.Set(cacheKey, article, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(30)
                    });
                }
            }

            if (article == null)
                return NotFound();

            return View(article);
        }
    }
}