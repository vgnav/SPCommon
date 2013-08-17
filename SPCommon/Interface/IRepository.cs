using System.Collections.Generic;

namespace SPCommon.Interface
{
    public interface IRepository<T> where T : class, new()
    {
        bool Create(T t);
        T Read(int id);
        bool Update(T t);
        bool Delete(T t);
        IList<T> FindAll();
        IList<T> FindByQuery(object query);
    }
}
