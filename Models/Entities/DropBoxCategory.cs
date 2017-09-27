using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication.Models.Entities
{
    public class DropBoxCategory
    {
        [Key]
        public int CategoryID { get; set; }
        [Required]
        [Display(Name = "Format")]
        public string CategoryType {get; set;}
        public bool Selected {get; set;} = false;
       
       
    }
}