using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Cache.Providers;
using SPCommon.Infrastructure.Factory;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Cache
{
    public class RepositoryCacheService<T> where T : BaseItem, new()
    {
        #region Properties and constructors

        protected ICacheProvider<T> CacheProvider { get; set; }
        protected IRepository<T> Repository { get; set; }

        public RepositoryCacheService(ICacheSettings settings)
        {
            // Going against DI here, but for flexibility's sakes we create the default cache provider and repository
            CacheProvider = new IISCacheProvider<T>(settings);
            Repository = RepositoryFactory.Instance.GetListRepository<T>(settings.Context as SPWeb, settings.ListName);
        }

        public RepositoryCacheService(IRepository<T> repository, ICacheSettings settings) : this(settings)
        {
            Repository = repository;
        }

        public RepositoryCacheService(IRepository<T> repository, ICacheProvider<T> cacheProvider)
        {
            CacheProvider = cacheProvider;
            Repository = repository;
        }

        #endregion

        public T GetItem()
        {
            return GetItem(false);
        }

        public T GetItem(bool refreshCache)
        {
            if (refreshCache)
                CacheProvider.Refresh();
            return CacheProvider.IsInCache() ? 
                CacheProvider.GetSingleItem() : 
                CacheProvider.InsertSingleItem(GetSingleItemFromRepository());
        }
        
        public IList<T> GetItems()
        {
            return GetItems(false);
        }

        public IList<T> GetItems(bool refreshCache)
        {
            if(refreshCache) CacheProvider.Refresh();
            return CacheProvider.IsInCache() ? 
                CacheProvider.GetListFromCache() : 
                CacheProvider.InsertListItems(GetItemsFromRepository());
        }

        public virtual IList<T> GetItemsFromRepository()
        {
            return CacheProvider.Settings.Query == null ? Repository.FindAll() : Repository.FindByQuery(CacheProvider.Settings.Query);
        }

        public virtual T GetSingleItemFromRepository()
        {
            return Repository.Read(CacheProvider.Settings.SingleItemId);
        }
    }
}
