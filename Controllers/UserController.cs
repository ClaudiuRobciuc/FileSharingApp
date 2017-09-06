using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ConsoleApplication.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager; 
        private readonly RoleManager<ApplicationRole> roleManager; 
   
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) 
        { 
            this.userManager = userManager; 
            this.roleManager = roleManager; 

        } 

        [HttpGet] 
        public IActionResult Index() 
        { 
            List<UserListView> model = new List<UserListView>(); 
            model = userManager.Users.Select(u => new UserListView
            { 
                Id = u.Id, 
                Name = u.Name, 
                Email = u.Email 
            }).ToList(); 
            return View(model); 
        }

        [HttpGet] 
        public IActionResult AddUser() 
        { 
            User model = new User(); 
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem 
            { 
                Text = r.Name, 
                Value = r.Id 
            }).ToList(); 
            return View(model); 
        } 
   
        [HttpPost] 
        public async Task<IActionResult> AddUser(User model) 
        { 
            if (ModelState.IsValid) 
            { 
                ApplicationUser user = new ApplicationUser 
                { 
                    Name = model.Name, 
                    UserName = model.UserName, 
                    Email = model.Email 
                }; 
                IdentityResult result = await userManager.CreateAsync(user, model.Password); 
                if (result.Succeeded) 
                { 
                    ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId); 
                    if (applicationRole != null) 
                    { 
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name); 
                        if (roleResult.Succeeded) 
                        { 
                            return RedirectToAction("Index"); 
                        } 
                    } 
                } 
            } 
            return View(model); 
        }
        [HttpGet] 
        public async Task<IActionResult> DeleteUser(string id) 
        { 
            string name = string.Empty; 
            
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id); 
                if (applicationUser != null) 
                { 
                    name = applicationUser.Name; 
                } 
                return PartialView("_DeleteUser", applicationUser); 
        } 
   
        [HttpPost] 
        public async Task<IActionResult> DeleteUser(string id, IFormCollection form) 
        { 
            if (!String.IsNullOrEmpty(id)) 
            { 
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id); 
                if (applicationUser != null) 
                { 
                    IdentityResult result = await userManager.DeleteAsync(applicationUser);  
                    if (result.Succeeded) 
                    { 
                        return RedirectToAction("Index"); 
                    } 
                } 
            } 
            return RedirectToAction("Index"); 
        }
    }
}