using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlothFreelance.IRepositories
{
    interface ITaskRequestRepository : IRepository<TaskRequests>
    {
        TaskRequests GetRequestByIdWithJoin(int? id, bool withUser, bool withTask);
    }
}
