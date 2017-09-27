using System.Collections.Generic;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public interface IItemsRepositoryDB
    {
        
       IEnumerable<DropBoxItems> GetAll();
       DropBoxItems Get(int id);
       void Delete(DropBoxItems s);
       void Update(DropBoxItems s);
       void Save(DropBoxItems s);
       DropBoxItems GetLastItem();
       int Size();

    }
}
