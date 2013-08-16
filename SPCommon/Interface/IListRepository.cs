using System.Collections.Generic;
using SPCommon.CAML;
using SPCommon.Entity;

namespace SPCommon.Interface
{
    public interface IListRepository<T> : IRepository<T> where T : BaseListItem, new()
    {
        IList<T> FindByQuery(object query);
        IList<T> FindByCAML(ICAMLExpression camlExpression);
    }
}
