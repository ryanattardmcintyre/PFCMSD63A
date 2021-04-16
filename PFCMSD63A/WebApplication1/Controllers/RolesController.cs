using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public RolesController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AllocateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AllocateRole(string role)
        {

            if (role == "DRIVER" || role == "PASSENGER" || role == "USER")
            {
                var currentLoggedInUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                await _userManager.AddToRoleAsync(currentLoggedInUser, role);

            }
            return View();
        }
    }
}
