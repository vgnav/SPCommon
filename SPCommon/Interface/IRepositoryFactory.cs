using SPCommon.Entity;

namespace SPCommon.Interface
{
    public interface IRepositoryFactory
    {
        IListRepository<T> CreateListRepository<T>() where T : BaseItem, new();
        IDocumentRepository<T> CreateDocumentRepository<T>() where T : BaseItem, new();
    }
}
