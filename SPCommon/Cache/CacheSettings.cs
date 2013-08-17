using SPCommon.Interface;

namespace SPCommon.Cache
{
    public struct CacheSettings : ICacheSettings
    {
        public string Key { get; set; }
        public object Query { get; set; }
        public int SingleItemId { get; set; }
        public object Context { get; set; }
        public string ListName { get; set; }
    }
}
