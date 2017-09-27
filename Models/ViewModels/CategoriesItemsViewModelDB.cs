using System.Collections.Generic;
using ConsoleApplication.Models.Entities;

namespace ConsoleApplication.Models.ViewModels
{
    public class CategoriesItemsViewModelDB
    {
        public List<DropBoxCategory> Categories { get; set; }
        public IEnumerable<DropBoxItems> Items { get; set; }

        
    }
}