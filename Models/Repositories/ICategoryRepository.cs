using System.Collections.Generic;
using ConsoleApplication.Models.Entities;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public interface ICategoryRepository
    {
        
       IEnumerable<Category> GetAll();
       Category Get(int id);
       void Delete(Category s);
       void Update(Category s);
       void Save(Category s);
       Category GetByName(string s);
       int Size();

    }
}
