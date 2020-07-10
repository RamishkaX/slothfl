using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlothFreelance.IRepositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetItemById(int? id);
        void AddNewItem(T item);
        void DeleteItem(int? id);
        void UpdateItem(T item);
    }
}
