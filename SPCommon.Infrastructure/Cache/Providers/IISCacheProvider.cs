using System.Collections.Generic;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Cache.Providers
{
    /// <summary>
    /// TODO: Implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IISCacheProvider<T> : ICacheProvider<T> where T : class
    {
        public ICacheSettings Settings { get; private set; }

        public IISCacheProvider(ICacheSettings settings)
        {
            Settings = settings;
        }

        public bool IsInCache()
        {
            throw new System.NotImplementedException();
        }

        public IList<T> GetListFromCache()
        {
            throw new System.NotImplementedException();
        }

        public IList<T> InsertListItems(IList<T> items)
        {
            throw new System.NotImplementedException();
        }

        public void Refresh()
        {
            throw new System.NotImplementedException();
        }

        public T GetSingleItem()
        {
            throw new System.NotImplementedException();
        }

        public T InsertSingleItem(T item)
        {
            throw new System.NotImplementedException();
        }
    }
}
