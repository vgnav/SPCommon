using System.Collections.Generic;
using SPCommon.Entity;
using SPCommon.Infrastructure.Cache;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    public class CachedListRepository<T> : IListRepository<T> where T : BaseListItem, new()
    {
        private readonly IListRepository<T> _listRepository;
        private readonly ICacheProvider<T> _cacheProvider;

        public CachedListRepository(IListRepository<T> listRepository, ICacheConfiguration configuration)
        {
            _listRepository = listRepository;
            _cacheProvider = new IISCacheProvider<T>(configuration);
        }

        public CachedListRepository(IListRepository<T> listRepository, ICacheProvider<T> cacheProvider, ICacheConfiguration configuration)
        {
            _listRepository = listRepository;
            _cacheProvider = cacheProvider;
            ICacheConfiguration configuration1 = configuration;
        }


        public bool Create(T t)
        {
            throw new System.NotImplementedException();
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
