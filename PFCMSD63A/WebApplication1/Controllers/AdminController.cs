using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles ="ADMIN")]
    public class AdminController : Controller
    {

        private readonly ICacheRepository _cacheRepo;
        public AdminController (ICacheRepository cacheRepo)
        {
            _cacheRepo = cacheRepo;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Menu m)
        {
            _cacheRepo.UpsertMenu(m);
            return View();
        }
    }
}
