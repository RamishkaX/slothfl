using SlothFreelance.Models;
using SlothFreelance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SlothFreelance.Unit
{
    public class UnitOfWork : IDisposable
    {
        private SFContext db = new SFContext();
        private UserRepository userRepository;
        private RoleRepository roleRepository;
        private ServiceRepository serviceRepository;
        private TasksRepository tasksRepository;
        private CategoryRepository categoryRepository;
        private TaskRequestRepository taskRequestRepository;
        private WorkOnTaskRepository workOnTaskRepository;
        public UserRepository Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(db);
                return userRepository;
            }
        }

        public RoleRepository Roles
        {
            get
            {
                if (roleRepository == null)
                    roleRepository = new RoleRepository(db);
                return roleRepository;
            }
        }

        public ServiceRepository Services
        {
            get
            {
                if (serviceRepository == null)
                    serviceRepository = new ServiceRepository(db);
                return serviceRepository;
            }
        }

        public TasksRepository Tasks
        {
            get
            {
                if (tasksRepository == null)
                    tasksRepository = new TasksRepository(db);
                return tasksRepository;
            }
        }

        public CategoryRepository Category
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(db);
                return categoryRepository;
            }
        }

        public TaskRequestRepository TaskRequests
        {
            get
            {
                if (taskRequestRepository == null)
                    taskRequestRepository = new TaskRequestRepository(db);
                return taskRequestRepository;
            }
        }

        public WorkOnTaskRepository WorkOnTask
        {
            get
            {
                if (workOnTaskRepository == null)
                    workOnTaskRepository = new WorkOnTaskRepository(db);
                return workOnTaskRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}