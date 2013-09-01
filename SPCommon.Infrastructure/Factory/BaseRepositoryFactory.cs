using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Repository;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Factory
{
    /// <summary>
    /// Key is the key the factory uses to return the correct repository
    /// If a repository corresponding to the key does not exist, it will return the generic list/library repository
    /// </summary>
    public abstract class BaseRepositoryFactory : IRepositoryFactory
    {
        protected string Key { get; private set; }

        protected BaseRepositoryFactory(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new NullReferenceException("Key has to be set to a non-empty and non-null value");
            Key = key;
        }

        public virtual IListRepository<T> CreateListRepository<T>() where T : BaseItem, new()
        {
            VerifyState();
            var dictionary = ProvideRepositories<T>();
            return dictionary.ContainsKey(Key) ? (IListRepository<T>) dictionary[Key] : null; 
        }

        public virtual IDocumentRepository<T> CreateDocumentRepository<T>() where T : BaseItem, new()
        {
            VerifyState();
            var dictionary = ProvideRepositories<T>();
            return dictionary.ContainsKey(Key) ? (IDocumentRepository<T>)dictionary[Key] : null; 
        }   

        #region Overidable methods

        /// <summary>
        /// Override this method to provide client specific repositories to the factory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual Dictionary<string, IRepository<T>> ProvideRepositories<T>() where T : BaseItem, new()
        {
            return new Dictionary<string, IRepository<T>>();
        }

        /// <summary>
        /// Raise NullReferenceException if there is something wrong with the state of the class
        /// Override method to provide specific verification scenario
        /// Example: check if the credentials are set, if the repository is returning WebServiceRepository
        /// </summary>
        protected virtual void VerifyState()
        {
            if (string.IsNullOrEmpty(Key))
                throw new NullReferenceException("Key has to be set to a non-empty and non-null value");
        }

        #endregion
    }
}
