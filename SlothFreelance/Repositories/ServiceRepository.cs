using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SlothFreelance.Repositories
{
    public class ServiceRepository : IRepository<Services>
    {
        private SFContext db;

        public ServiceRepository(SFContext context)
        {
            db = context;
        }
        public void AddNewItem(Services item)
        {
            db.Services.Add(item);
        }

        public void DeleteItem(int? id)
        {
            Services service = db.Services.Find(id);

            if (service != null)
            {
                db.Services.Remove(service);
            }
        }

        public IEnumerable<Services> GetAll()
        {
            return db.Services.ToList();
        }

        public Services GetItemById(int? id)
        {
            return db.Services.FirstOrDefault(c => c.ServiceId == id);
        }

        public void UpdateItem(Services item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}