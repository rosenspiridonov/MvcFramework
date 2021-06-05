using MyFirstMvcApp.ViewModels;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Linq;
using System.Text;

namespace MyFirstMvcApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public HttpResponse Index()
        {
            var viewModel = new IndexViewModel()
            {
                CurrentYear = DateTime.UtcNow.Year,
                Message = "Welcome to Battle Cards"
            };

            return this.View(viewModel);
        }

        public HttpResponse About()
        {
            return View();
        }
    }
}
