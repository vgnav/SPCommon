namespace SPCommon.Interface
{
    public interface ICacheConfiguration
    {
        string Key { get; set; }
        object Query { get; set; }
        int SingleItemId { get; set; }
    }
}
