using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Repository;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Factory
{
    public class RepositoryFactory
    {
        private static RepositoryFactory _instance;
        public static RepositoryFactory Instance
        {
            get { return _instance ?? (_instance = new RepositoryFactory()); }
        }

        public TRepository Get<TRepository, T>(SPWeb web, string listName) where TRepository : IRepository<T>
            where T : BaseItem, new()
        {
            var dictionary = ProvideRepositories<T>(web);
            if (dictionary.ContainsKey(listName)) return (TRepository) dictionary[listName];
            
            var repositoryType = typeof (TRepository);
            if (repositoryType == typeof (IListRepository<T>))
            {
                return (TRepository) (new GenericListRepository<T>(web, listName) as IRepository<T>);
            }
            if (repositoryType == typeof(IDocumentRepository<T>))
            {
                return (TRepository)(new GenericDocumentRepository<T>(web, listName) as IRepository<T>);
            }
            throw new Exception("Cannot create a repository with the parameters provided");
        }

        public IListRepository<T> GetListRepository<T>(SPWeb web, string listName) where T : BaseItem, new()
        {
            var dictionary = ProvideRepositories<T>(web);
            if (dictionary.ContainsKey(listName)) return (IListRepository<T>)dictionary[listName];
            return new GenericListRepository<T>(web, listName);
        }

        public IDocumentRepository<T> GetDocumentRepository<T>(SPWeb web, string listName) where T : BaseItem, new()
        {
            var dictionary = ProvideRepositories<T>(web);
            if (dictionary.ContainsKey(listName)) return (IDocumentRepository<T>)dictionary[listName];
            return new GenericDocumentRepository<T>(web, listName);
        }

        public virtual Dictionary<string, IRepository<T>> ProvideRepositories<T>(SPWeb web) where T : BaseItem, new()
        {
            return new Dictionary<string, IRepository<T>>();
        }
    }

}
