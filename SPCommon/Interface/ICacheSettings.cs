namespace SPCommon.Interface
{
    public interface ICacheSettings
    {
        string Key { get; set; }
        object Query { get; set; }
        int SingleItemId { get; set; }
        object Context { get; set; } // This is usually the SPWeb object
        string ListName { get; set; }
        int TimeToExpire { get; set; }
        bool IsSlidingCache { get; set; }
    }
}
