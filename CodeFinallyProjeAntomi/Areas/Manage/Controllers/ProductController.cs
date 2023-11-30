using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    //[Authorize(Roles = "SuperAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index( int currentPage=1)
        {
            IQueryable<Product> products = _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.Brand)
                .Include(p=>p.productTags.Where(pt=>pt.IsDeleted==false)).ThenInclude(pt=>pt.Tag)
                .Where(c => c.IsDeleted == false)
                .OrderByDescending(c => c.Id);


            return View(PageNatedList<Product>.Create(products,currentPage,6,10));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product? product = await _context.Products
                 .Include(p => p.productImages.Where(pi => pi.IsDeleted == false))
                 .Include(c=>c.Category).Where(c=>c.IsDeleted==false && c.Id==id)
                 .Include(c => c.Brand).Where(b => b.IsDeleted == false && b.Id == id)
                 .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if(product == null) return NotFound();


            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories=await _context.Categories.Where(c=>c.IsDeleted==false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            //if (!ModelState.IsValid) return View(product);

            if (product.CategoryId == null || !await  _context.Categories.AnyAsync(c=>c.IsDeleted==false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is inCorrect");
                return View(product);
            }

            if(product.BrandId !=null && !await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand is inCorrect");
                return View(product);
            }

            List<ProductTag>productTags=new List<ProductTag>();

            if(product.TagId1 != null && product.TagId1.Count() > 0)
            {
                foreach (int tagId in product.TagId1)
                {
                    if(!await _context.Tags.AnyAsync(t=>t.IsDeleted ==false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagId1", "Tag is inCorrect");
                        return View(product);
                    }

                    ProductTag productTag=new ProductTag
                    {
                        TagId= tagId,
                    };
                    productTags.Add(productTag);
                }
            }

            product.productTags=productTags;

            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "value cannot be less than 0");
                return View(product);
            }

            if(product.DisCountPrice<= 0)
            {
                ModelState.AddModelError("DisCountPrice", "value cannot be less than 0");
                return View(product);
            }

            if(product.Price <= product.DisCountPrice) 
            {
                ModelState.AddModelError("DisCountPrice", "it cannot be bigger than the discount price ");
                return View(product);
            }

            if(product.Files == null)
            {
                ModelState.AddModelError("Files", "Min 1 file entery plese");
                return View(product);
            }

            if(product.Files.Count() > 10)
            {
                ModelState.AddModelError("Files", "Miximum 10 file");
                return View(product);
            }

            foreach (IFormFile file in product.Files)
            {
                if (!file.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Files", "File not found");
                    return View(product);
                }

                if ((file.Length / 1024) > 400)
                {
                    ModelState.AddModelError("Files", "File must be 400kb");
                    return View(product);
                }
            }

            List<ProductImage> productImages=new List<ProductImage>();

            foreach (IFormFile file in product.Files)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + file.FileName
                   .Substring(file.FileName.LastIndexOf('.'));

                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                ProductImage productImage = new ProductImage
                {
                    Image = fileName,
                };
                productImages.Add(productImage);
            }

            product.productImages=productImages;

            if (product.HoverFile != null)
            {

                if (!product.HoverFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("HoverFile", "HoverFile not found");
                    return View(product);
                }

                if ((product.HoverFile.Length / 1024) > 400)
                {
                    ModelState.AddModelError("HoverFile", "HoverFile must be 400kb");
                    return View(product);
                }
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + product.HoverFile.FileName
                .Substring(product.HoverFile.FileName.LastIndexOf('.'));

                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", "Hover-image", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.HoverFile.CopyToAsync(fileStream);
                }

                product.HoverImage = fileName;
            }
            else
            {
                ModelState.AddModelError("HoverFile", "HoverFile is required");
                return View(product);
            }

            if (product.MainFile != null)
            {

                if (!product.MainFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Files", "File not found");
                    return View(product);
                }

                if ((product.MainFile.Length / 1024) > 400)
                {
                    ModelState.AddModelError("Files", "File must be 400kb");
                    return View(product);
                }
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + product.MainFile.FileName
                .Substring(product.MainFile.FileName.LastIndexOf('.'));

                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", "Main-image", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.MainFile.CopyToAsync(fileStream);
                }

                product.MainImage = fileName;
            }
            else
            {
                ModelState.AddModelError("MainFile", "MainFile is required");
                return View(product);
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            Brand brand=await _context.Brands.FirstOrDefaultAsync(b=>b.Id == product.BrandId);

            string seria = (category.Name.Substring(0, 2) + brand.Name.Substring(0, 2)).ToLower();

            Product product1 = await _context.Products.Where(c => c.Seria.ToLower() == seria).OrderByDescending(c=>c.Number).FirstOrDefaultAsync();

            int? num = product1 !=null ? product1.Number+1 :1;

            product.Seria=seria;
            product.Number= num;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if(id == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p=>p.productImages.Where(pi=>pi.IsDeleted==false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted==false);

            if(product == null) return NotFound();
            product.TagId1 = await _context.ProductTags
                .Where(t => t.ProductId == product.Id && t.IsDeleted == false)
                .Select(c => c.TagId)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id,Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            if(id==null) return NotFound();

            if (product.Id != id) return BadRequest();

            Product? dbProduct=await _context.Products
                .Include(c=>c.productImages.Where(pi=>pi.IsDeleted==false))
                .Include(c=>c.productTags.Where(pt=>pt.IsDeleted==false))
                .FirstOrDefaultAsync(p=>p.Id==id && p.IsDeleted==false);

            if(dbProduct ==null) return NotFound();

            dbProduct.TagId1 = product.TagId1;

            int Upload=10-dbProduct.productImages.Count;

            if(product.Files != null && product.Files.Count() > Upload)
            {
                ModelState.AddModelError("Files", $"max {Upload}");
                return View(dbProduct);
            }

            if(product.CategoryId ==null || !await _context.Categories.AnyAsync(c=>c.IsDeleted==false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is Incorrect");
                return View(dbProduct);
            }
            if (product.BrandId != null && !await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand is Incorrect");
                return View(dbProduct);
            }

            if(dbProduct.productTags != null && dbProduct.productTags.Count > 0)
            {
                foreach (ProductTag productTag in dbProduct.productTags)
                {
                    product.IsDeleted = true;
                    productTag.DeletedAt= DateTime.UtcNow;
                    productTag.CreatedBy = "Amrah";
                }
            }

            List<ProductTag> productTags = new List<ProductTag>();

            if (product.TagId1 != null && product.TagId1.Count() > 0)
            {
                foreach (int tagId in product.TagId1)
                {
                    if (!await _context.Tags.AnyAsync(t => t.IsDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagId1", "Tag is inCorrect");
                        return View(dbProduct);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                    };
                    productTags.Add(productTag);
                }
            }


            dbProduct.productTags.AddRange(productTags);


            if (product.HoverFile != null)
            {

                if (!product.HoverFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("HoverFile", "HoverFile not found");
                    return View(dbProduct);
                }

                if ((product.HoverFile.Length / 1024) > 400)
                {
                    ModelState.AddModelError("HoverFile", "HoverFile must be 400kb");
                    return View(dbProduct);
                }
                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", "Hover-image", dbProduct.HoverImage);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + product.HoverFile.FileName
                .Substring(product.HoverFile.FileName.LastIndexOf('.'));

                filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", "Hover-image", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.HoverFile.CopyToAsync(fileStream);
                }

                dbProduct.HoverImage = fileName;
            }

            if (product.MainFile != null)
            {

                if (!product.MainFile.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Files", "File not found");
                    return View(product);
                }

                if ((product.MainFile.Length / 1024) > 400)
                {
                    ModelState.AddModelError("Files", "File must be 400kb");
                    return View(product);
                }

                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", "Main-image", dbProduct.MainImage);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + product.MainFile.FileName
                .Substring(product.MainFile.FileName.LastIndexOf('.'));

                filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", "Main-image", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.MainFile.CopyToAsync(fileStream);
                }

                dbProduct.MainImage = fileName;
            }

            if(product.Files != null)
            {
                foreach (IFormFile file in product.Files)
                {
                    if (!file.ContentType.Contains("image/"))
                    {
                        ModelState.AddModelError("Files", "File not found");
                        return View(dbProduct);
                    }

                    if ((file.Length / 1024) > 400)
                    {
                        ModelState.AddModelError("Files", "File must be 400kb");
                        return View(dbProduct);
                    }
                }

                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + file.FileName
                       .Substring(file.FileName.LastIndexOf('.'));

                    string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Product", fileName);

                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = fileName,
                    };
                    productImages.Add(productImage);
                }

                dbProduct.productImages.AddRange(productImages);
            }
            dbProduct.Title= product.Title.Trim();
            dbProduct.CategoryId= product.CategoryId;
            dbProduct.BrandId= product.BrandId;
            dbProduct.Price= product.Price;
            dbProduct.DisCountPrice= product.DisCountPrice;
            dbProduct.Description= product.Description;
            dbProduct.ExTag= product.ExTag;
            dbProduct.Count= product.Count;
            dbProduct.NewArrival= product.NewArrival;
            dbProduct.BestSeller= product.BestSeller;
            dbProduct.Featured= product.Featured;
            dbProduct.SmallDescription= product.SmallDescription;
            dbProduct.UpdatedBy = "Amrah";
            dbProduct.UpdatedAt= DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> deleteImage(int? id, int? imageId)
        {
            if(id==null) return BadRequest();

            if(imageId==null) return BadRequest();  

            Product? product=await _context.Products
                .Include(p=>p.productImages.Where(pi=>pi.IsDeleted==false))
                .FirstOrDefaultAsync(p=>p.Id==id && p.IsDeleted == false);

            if(product==null) return NotFound();    

            if(!product.productImages.Any(pi=>pi.Id==imageId)) return BadRequest();

            if(product.productImages.Count()<=1) return BadRequest();

            product.productImages.FirstOrDefault(pi=>pi.Id==imageId).IsDeleted=true;
            product.productImages.FirstOrDefault(pi => pi.Id == imageId).DeletedBy = "Amrah";
            product.productImages.FirstOrDefault(pi => pi.Id == imageId).DeletedAt = DateTime.Now;

            string fileName = product.productImages.FirstOrDefault(pi => pi.Id == imageId).Image;

            await _context.SaveChangesAsync();

            string filePath = Path.Combine(_env.WebRootPath, "assets", "Photos", "Product",fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return PartialView("_DeleteImagePartial",product.productImages.Where(p=>p.IsDeleted==false).ToList());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null) return NotFound();

            Product? product = await _context.Products
           .Include(p => p.productImages.Where(pi => pi.IsDeleted == false))
           .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            product.IsDeleted = true;
            product.DeletedBy = "Amrah";
            product.DeletedAt= DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
