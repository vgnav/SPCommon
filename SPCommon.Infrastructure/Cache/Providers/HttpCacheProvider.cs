using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint.Administration;
using SPCommon.Infrastructure.Common;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Cache.Providers
{
    public class SyncLock
    {
        private SyncLock(){}
        private static SyncLock _instance;
        public static SyncLock Instance
        {
            get
            {
                return _instance ?? (_instance = new SyncLock());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpCacheProvider<T> : ICacheProvider<T> where T : class
    {
        public ICacheSettings Settings { get; private set; }
        static string ServerName { get { return Environment.MachineName; } }

        public HttpCacheProvider(ICacheSettings settings)
        {
            Settings = settings;
        }

        public bool IsInCache()
        {
            if (ShouldClearCache())
            {
                ClearCache();
                return false;
            }

            return (HttpRuntime.Cache[Settings.Key] != null);
        }

        private void ClearCache()
        {
            var server = SPFarm.Local.Servers[ServerName];
            if (!server.Properties.ContainsKey(Constants.Cache.ClearCacheFlag)) return;

            lock (SyncLock.Instance)
            {
                if (server.Properties.ContainsKey(Constants.Cache.ClearCacheFlag))
                {
                    if (HttpRuntime.Cache[Settings.Key] != null)
                        HttpRuntime.Cache.Remove(Settings.Key);

                    server.Properties.Remove(Constants.Cache.ClearCacheFlag);
                }
                server.Update();
            }
        }

        private static bool ShouldClearCache()
        {
            var server = SPFarm.Local.Servers[ServerName];
            return server.Properties.ContainsKey(Constants.Cache.ClearCacheFlag);
        }

        public IList<T> GetListFromCache()
        {
            throw new System.NotImplementedException();
        }

        public IList<T> InsertListItems(IList<T> items)
        {
            throw new System.NotImplementedException();
        }

        public void Refresh()
        {
            throw new System.NotImplementedException();
        }

        public T GetSingleItem()
        {
            throw new System.NotImplementedException();
        }

        public T InsertSingleItem(T item)
        {
            throw new System.NotImplementedException();
        }
    }
}
