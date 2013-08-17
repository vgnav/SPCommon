using System;
using System.Collections.Generic;
using SPCommon.Entity;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    public abstract class BaseRepositoryFactory
    {
        public IRepository<T> GetRepository<T>(string key, string listName) where T : BaseListItem, new()
        {
            return ProvideRepositories<T>(listName)[key];
        }

        public abstract Dictionary<string, IRepository<T>> ProvideRepositories<T>(string listName) where T : BaseListItem, new();
    }

    public class ListFactory : BaseRepositoryFactory
    {
        public override Dictionary<string, IRepository<T>> ProvideRepositories<T>(string listName)
        {
            throw new NotImplementedException();
        }
    }
}
