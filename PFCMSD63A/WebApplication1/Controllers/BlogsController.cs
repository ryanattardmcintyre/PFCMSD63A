using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models.Domain;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogsRepository _blogsRepo;
        private readonly IConfiguration _config;
        public BlogsController(IBlogsRepository blogsRepo, IConfiguration config)
        {
            _config = config;
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
        public IActionResult Create(IFormFile logo, Blog b)
        {
            try
            {
                string bucketName = _config.GetSection("AppSettings").GetSection("BucketName").Value;
                string uniqueFilename = Guid.NewGuid() + System.IO.Path.GetExtension(logo.FileName);

                //upload a file in the cloud storage
                var storage = StorageClient.Create();

                using (var myStream = logo.OpenReadStream())
                {
                    storage.UploadObject(bucketName, uniqueFilename, null, myStream);
                }

                //add a reference to the file url with the instance of the blog >>> b

                b.Url = $"https://storage.googleapis.com/{bucketName}/{uniqueFilename}";

                //save everything in db
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
