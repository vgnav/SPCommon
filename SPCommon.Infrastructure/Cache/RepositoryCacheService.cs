using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Factory;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Cache
{
    public class RepositoryCacheService<T> where T : BaseListItem, new()
    {
        #region Properties and constructors

        protected ICacheProvider<T> CacheProvider { get; set; }
        protected IRepository<T> Repository { get; set; }

        public RepositoryCacheService(ICacheConfiguration configuration)
        {
            // Going against DI here, but for flexibility's sakes we do some object construction
            CacheProvider = new IISCacheProvider<T>(configuration);
            Repository = ListRepositoryFactory.Instance.GetRepository<T>(configuration.ListName, configuration.Context as SPWeb);
        }

        public RepositoryCacheService(IRepository<T> repository, ICacheConfiguration configuration) : this(configuration)
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
            return CacheProvider.Configuration.Query == null ? Repository.FindAll() : Repository.FindByQuery(CacheProvider.Configuration.Query);
        }

        public virtual T GetSingleItemFromRepository()
        {
            return Repository.Read(CacheProvider.Configuration.SingleItemId);
        }
    }

    public class CacheSettings : ICacheConfiguration
    {
        public string Key { get; set; }
        public object Query { get; set; }
        public int SingleItemId { get; set; }
        public object Context { get; set; }
        public string ListName { get; set; }
    }

    class Client
    {
        void Method()
        {
            var cacheSettings = new CacheSettings
            {
                ListName = "Test",
                Query = null,
                Context = SPContext.Current.Web,
                Key = "something",                
            };
            var cache = new RepositoryCacheService<BaseListItem>(cacheSettings);
            var items = cache.GetItems();
            
        }
    }
    
}
