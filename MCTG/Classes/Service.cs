using MTCG.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Classes
{
    public class Service<T> : IService<T> where T : class
    {
        protected readonly UnitOfWork _unitOfWork;

        public UnitOfWork UnitOfWork => _unitOfWork;

        public Service(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
