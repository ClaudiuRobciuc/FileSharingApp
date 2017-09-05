using System.Collections.Generic;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public interface IItemsRepository
    {
        
       IEnumerable<Items> GetAll();
       Items Get(int id);
       void Delete(Items s);
       void Update(Items s);
       void Save(Items s);
       Items GetLastItem();

    }
}
