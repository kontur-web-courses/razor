using BadNews.Components;
using BadNews.ModelBuilders.News;
using BadNews.Models.Editor;
using BadNews.Models.News;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BadNews.Controllers
{
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, VaryByHeader = "Cookie")]
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;
        private IMemoryCache memoryCache;
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

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle(Guid id)
        {
            string cacheKey = id.ToString();
            if (!memoryCache.TryGetValue(cacheKey, out FullArticleModel model))
            {
                model = newsModelBuilder.BuildFullArticleModel(id);
                if (model != null)
                {
                    memoryCache.Set(cacheKey, model, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(30)
                    });
                }

            }
            if (model is null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateArticle([FromForm] IndexViewModel model)
        {
            return View("Index", model);
        }
    }
}