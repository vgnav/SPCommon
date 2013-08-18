using System.Collections.Generic;
using SPCommon.Entity;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    public class GenericListWSRepository<T> : IListRepository<T> where T : BaseItem, new()
    {
        public bool Create(T t)
        {

            return true;
        }

        public T Read(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(T t)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(T t)
        {
            throw new System.NotImplementedException();
        }

        public IList<T> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<T> FindByQuery(object query)
        {
            throw new System.NotImplementedException();
        }

        public IList<T> FindByCAML(ICAMLExpression camlExpression)
        {
            throw new System.NotImplementedException();
        }
    }
}
