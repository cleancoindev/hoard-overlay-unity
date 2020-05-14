namespace Hoard.MVC
{
    /// <summary>
    ///   Interface implemented by objects that can provide correct hoard service options
    /// </summary>
    public interface IHoardConfigProvider<T> where T : HoardServiceConfig
    {
        T GetHoardServiceOptions();
    }
}