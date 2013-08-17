namespace SPCommon.Interface
{
    public interface IDocumentRepository<T> : IListRepository<T> where T : class, new()
    {
        void DownloadFileData(T t);
    }
}
