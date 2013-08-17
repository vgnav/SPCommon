using System.Collections.Generic;

namespace SPCommon.Interface
{
    public interface ICacheProvider<T> where T : class
    {
        bool IsInCache();
        IList<T> GetListFromCache();
        IList<T> InsertListItems(IList<T> items);
        void Refresh();
        T GetSingleItem();
        T InsertSingleItem(T item);
        ICacheSettings Settings { get; }
    }
}
