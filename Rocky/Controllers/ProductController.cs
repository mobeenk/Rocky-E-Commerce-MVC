using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        // this is able to access root folder
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public DataContext DataContext { get; }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _dataContext.Product
                .Include(c=>c.Category)
                .Include(a=>a.ApplicationType)
                ;
            //foreach(var obj in objList)
            //{
            //    obj.Category = _dataContext.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    obj.ApplicationType = _dataContext.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            //}
            return View(objList);
            
        }

      

        //GET - Upsert
        public IActionResult Upsert(int? id)
        {
    
            //ViewBags used to pass data from controller to view
            //ViewBag.CategoryDropDown = CategoryDropDown;
            //ViewData["CategoryDropDown"] = CategoryDropDown;
            //Product product = new Product();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _dataContext.Category.Select(i =>
                new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }) ,

               ApplicationTypeSelectList = _dataContext.ApplicationType.Select(i =>
               new SelectListItem
               {
                   Text = i.Name,
                   Value = i.Id.ToString()
               })
            };
            // if we click create button
            if(id == null)
            {
                ///create
                return View(productVM);
            }
            //if clicked edit
            else
            {
                productVM.Product = _dataContext.Product.Find(id);
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
                
        }

        //POST - Insert is applied Create on link https://localhost:44310/Product/Upsert then hit create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(productVM.Product.Id == 0)
                {
                    //create
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _dataContext.Product.Add(productVM.Product);

                }
                ///update
                else
                {
                    var updatedProduct = _dataContext.Product
                            .AsNoTracking()
                            .FirstOrDefault(p => p.Id == productVM.Product.Id)
                            ;

                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, updatedProduct.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                       
                    }
                    else
                    {
                        productVM.Product.Image = updatedProduct.Image;
                    }
                    _dataContext.Update(productVM.Product);
                }
                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _dataContext.Category.Select(i =>
                new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

            productVM.ApplicationTypeSelectList = _dataContext.ApplicationType.Select(i =>
                new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

            return View(productVM);

        }
       


        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Product product = _dataContext.Product
                .Include(c => c.Category)
                .Include(c => c.ApplicationType)
                .FirstOrDefault(i => i.Id == id);
  
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        //POST - DELETE
        [HttpPost,ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _dataContext.Product.Find(id);
            string webRootPath = _webHostEnvironment.WebRootPath;
            if (obj == null)
            {
                return NotFound();
            }
            string upload = webRootPath + WC.ImagePath;
           
            

            var oldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _dataContext.Product.Remove(obj);
            _dataContext.SaveChanges();
                return RedirectToAction("Index");
            


        }

    }
}
