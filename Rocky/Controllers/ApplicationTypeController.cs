using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly DataContext _dataContext;

        public ApplicationTypeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _dataContext.ApplicationType.ToList();
            return View(objList);
        }
        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }
        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            _dataContext.ApplicationType.Add(obj);
            _dataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var obj = _dataContext.ApplicationType.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _dataContext.ApplicationType.Update(obj);
                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);

        }


        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var obj = _dataContext.ApplicationType.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _dataContext.ApplicationType.Find(id);
            if (ModelState.IsValid)
            {
                _dataContext.ApplicationType.Remove(obj);
                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);

        }
    }
}
