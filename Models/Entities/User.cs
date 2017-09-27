using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication.Models.Entities 
{
    public class User
    {      
        public string UserID { get; set; } 
        [Display(Name="UserName")]
        public string UserName { get; set; } 
        [DataType(DataType.Password)] 
        public string Password { get; set; } 
        [Display(Name="Name")] 
        public string Name { get; set; } 
        public string dropBoxApi {get; set;}
        public string Email { get; set; } 
        [NotMapped]
        public List<SelectListItem> ApplicationRoles { get; set; } 
        [Display(Name = "Role")] 
        public string ApplicationRoleId { get; set; } 
        
    
    }
}