using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Repository;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Factory
{

    public class ListRepositoryFactory
    {
        private static ListRepositoryFactory _instance;
        public static ListRepositoryFactory Instance
        {
            get { return _instance ?? (_instance = new ListRepositoryFactory()); }
        }

        public IListRepository<T> GetRepository<T>(string listName) where T : BaseListItem, new()
        {
            if(SPContext.Current == null || SPContext.Current.Web == null)
                throw new NullReferenceException("SPContext.Current or SPContext.Current.Web is null");
            return GetRepository<T>(listName, SPContext.Current.Web);
        }

        public IListRepository<T> GetRepository<T>(string listName, SPWeb web) where T : BaseListItem, new()
        {
            // Search for a specific repository with the listname. If not found, then return generic repository
            var availableRepositories = ProvideRepositories<T>(web);
            return availableRepositories.ContainsKey(listName) ? 
                    availableRepositories[listName] : new GenericListRepository<T>(web, listName);
        }

        /// <summary>
        /// Clients should override this and provide a dictionary of ListRepositories such that the ListName maps to the repository type
        /// For example, list type "Tasks" should map to TaskRepository with the TaskItem type
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public virtual Dictionary<string, IListRepository<T>> ProvideRepositories<T>(SPWeb web) where T : BaseListItem, new()
        {
            return new Dictionary<string, IListRepository<T>>();
        }
    }

    public class DocumentRepositoryFactory
    {
        private static DocumentRepositoryFactory _instance;
        public static DocumentRepositoryFactory Instance
        {
            get { return _instance ?? (_instance = new DocumentRepositoryFactory()); }
        }

        public IDocumentRepository<T> GetRepository<T>(string libraryName) where T : BaseDocument, new()
        {
            if (SPContext.Current == null || SPContext.Current.Web == null)
                throw new NullReferenceException("SPContext.Current or SPContext.Current.Web is null");
            return GetRepository<T>(libraryName, SPContext.Current.Web);
        }

        public IDocumentRepository<T> GetRepository<T>(string libraryName, SPWeb web) where T : BaseDocument, new()
        {
            // Search for a specific repository with the library name. If not found, then return generic repository
            var availableRepositories = ProvideRepositories<T>();
            return availableRepositories.ContainsKey(libraryName) ? 
                    availableRepositories[libraryName] : new GenericDocumentRepository<T>(web, libraryName);
        }

        // Overridable method
        public virtual Dictionary<string, IDocumentRepository<T>> ProvideRepositories<T>() where T : BaseDocument, new()
        {
            return new Dictionary<string, IDocumentRepository<T>>();
        }
    }
}
