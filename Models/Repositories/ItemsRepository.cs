using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        private MyDbContext _db;
        private DbSet<Items> _items;

        public ItemsRepository(MyDbContext db)
        {
            _db = db;
            _items = db.Items;
        }
        public void Delete(Items s)
        {
            Items items = _db.Items.Find(s.ItemID);
            _db.Items.Remove(items);
            _db.SaveChanges();
        }

        public Items Get(int id)
        {
            Items items =  _db.Items.Where(r => r.ItemID == id).FirstOrDefault();
            return items;
        }

        public Items GetLastItem()
        {
            Items item = new Items();
            foreach(Items i in _db.Items)
            {
                item=i;
            }
            return item;
        }

        public IEnumerable<Items> GetAll()
        {
            IEnumerable<Items> items = _db.Items;
            return items;
        }

        public void Save(Items items)
        {
            _db.Items.Add(items);
            _db.SaveChanges();
        }

        public void Update(Items Items)
        {
            _db.Items.Update(Items);
            _db.SaveChanges();
        }
    }
}