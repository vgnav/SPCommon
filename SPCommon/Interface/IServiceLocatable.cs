namespace SPCommon.Interface
{
    /// <summary>
    /// Classes implementing this interface must provide a method which returns a new instance of the same class 
    /// initialised with the given parameters. This ensures DI can still be used.
    /// Caveat: constructor parameters need to be known by the client otherwise exceptions will be thrown
    /// </summary>
    public interface IServiceLocatable
    {
        T CreateInstance<T>(params object[] args) where T : class, new();
    }
}
