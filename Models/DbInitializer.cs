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
using Microsoft.EntityFrameworkCore;
using Dropbox.Api;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Text;
using Dropbox.Api.Files;

namespace ConsoleApplication.Models.ViewModels 
{
    public enum Roles
   {
      Admin,
     
   }
    public static class DbInitializer
    {
        public async static void Initialize(MyDbContext context) 
        {
            
            //context.Database.EnsureCreated();
            context.Database.Migrate();
    
            if (context.Items.Any()&&context.DropBoxItems.Any())
            {
                return;   // DB has been seeded
            }
        

            ApplicationRole applicationRole = new ApplicationRole {
                CreatedDate = DateTime.UtcNow
            };
           
           
            context.SaveChanges();
            /*var task = Task.Run((Func<Task>)DbInitializer.Run);
            task.Wait();*/
            using (var dbx = new DropboxClient("MgX6Ia7UK1AAAAAAAAAACUxEgjuhsT4DtHykvshYVhkO5EtLqHoZOvPVuG4NJ-L2"))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine(full.Email);
                await ListRootFolder(dbx, context);
                //await Download(dbx,"","Get Started with Dropbox.pdf");
            } 
            
        }
        
        /*static async Task Run()
        {
            using (var dbx = new DropboxClient("MgX6Ia7UK1AAAAAAAAAACUxEgjuhsT4DtHykvshYVhkO5EtLqHoZOvPVuG4NJ-L2"))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine(full.Email);
                await ListRootFolder(dbx, context);
                //await Download(dbx,"","Get Started with Dropbox.pdf");
            }
        }*/
        static async Task ListRootFolder(DropboxClient dbx, MyDbContext context)
        {
            List<DropBoxCategory> categories = new List<DropBoxCategory>();
            var list = await dbx.Files.ListFolderAsync(string.Empty);
            foreach (var file in list.Entries.Where(i => i.IsFile))
            {
                bool cExist= false;
                DropBoxItems item = new DropBoxItems();
                String fName = file.Name;
                String aux = fName.Substring(0,(fName.IndexOf('.')));
                String form = fName.Substring(fName.IndexOf('.'),(fName.Length-fName.IndexOf('.')));
                //item = new Items{Title=aux,Format=form, Path=Path.Combine(uploads,file.FileName),date=System.DateTime.Now.ToString()};
                item.Title=aux;
                item.Format=form;
                item.date = System.DateTime.UtcNow.ToString();
                item.Tags = "DropBoxFile";
                item.Path = "/"+fName;
                var author = await dbx.Users.GetCurrentAccountAsync();
                item.Author = author.Name.ToString();
                context.DropBoxItems.Add(item);
                DropBoxCategory category = new DropBoxCategory{CategoryType=form};
                categories.Add(category);
                for(int i=0; i< (categories.Count()-1)&&(cExist==false); i++)
                {
                    if(categories[i].CategoryType.Equals(form))
                    {
                        cExist=true;
                        categories.Remove(categories[i]);
                    }
                }
                if(cExist==false)
                {   
                    context.DropBoxCategory.Add(category);
                }
            }
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


