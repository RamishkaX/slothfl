using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SlothFreelance.IRepositories
{
    interface IUserRepository : IRepository<Users>
    {
        Users GetUserByLoginData(string email, string password);
        Users GetUserByIdWithJoin(int? id, bool withRole = true, bool withTasks = false, bool withServices = false);
        Users GetUserByIdWithJoinRequests(int? id);
    }
}
