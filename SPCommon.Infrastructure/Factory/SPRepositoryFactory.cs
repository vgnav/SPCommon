using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.Entity;
using SPCommon.Infrastructure.Repository;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Factory
{
    /// <summary>
    /// Extend this class in client solutions to provide your own SP based repositories
    /// Override the ProvideRepositories method and return a dictionary of your own repositories
    /// 
    /// If there is no repository using the list name provided, then it will either return the generic list or document library repository
    /// </summary>
    public class SPRepositoryFactory : BaseRepositoryFactory
    {
        protected SPWeb Web;

        public SPRepositoryFactory(SPWeb web, string key) : base(key)
        {
            Web = web;
        }

        public override IDocumentRepository<T> CreateDocumentRepository<T>()
        {
            return base.CreateDocumentRepository<T>() ?? GetDefaultDocumentRepository<T>();
        }

        public override IListRepository<T> CreateListRepository<T>()
        {
            return base.CreateListRepository<T>() ?? GetDefaultListRepository<T>();
        }

        /// <summary>
        /// Raise NullReferenceException if there is somethign wrong with the state of the class
        /// Override method to provide specific verification scenario
        /// Example: check if the credentials are set, if the repository is returning WebServiceRepository
        /// </summary>
        protected override void VerifyState()
        {
            base.VerifyState();
            if(Web == null)
                throw new NullReferenceException("The Web object needs to be non-null");
        }

        #region Private methods

        private IListRepository<T> GetDefaultListRepository<T>() where T : BaseItem, new()
        {
            // Need to verify state before returning Generic List Repository
            VerifyState();
            return new GenericListRepository<T>(Web, Key);
        }
        
        private IDocumentRepository<T> GetDefaultDocumentRepository<T>() where T : BaseItem, new()
        {
            // Need to verify state before returning Generic Document Repository
            VerifyState();
            return new GenericDocumentRepository<T>(Web, Key);
        }

        #endregion
    }
}
