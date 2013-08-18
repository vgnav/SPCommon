using System.Collections.Generic;
using System.Web.UI;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Repository;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Factory
{
    /// <summary>
    /// Extend this class in client solutions to provide your own repositories as well as the generic list and generic document library ones
    /// Override the ProvideRepositories method and return a dictionary of your own repositories
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory, IServiceLocatable
    {
        protected readonly SPWeb Web;
        protected readonly string ListName;

        // Required to support ServiceLocator
        public RepositoryFactory(){}

        public RepositoryFactory(SPWeb web, string listName)
        {
            Web = web;
            ListName = listName;
        }

        /// <summary>
        /// Called by consumers of the ServiceLocator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T CreateInstance<T>(params object[] args) where T : class, new()
        {
            return new RepositoryFactory(args[0] as SPWeb, args[1] as string) as T;
        }

        public IListRepository<T> CreateListRepository<T>() where T : 
            BaseItem, new()
        {
            var dictionary = ProvideRepositories<T>();
            if (dictionary.ContainsKey(ListName)) return (IListRepository<T>)dictionary[ListName];
            return new GenericListRepository<T>(Web, ListName);
        }

        public IDocumentRepository<T> CreateDocumentRepository<T>() 
            where T : BaseItem, new()
        {
            var dictionary = ProvideRepositories<T>();
            if (dictionary.ContainsKey(ListName)) return (IDocumentRepository<T>)dictionary[ListName];
            return new GenericDocumentRepository<T>(Web, ListName);
        }

        protected virtual Dictionary<string, IRepository<T>> ProvideRepositories<T>() 
            where T : BaseItem, new()
        {
            return new Dictionary<string, IRepository<T>>();
        }

        
    }

}
