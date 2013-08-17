using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Repository;
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
            // Going against DI here, but for flexibility's sakes, we do some object construction here
            CacheProvider = new IISCacheProvider<T>(Configuration);
            Repository = new GenericListRepository<T>(Configuration.Context as SPWeb, Configuration.ListName);
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
