using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogsRepository _blogsRepo;
        public BlogsController(IBlogsRepository blogsRepo)
        {
            _blogsRepo = blogsRepo;
        }

        public IActionResult Index()
        {
            var list = _blogsRepo.GetBlogs();
           
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Blog b)
        {
            try
            {
                _blogsRepo.AddBlog(b);
                TempData["message"] = $"Blog {b.Url} was created successfully";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Blog wasnt created successfully";
                return View(b);
            }
        
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _blogsRepo.DeleteBlog(id);
                TempData["message"] = $"Blog was deleted successfully";
            }
            catch (Exception 
            e)
            {
                TempData["error"] = "Blog wasnt deleted successfully";
            }

            return RedirectToAction("Index");

        }

    }
}
