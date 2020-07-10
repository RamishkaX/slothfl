using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlothFreelance.IRepositories
{
    interface ITasksRepository : IRepository<Tasks>
    {
        Tasks GetTaskByIdWithJoin(int? id, bool withCategory, bool withRequests = false, bool withUser = false,bool withWorks = true);
        IEnumerable<Tasks> GetTasksByFilter(int page, int[] filters, int pageSize);
        TaskRequests GetSelectedRequest(int? id);
        IEnumerable<Tasks> GetTasksCount(int count);
    }
}
