using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ConsoleApplication.Models.ViewModels;

namespace ConsoleApplication.Models.Repositories
{
    public class ItemsRepositoryDB : IItemsRepositoryDB
    {
        private MyDbContext _db;
        private DbSet<DropBoxItems> _items;

        public ItemsRepositoryDB(MyDbContext db)
        {
            _db = db;
            _items = db.DropBoxItems;
        }
        public void Delete(DropBoxItems s)
        {
            DropBoxItems items = _db.DropBoxItems.Find(s.ItemID);
            _db.DropBoxItems.Remove(items);
            _db.SaveChanges();
        }

        public DropBoxItems Get(int id)
        {
            DropBoxItems items =  _db.DropBoxItems.AsNoTracking().Where(r => r.ItemID == id).FirstOrDefault();
            return items;
        }

        public DropBoxItems GetLastItem()
        {
            DropBoxItems item = new DropBoxItems();
            foreach(DropBoxItems i in _db.DropBoxItems)
            {
                item=i;
            }
            return item;
        }

        public IEnumerable<DropBoxItems> GetAll()
        {
            IEnumerable<DropBoxItems> items = _db.DropBoxItems.AsNoTracking();
            return items;
        }

        public void Save(DropBoxItems items)
        {
            _db.DropBoxItems.Add(items);
            _db.SaveChanges();
        }

        public void Update(DropBoxItems Items)
        {
            _db.DropBoxItems.Update(Items);
            _db.SaveChanges();
        }
        public int Size()
        {
            int i=0;
            IEnumerable<DropBoxItems> items = _db.DropBoxItems;
            foreach(DropBoxItems c in items)
            {
                i++;
            }
            return i;
        }
    }
}