using System;
using System.Web;
using Microsoft.SharePoint.Administration;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Cache.Providers
{
    public class HttpCacheLock
    {
        private HttpCacheLock(){}
        private static HttpCacheLock _instance;
        public static HttpCacheLock Instance
        {
            get
            {
                return _instance ?? (_instance = new HttpCacheLock());
            }
        }
    }
    
    public class GlobalCacheClearer
    {
        public static event ClearCache OnCacheClear;
        public delegate void ClearCache(GlobalCacheClearer sender);
        private string ServerName { get { return Environment.MachineName; }}
        private const string ClearCacheFlag = "CLEAR_CACHE";

        public void ClearAllCacheItems()
        {
            // This will execute the all subscribed listerens' methods
            OnCacheClear(this);
            // OK to clear the Cache flag from the local server
            SPFarm.Local.Servers[ServerName].Properties.Remove(ClearCacheFlag);
        }

        public void SetClearCacheFlag()
        {
            // Set flag on all servers
            foreach (var server in SPFarm.Local.Servers)
            {
                if (server.Properties.ContainsKey(ClearCacheFlag))
                    server.Properties[ClearCacheFlag] = DateTime.Now;
                else
                    server.Properties.Add(ClearCacheFlag, DateTime.Now);
            }
        }
    }

    public class HttpCacheProvider : ICacheProvider
    {
        public ICacheSettings Settings { get; private set; }
        private static System.Web.Caching.Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }

        public HttpCacheProvider(ICacheSettings settings)
        {
            Settings = settings;
            GlobalCacheClearer.OnCacheClear += GlobalCacheClearer_OnCacheClear;
        }

        // Method will be called when signal to clear cache is set.
        void GlobalCacheClearer_OnCacheClear(GlobalCacheClearer sender)
        {
            Clear();
        }

        #region Interface methods

        public T GetItemFromCache<T>() where T : class
        {
            lock (HttpCacheLock.Instance)
            {
                return Cache[Settings.Key] as T; // Eill return null if item not found
            }
        }

        public T PutItemIntoCache<T>(T t) where T : class
        {
            if (IsInCache()) return t;
            lock (HttpCacheLock.Instance)
            {
                if (IsInCache()) return t;
                if(Settings.IsSlidingCache)
                    Cache.Insert(Settings.Key, t, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(Settings.TimeToExpire));
                else
                    Cache.Insert(Settings.Key, t, null, DateTime.Now.AddMinutes(Settings.TimeToExpire), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return t;
        }

        public void Clear()
        {
            if (!IsInCache()) return;
            lock (HttpCacheLock.Instance)
            {
                if (!IsInCache()) return;
                Cache.Remove(Settings.Key);
            }
        }

        public bool IsInCache()
        {
            return (Cache[Settings.Key] != null);
        }

        #endregion

    }
}
