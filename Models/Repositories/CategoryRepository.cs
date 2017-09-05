using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private MyDbContext _db;
        private DbSet<Category> _Category;

        public CategoryRepository(MyDbContext db)
        {
            _db = db;
            _Category = db.Category;
        }
        public void Delete(Category s)
        {
            Category category = _db.Category.Find(s.CategoryID);
            _db.Category.Remove(category);
            _db.SaveChanges();
        }

        public Category Get(int id)
        {
            Category category =  _db.Category.Where(r => r.CategoryID == id).FirstOrDefault();
            return category;
        }

        public IEnumerable<Category> GetAll()
        {
            IEnumerable<Category> categories = _db.Category;
            return categories;
        }

        public void Save(Category category)
        {
            _db.Category.Add(category);
            _db.SaveChanges();
        }

        public void Update(Category category)
        {
            _db.Category.Update(category);
            _db.SaveChanges();
        }
        public Category GetByName(string s)
        {
            Category category = _db.Category.Where(r => r.CategoryType.Equals(s)).FirstOrDefault();
            return category;
        }
        public int Size()
        {
            int i=0;
            IEnumerable<Category> categories = _db.Category;
            foreach(Category c in categories)
            {
                i++;
            }
            return i;
        }
    }
}