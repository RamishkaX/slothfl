using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SlothFreelance.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private SFContext db;

        public CategoryRepository(SFContext context)
        {
            db = context;
        }

        public void AddNewItem(Categories item)
        {
            db.Categories.Add(item);
        }

        public void DeleteItem(int? id)
        {
            Categories categories = db.Categories.Find(id);

            if (categories != null)
            {
                db.Categories.Remove(categories);
            }
        }

        public IEnumerable<Categories> GetAll()
        {
            return db.Categories.ToList();
        }

        public Categories GetCategoryByIdWithJoin(int? id, bool withTask = true)
        {
            if (withTask)
                return db.Categories.Include(c => c.Tasks).FirstOrDefault(c => c.CategoryId == id);
            else
                return GetItemById(id);
        }

        public Categories GetItemById(int? id)
        {
            return db.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void UpdateItem(Categories item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}