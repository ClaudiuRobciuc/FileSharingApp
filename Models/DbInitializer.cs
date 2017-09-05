using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using ConsoleApplication.Models.ViewModels;
using ConsoleApplication.Models.Repositories;
using ConsoleApplication.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ConsoleApplication.Models.ViewModels 
{
    public enum Roles
   {
      Admin,
     
   }
    public static class DbInitializer
    {
        public static void Initialize(MyDbContext context) 
        {
            
            context.Database.EnsureCreated();
    
            
            if (context.Items.Any())
            {
                return;   // DB has been seeded
            }
        
            List<Items> items = new List<Items>()
            {
                
            new Items{Title="Aici", Description="Descr", Tags="tag", Format=".txt", Path="/Files/Aici.txt"}
           
            };
            foreach (Items s in items)
            {
                context.Items.Add(s);
            } 

            var category = new Category[]
            {
            
            new Category{CategoryType = ".txt"},
            };

            foreach (Category c in category)
            {
                context.Category.Add(c);
            }
            ApplicationRole applicationRole = new ApplicationRole {
                CreatedDate = DateTime.UtcNow
            };
            
            context.SaveChanges();

        }
        public static async Task SeedRolesUsers(IServiceProvider serviceProvider)
        {
                Role aux = new Role{RoleName="Admin"};
                 ApplicationRole applicationRole =  
               new ApplicationRole 
               {                    
                   CreatedDate = DateTime.UtcNow                    
               }; 
                applicationRole.Name = aux.RoleName; 
                
                applicationRole.IPAddress ="10.10.10.10";
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                await roleManager.CreateAsync(applicationRole);

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                ApplicationUser user = new ApplicationUser 
                { 
                    Name = "Claudiu Robciuc", 
                    UserName = "Admin", 
                    Email = "claudiu1robciuc@yahoo.com" 
                }; 
              await userManager.CreateAsync(user, "Cclaudiu2@"); 
              await userManager.AddToRoleAsync(user, applicationRole.Name); 
                        
            
        }            
    }
}


