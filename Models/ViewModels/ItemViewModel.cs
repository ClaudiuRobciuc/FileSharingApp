using System.Collections.Generic;
using ConsoleApplication.Models.Entities;

namespace ConsoleApplication.Models.ViewModels
{
    public class ItemViewModel
    {
        public List<Category> Categories { get; set; }
        public Items Items { get; set; }

        
    }
}