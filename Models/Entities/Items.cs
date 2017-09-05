using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApplication.Models.Entities 
{
    public class Items
    {      
        [Key]
       public int ItemID { get; set; }
       [Required]
        
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Tags")]
        [Required]
        public string Tags {get; set;} 
        [Display(Name = "Link")]
        public string Link {get; set;}
        public string Format {get; set;}
        public string Path {get; set;}
        public string date{get; set;}
        public string Author{get; set;}
    }
}