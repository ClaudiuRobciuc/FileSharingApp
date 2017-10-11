using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Models.ViewModels;
using ConsoleApplication.Models.Repositories;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ConsoleApplication.Controllers
{
   [Authorize(Roles="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager; 
        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpGet] 
        public IActionResult Index() 
        { 
            List<Role> model = new List<Role>(); 
            model = roleManager.Roles.Select(r => new Role 
            { 
                RoleName = r.Name, 
                Id = r.Id, 
            }).ToList(); 
         return View(model); 
        }

        [HttpGet] 
        public async Task<IActionResult> AddRole(string id) 
        { 
            Role model = new Role(); 
            if (!String.IsNullOrEmpty(id)) 
            { 
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id); 
                if (applicationRole != null) 
                { 
                    model.Id = applicationRole.Id; 
                    model.RoleName = applicationRole.Name;  
                } 
            } 
            return View(model); 
        } 
        [HttpPost] 
        public async Task<IActionResult> AddRole(string id, Role model) 
        { 
            if (ModelState.IsValid) 
            { 
                bool isExist = !String.IsNullOrEmpty(id); 
                ApplicationRole applicationRole = isExist ? await roleManager.FindByIdAsync(id) : 
               new ApplicationRole 
               {                    
                   CreatedDate = DateTime.UtcNow                    
               }; 
                applicationRole.Name = model.RoleName; 
                applicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(); 
                IdentityResult roleRuslt = isExist?  await roleManager.UpdateAsync(applicationRole) 
                                                    : await roleManager.CreateAsync(applicationRole); 
                if (roleRuslt.Succeeded) 
                { 
                    return RedirectToAction("Index"); 
                } 
            } 
            return View(model); 
        }
        [HttpGet] 
        public async Task<IActionResult> DeleteRole(string id) 
        { 
            string name = string.Empty; 
            ApplicationRole applicationRole = new ApplicationRole();
            if (!String.IsNullOrEmpty(id)) 
            { 
                applicationRole = await roleManager.FindByIdAsync(id); 
                if (applicationRole != null) 
                { 
                    name = applicationRole.Name; 
                } 
            } 
            ApplicationRoleView applicationRoleView = new ApplicationRoleView();
            applicationRoleView.Id= id;
            applicationRoleView.Name=name;
            return PartialView("_DeleteRole", applicationRoleView); 
        } 
   
        [HttpPost] 
        public async Task<IActionResult> DeleteRole(ApplicationRoleView application) 
        { 
            string id = application.Id;
            if(!String.IsNullOrEmpty(id)) 
            { 
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id); 
                if (applicationRole != null) 
                { 
                    IdentityResult roleRuslt = roleManager.DeleteAsync(applicationRole).Result; 
                    if (roleRuslt.Succeeded) 
                    { 
                        return RedirectToAction("Index"); 
                    } 
                } 
                } 
            return RedirectToAction("Index"); 
        }
    }
}