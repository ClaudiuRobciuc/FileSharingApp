using System.Collections.Generic;
using ConsoleApplication.Models.Entities;

namespace ConsoleApplication.Models.ViewModels
{
    public class CategoriesItemsViewModel
    {
        public List<Category> Categories { get; set; }
        public IEnumerable<Items> Items { get; set; }
        public List<string> GroupTags {get; set;}

        
    }
}