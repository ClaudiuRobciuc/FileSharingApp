using System.Collections.Generic;
using ConsoleApplication.Models.Entities;

namespace ConsoleApplication.Models.ViewModels
{
    public class ItemViewModelDB
    {
        public List<DropBoxCategory> Categories { get; set; }
        public DropBoxItems Items { get; set; }

        
    }
}