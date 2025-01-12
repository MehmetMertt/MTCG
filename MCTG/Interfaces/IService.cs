using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    public interface IService<T> where T : class
    {
        UnitOfWork UnitOfWork { get; }
    }
}
