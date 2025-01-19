using GetMyThingsWeb.Models;
using GetMyThingsWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace GetMyThingsWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.db = context;
            this.environment = environment;
        }


        public IActionResult Index(int pageIndex)
        {
            IQueryable<Product> query = db.Products.OrderByDescending(m => m.Id);

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
           
            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

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


        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = db.Products.Find(id); if (product == null) { }

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy"); 
                
                return View(productDto);
            }

            // update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // delete the old image
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName; 
                System.IO.File.Delete(oldImageFullPath);
            }

            // update the product in the database
            product.Name = productDto.Name; 
            product.Brand = productDto.Brand; 
            product.Category = productDto.Category; 
            product.Price = productDto. Price; 
            product.Description = productDto.Description; 
            product.ImageFileName = newFileName; 
            
            db.SaveChanges(); 
            
            return RedirectToAction("Index", "Products");
        }


        public IActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName; 
            System.IO.File.Delete(imageFullPath); 

            db.Products.Remove(product); 
            db.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }
    }
}
