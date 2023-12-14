using System;
using BadNews.Models.Editor;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    public class EditorController : Controller
    {
        private INewsRepository NewsRepository { get; }

        public EditorController(INewsRepository newsRepository)
        {
            NewsRepository = newsRepository;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel());
        }

        [HttpPost]
        public IActionResult CreateArticle([FromForm] IndexViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var id = NewsRepository.CreateArticle(new NewsArticle
            {
                Date = DateTime.Now.Date,
                Header = model.Header,
                Teaser = model.Teaser,
                ContentHtml = model.ContentHtml,
            });

            return RedirectToAction("FullArticle", "News", new { id });
        }
    }
}