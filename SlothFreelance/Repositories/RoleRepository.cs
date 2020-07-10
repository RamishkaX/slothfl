using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SlothFreelance.Repositories
{
    public class RoleRepository : IRepository<Roles>
    {
        private SFContext db;

        public RoleRepository(SFContext context)
        {
            db = context;
        }

        public void AddNewItem(Roles item)
        {
            db.Roles.Add(item);
        }

        public void DeleteItem(int? id)
        {
            Roles roles = db.Roles.Find(id);

            if (roles != null)
            {
                db.Roles.Remove(roles);
            }
        }

        public IEnumerable<Roles> GetAll()
        {
            return db.Roles.Include(c => c.Users).ToList();
        }

        public Roles GetItemById(int? id)
        {
            return db.Roles.Include(c => c.Users).FirstOrDefault(c => c.RoleId == id);
        }

        public void UpdateItem(Roles item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}