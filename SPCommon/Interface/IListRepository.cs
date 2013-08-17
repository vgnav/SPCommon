using System.Collections.Generic;

namespace SPCommon.Interface
{
    public interface IListRepository<T> : IRepository<T> where T : class, new()
    {
        IList<T> FindByCAML(ICAMLExpression camlExpression);
    }
}
