using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace SlothFreelance.Repositories
{
    public class TaskRequestRepository : ITaskRequestRepository
    {
        private SFContext db;

        public TaskRequestRepository(SFContext context)
        {
            db = context;
        }

        public void AddNewItem(TaskRequests item)
        {
            db.TaskRequests.Add(item);
        }

        public void DeleteItem(int? id)
        {
            TaskRequests taskRequests = db.TaskRequests.Find(id);

            if (taskRequests != null)
            {
                db.TaskRequests.Remove(taskRequests);
            }
        }

        public IEnumerable<TaskRequests> GetAll()
        {
            return db.TaskRequests.ToList();
        }

        public TaskRequests GetItemById(int? id)
        {
            return db.TaskRequests.FirstOrDefault(c => c.RequestId == id);
        }

        public TaskRequests GetRequestByIdWithJoin(int? id, bool withUser, bool withTask)
        {
            if (withTask && withUser)
                return db.TaskRequests.Include(c => c.User).Include(c => c.Task).FirstOrDefault(c => c.RequestId == id);
            else if (withTask && !withUser)
                return db.TaskRequests.Include(c => c.Task).FirstOrDefault(c => c.RequestId == id);
            else if (!withTask && withUser)
                return db.TaskRequests.Include(c => c.User).FirstOrDefault(c => c.RequestId == id);
            else
                return GetItemById(id);
        }

        public void UpdateItem(TaskRequests item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}