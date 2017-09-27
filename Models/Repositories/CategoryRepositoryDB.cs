using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public class CategoryRepositoryDB : ICategoryRepositoryDB
    {
        private MyDbContext _db;
        private DbSet<DropBoxCategory> _Category;

        public CategoryRepositoryDB(MyDbContext db)
        {
            _db = db;
            _Category = db.DropBoxCategory;
        }
        public void Delete(DropBoxCategory s)
        {
            DropBoxCategory category = _db.DropBoxCategory.Find(s.CategoryID);
            _db.DropBoxCategory.Remove(category);
            _db.SaveChanges();
        }

        public DropBoxCategory Get(int id)
        {
            DropBoxCategory category =  _db.DropBoxCategory.Where(r => r.CategoryID == id).FirstOrDefault();
            return category;
        }

        public IEnumerable<DropBoxCategory> GetAll()
        {
            IEnumerable<DropBoxCategory> categories = _db.DropBoxCategory;
            return categories;
        }

        public void Save(DropBoxCategory category)
        {
            _db.DropBoxCategory.Add(category);
            _db.SaveChanges();
        }

        public void Update(DropBoxCategory category)
        {
            _db.DropBoxCategory.Update(category);
            _db.SaveChanges();
        }
        public DropBoxCategory GetByName(string s)
        {
            DropBoxCategory category = _db.DropBoxCategory.Where(r => r.CategoryType.Equals(s)).FirstOrDefault();
            return category;
        }
        public int Size()
        {
            int i=0;
            IEnumerable<DropBoxCategory> categories = _db.DropBoxCategory;
            foreach(DropBoxCategory c in categories)
            {
                i++;
            }
            return i;
        }
    }
}