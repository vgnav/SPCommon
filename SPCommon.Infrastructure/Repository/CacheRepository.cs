using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.WebControls;
using SPCommon.Entity;
using SPCommon.Infrastructure.Cache.Providers;
using SPCommon.Infrastructure.Factory;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    /// <summary>
    /// Cache wrapper around repository.
    /// On 'get' calls, will always attempt first to return items from cache
    /// On 'set' calls, will invalidate then reset the cache
    /// </summary>
    /// <typeparam name="T">Dervied from BaseItem. Entity class</typeparam>
    public class CacheRepository<T> : IRepository<T> where T : BaseItem, new()
    {
        #region Constructors and properties

        private readonly ICacheProvider _cacheProvider;
        private readonly IRepository<T> _repository;

        public CacheRepository(ICacheSettings settings)
        {
            // Create default repository and cache providers
            _repository = (new RepositoryFactory(settings.Context as SPWeb, settings.ListName)).CreateListRepository<T>();
            _cacheProvider = new HttpCacheProvider(settings);
        }
        
        public CacheRepository(IRepositoryFactory factory, ICacheProvider cacheProvider)
        {
            _repository = factory.CreateListRepository<T>();
            _cacheProvider = cacheProvider;
        }

        public CacheRepository(IRepository<T> repository, ICacheProvider cacheProvider)
        {
            _repository = repository;
            _cacheProvider = cacheProvider;
        }

        #endregion

        #region Interface methods

        public bool Create(T t)
        {
            // Create the item and refresh the cache. Cache wil be updated on next fetch
            if(!_repository.Create(t)) throw new Exception("Could not create item");
            _cacheProvider.Clear();
            return true;
        }

        public T Read(int id)
        {
            // Get items from cache. FindByQuery will either get from cache or fetch from repository
            var items = FindByQuery(_cacheProvider.Settings.Query);
            return items.SingleOrDefault(item => item.Id == id);
        }

        public bool Update(T t)
        {
            // Update the item and refresh the cache. Cache will be updated on next fetch
            if (!_repository.Update(t)) throw new Exception("Could not update item");
            // Clear the cache
            _cacheProvider.Clear();
            return true;
        }

        public bool Delete(T t)
        {
            if(!_repository.Delete(t)) throw new Exception("Could not delete item");
            _cacheProvider.Clear();
            return true;
        }

        public IList<T> FindAll()
        {
            return _cacheProvider.GetItemFromCache<IList<T>>() ?? _cacheProvider.PutItemIntoCache(_repository.FindAll());
        }

        public IList<T> FindByQuery(object query)
        {
            return _cacheProvider.GetItemFromCache<IList<T>>() ?? _cacheProvider.PutItemIntoCache(_repository.FindByQuery(_cacheProvider.Settings.Query));
        }

        #endregion
    }
}
