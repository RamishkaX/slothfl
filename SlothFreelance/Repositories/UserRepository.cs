using SlothFreelance.IRepositories;
using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SlothFreelance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private SFContext db;
        
        public UserRepository(SFContext context)
        {
            db = context;
        }

        public void AddNewItem(Users item)
        {
            db.Users.Add(item);
        }

        public void DeleteItem(int? id)
        {
            Users user = db.Users.Find(id);
            
            if (user != null)
            {
                db.Users.Remove(user);
            }
        }

        public IEnumerable<Users> GetAll()
        {
            return db.Users.ToList();
        }

        
        public Users GetItemById(int? id)
        {
            return db.Users.FirstOrDefault(c => c.UserId == id);
        }

        public Users GetUserByIdWithJoin(int? id, bool withRole, bool withTasks = false, bool withServices = false)
        {
            if (withRole && withTasks && withServices)
                return db.Users.Include(c => c.Role).Include(c => c.Tasks).Include(c => c.Services).FirstOrDefault(c => c.UserId == id);
            else if (withRole && withTasks && !withServices)
                return db.Users.Include(c => c.Role).Include(c => c.Tasks).FirstOrDefault(c => c.UserId == id);
            else if (withRole && !withTasks && withServices)
                return db.Users.Include(c => c.Role).Include(c => c.Services).FirstOrDefault(c => c.UserId == id);
            else if (withRole && !withTasks && !withServices)
                return db.Users.Include(c => c.Role).FirstOrDefault(c => c.UserId == id);
            else if (!withRole && withTasks && withServices)
                return db.Users.Include(c => c.Tasks).Include(c => c.Services).FirstOrDefault(c => c.UserId == id);
            else if (!withRole && withTasks && !withServices)
                return db.Users.Include(c => c.Tasks).FirstOrDefault(c => c.UserId == id);
            else if (!withRole && !withTasks && withServices)
                return db.Users.Include(c => c.Services).FirstOrDefault(c => c.UserId == id);
            else
                return GetItemById(id);
        }

        public Users GetUserByIdWithJoinRequests(int? id)
        {
            return db.Users.Include(c => c.TaskRequests).FirstOrDefault(c => c.UserId == id);
        }

        public Users GetUserByLoginData(string email, string password = null)
        {
            if (password == null)
            {
                return db.Users.FirstOrDefault(u => u.Email == email);
            }
            return db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public void UpdateItem(Users item)
        {
            db.Entry(item).State = EntityState.Modified;
        }
    }
}