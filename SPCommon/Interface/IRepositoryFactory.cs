using SPCommon.Entity;

namespace SPCommon.Interface
{
    public interface IRepositoryFactory
    {
        IRepository<T> CreateRepository<T>() where T : BaseItem, new();
        IListRepository<T> CreateListRepository<T>() where T : BaseItem, new();
        IDocumentRepository<T> CreateDocumentRepository<T>() where T : BaseItem, new();
    }
}
