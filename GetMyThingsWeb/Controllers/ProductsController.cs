using GetMyThingsWeb.Models;
using GetMyThingsWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace GetMyThingsWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.db = context;
            this.environment = environment;
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

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"); 
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName); 

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // save the new product in the database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            db.Products.Add(product);
            db.SaveChanges();
        
            return RedirectToAction("Index", "Products");
        }


        public IActionResult Edit(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // create productDto from product
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id; 
            ViewData["ImageFileName"] = product.ImageFileName; 
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");


            return View(productDto);
        }
    }
}
