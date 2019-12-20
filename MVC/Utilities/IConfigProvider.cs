namespace Hoard.MVC
{
    /// <summary>
    ///   Interface implemented by objects that can provide correct hoard service options
    /// </summary>
    public interface IHoardOptionsProvider
    {
        HoardServiceOptions GetHoardServiceOptions();
    }
}
