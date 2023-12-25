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

        public IActionResult Index(int? year, int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(model);
        }

        public IActionResult FullArticle(Guid id)
        {
            if (memoryCache.TryGetValue(id, out var article))
                return View(article);

            article = newsModelBuilder.BuildFullArticleModel(id);
            if (article == null)
                return NotFound();

            memoryCache.Set(id, article, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(30)
            });

            return View(article);
        }
    }
}