using System;
using System.Collections.Generic;
using SPCommon.Entity;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Cache
{
    public class RepositoryCacheService<T> where T : BaseListItem, new()
    {
        #region Properties and constructors

        protected ICacheProvider<T> CacheProvider { get; set; }
        protected IRepository<T> Repository { get; set; }
        protected ICacheConfiguration Configuration { get; set; }

        public RepositoryCacheService(ICacheConfiguration configuration)
        {
            Configuration = configuration;
            CacheProvider = new IISCacheProvider<T>(Configuration);
        }

        public RepositoryCacheService(IRepository<T> repository, ICacheConfiguration configuration) : this(configuration)
        {
            Repository = repository;
        }

        public RepositoryCacheService(ICacheProvider<T> cacheProvider, IRepository<T> repository,
            ICacheConfiguration configuration) : this(repository, configuration)
        {
            CacheProvider = cacheProvider;
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
            return Configuration.Query == null ? Repository.FindAll() : Repository.FindByQuery(Configuration.Query);
        }

        public virtual T GetSingleItemFromRepository()
        {
            return Repository.Read(Configuration.SingleItemId);
        }
    }
    
}
