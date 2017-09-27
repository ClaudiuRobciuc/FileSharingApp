using System.Collections.Generic;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public interface ICategoryRepositoryDB
    {
        
       IEnumerable<DropBoxCategory> GetAll();
       DropBoxCategory Get(int id);
       void Delete(DropBoxCategory s);
       void Update(DropBoxCategory s);
       void Save(DropBoxCategory s);
       DropBoxCategory GetByName(string s);
       int Size();

    }
}
