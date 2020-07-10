using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlothFreelance.IRepositories
{
    interface ICategoryRepository : IRepository<Categories>
    {
        Categories GetCategoryByIdWithJoin(int? id, bool withTask = true);
    }
}
