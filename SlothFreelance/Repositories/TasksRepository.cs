using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SlothFreelance.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private SFContext db;

        public TasksRepository(SFContext context)
        {
            db = context;
        }

        public void AddNewItem(Tasks item)
        {
            db.Tasks.Add(item);
        }

        public void DeleteItem(int? id)
        {
            Tasks task = db.Tasks.Find(id);

            if (task != null)
            {
                db.Tasks.Remove(task);
            }
        }

        public IEnumerable<Tasks> GetAll()
        {
            return db.Tasks.Include(c => c.Category).ToList();
        }

        public Tasks GetItemById(int? id)
        {
            return db.Tasks.FirstOrDefault(c => c.TaskId == id);
        }

        public TaskRequests GetSelectedRequest(int? id)
        {
            return db.Tasks.Include(c => c.Requests).FirstOrDefault(c => c.TaskId == id).Requests.FirstOrDefault(c => c.Status == "Выбран");
        }

        public Tasks GetTaskByIdWithJoin(int? id, bool withCategory, bool withRequests = false, bool withUser = false, bool withWorks = true)
        {
            if (withCategory && withRequests && withUser && withWorks)
                return db.Tasks.Include(c => c.Users).Include(c => c.Requests).Include(c => c.Category).Include(c => c.WorkOnTasks).FirstOrDefault(c => c.TaskId == id);
            else if (withCategory && withRequests && !withUser)
                return db.Tasks.Include(c => c.Category).Include(c => c.Requests).FirstOrDefault(c => c.TaskId == id);
            else if (withCategory && !withRequests && withUser)
                return db.Tasks.Include(c => c.Category).Include(c => c.Users).FirstOrDefault(c => c.TaskId == id);
            else if (withCategory && !withRequests && !withUser)
                return db.Tasks.Include(c => c.Category).FirstOrDefault(c => c.TaskId == id);
            else if (!withCategory && withRequests && withUser)
                return db.Tasks.Include(c => c.Requests).Include(c => c.Users).FirstOrDefault(c => c.TaskId == id);
            else if (!withCategory && withRequests && !withUser)
                return db.Tasks.Include(c => c.Requests).FirstOrDefault(c => c.TaskId == id);
            else if (!withCategory && !withRequests && withUser)
                return db.Tasks.Include(c => c.Users).FirstOrDefault(c => c.TaskId == id);
            else
                return GetItemById(id);
        }

        public IEnumerable<Tasks> GetTasksByFilter(int page, int[] filters = null, int pageSize = 5)
        {
            List<Tasks> tasks = new List<Tasks>();

            if (filters != null)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    var filterId = filters[i];
                    var filter = db.Categories.FirstOrDefault(c => c.CategoryId == filterId);

                    if (filter == null)
                    {
                        return null;
                    }
                    List<Tasks> newTasks = db.Tasks.Include(c => c.Category).Where(c => c.CategoryId == filter.CategoryId && c.Status == "Размещен").ToList();
                    for (int j = 0; j < newTasks.Count(); j++)
                    {
                        tasks.Add(newTasks[j]);
                    }
                    
                }

                tasks = tasks.OrderByDescending(c => c.TaskId).Take(page * pageSize).ToList();
            }
            else
            {
                tasks = db.Tasks.Include(c => c.Category).OrderByDescending(c => c.TaskId).Where(c => c.Status == "Размещен").Take(page * pageSize).ToList();
            }

            return tasks;
        }

        public IEnumerable<Tasks> GetTasksCount(int count)
        {
            return db.Tasks.Include(c => c.Category).OrderByDescending(c => c.TaskId).Where(c => c.Status == "Размещен").Take(count).ToList();
        }

        public void UpdateItem(Tasks item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}