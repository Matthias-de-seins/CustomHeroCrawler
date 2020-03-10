using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wba.Crawler.Domain;
using Wba.Crawler.Models;

namespace Wba.Crawler.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            CustomCrawler myCrawler = new CustomCrawler();

            var heronames = myCrawler.GetAllLinks();

            //1)
            //myCrawler.DownloadHeroes();
            //2)
            //myCrawler.MakeHeroFiles(heronames);
            //3)
            //myCrawler.DownloadHero(heronames);
            //4)
            //myCrawler.CreateHeroImages(heronames);
            //5
            //myCrawler.DownloadHeroImages(heronames);
            //6
            //myCrawler.CreateJsonFile();



            myCrawler.PopulateJsonFile(heronames);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
