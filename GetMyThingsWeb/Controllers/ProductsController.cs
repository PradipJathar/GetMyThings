using GetMyThingsWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace GetMyThingsWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;

        public ProductsController(ApplicationDbContext context)
        {
            this.db = context;
        }


        public IActionResult Index()
        {
            var products = db.Products.OrderByDescending(m => m.Id).ToList();

            return View(products);
        }


        public IActionResult Create()
        {
            return View();
        }
    }
}
