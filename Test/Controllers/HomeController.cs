using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LapTopContext _context;
        public HomeController(ILogger<HomeController> logger, LapTopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            
            List<Sanpham> listdata = _context.Sanpham.Where(sp => sp.Ramat == 2021).Select(sp => new Sanpham
            {
               Masp = sp.Masp,
               Tensp = sp.Tensp,
               Dongia = sp.Dongia,
               Hinhanh = sp.Hinhanh,
               BoxulyNavigation = sp.BoxulyNavigation,
               RamNavigation= sp.RamNavigation,
               ManhinhNavigation =  sp.ManhinhNavigation,

            }).Take(8).ToList();


            return View(listdata);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
        }

        public IActionResult Signin()
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
