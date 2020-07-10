using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlothFreelance.IRepositories
{
    interface IWorkOnTaskRepository : IRepository<WorkOnTask>
    {
        WorkOnTask GetWorkByIdWithJoin(int? id, bool withTask, bool withUser);
    }
}
