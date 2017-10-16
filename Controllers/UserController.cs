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
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Controllers
{
    [Authorize(Roles="Admin")]
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

        [HttpGet] 
        public async Task<IActionResult> EditUser(string id) 
        { 
            EditUserViewModel model = new EditUserViewModel(); 
            model.ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem 
            { 
                Text = r.Name, 
                Value = r.Id 
            }).ToList(); 
   
            if (!String.IsNullOrEmpty(id)) 
            { 
                ApplicationUser user = await userManager.FindByIdAsync(id); 
                if (user != null) 
                { 
                    model.Name = user.Name; 
                    model.Email = user.Email; 
                    model.ApplicationRoleId = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Id; 
                } 
            } 
            return PartialView("_EditUser", model); 
        } 
   
        [HttpPost] 
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model) 
        { 
            if (ModelState.IsValid) 
            { 
                ApplicationUser user = await userManager.FindByIdAsync(id); 
                if (user != null) 
                { 
                    user.Name = model.Name; 
                    user.Email = model.Email; 
                    string existingRole = userManager.GetRolesAsync(user).Result.Single(); 
                    string existingRoleId = roleManager.Roles.Single(r => r.Name == existingRole).Id; 
                    IdentityResult result = await userManager.UpdateAsync(user); 
                    if (result.Succeeded) 
                    { 
                        if (existingRoleId != model.ApplicationRoleId) 
                        { 
                            IdentityResult roleResult = await userManager.RemoveFromRoleAsync(user, existingRole); 
                            if (roleResult.Succeeded) 
                            { 
                                ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId); 
                                if (applicationRole != null) 
                                { 
                                    IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name); 
                                    if (newRoleResult.Succeeded) 
                                    { 
                                        return RedirectToAction("Index"); 
                                    } 
                                } 
                            } 
                        }
                        else
                        {
                           return RedirectToAction("Index");     
                        } 
                    } 
                } 
            } 
         
            return PartialView("_EditUser", model); 
        }
    }
}