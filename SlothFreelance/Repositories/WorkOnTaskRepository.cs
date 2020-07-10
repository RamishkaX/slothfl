using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SlothFreelance.Repositories
{
    public class WorkOnTaskRepository : IWorkOnTaskRepository
    {
        private SFContext db;

        public WorkOnTaskRepository(SFContext context)
        {
            db = context;
        }

        public void AddNewItem(WorkOnTask item)
        {
            db.WorkOnTasks.Add(item);
        }

        public void DeleteItem(int? id)
        {
            WorkOnTask work = db.WorkOnTasks.Find(id);

            if (work != null)
            {
                db.WorkOnTasks.Remove(work);
            }
        }

        public IEnumerable<WorkOnTask> GetAll()
        {
            return db.WorkOnTasks.ToList();
        }

        public WorkOnTask GetItemById(int? id)
        {
            return db.WorkOnTasks.FirstOrDefault(c => c.WorkId == id);
        }

        public WorkOnTask GetWorkByIdWithJoin(int? id, bool withTask, bool withUser)
        {
            if (withTask && withUser)
                return db.WorkOnTasks.Include(c => c.Tasks).Include(c => c.Users).FirstOrDefault(c => c.WorkId == id);
            else if (withTask && !withUser)
                return db.WorkOnTasks.Include(c => c.Tasks).FirstOrDefault(c => c.WorkId == id);
            else if (!withTask && withUser)
                return db.WorkOnTasks.Include(c => c.Users).FirstOrDefault(c => c.WorkId == id);
            else
                return db.WorkOnTasks.FirstOrDefault(c => c.WorkId == id);
        }

        public void UpdateItem(WorkOnTask item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}