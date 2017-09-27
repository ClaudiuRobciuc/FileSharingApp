using ConsoleApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace ConsoleApplication.Models.ViewModels 
{ 
    public class MyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Items> Items {get; set;}
        public DbSet<DropBoxItems> DropBoxItems {get; set;}
        public DbSet<Category> Category { get; set; }
         public DbSet<DropBoxCategory> DropBoxCategory { get; set; }
        public DbSet<User> User{get; set;}
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlite("Filename=./mydb.db");
        } 

    }
}