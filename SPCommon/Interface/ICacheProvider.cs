using System.Collections.Generic;

namespace SPCommon.Interface
{
    public interface ICacheProvider
    {
        ICacheSettings Settings { get; }
        bool IsInCache();
        void Clear();
        T GetItemFromCache<T>() where T : class;
        T PutItemIntoCache<T>(T t) where T : class;
    }
}
