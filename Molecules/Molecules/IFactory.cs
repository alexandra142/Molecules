namespace Molecules
{
    public interface IFactory<T>
    {
        T NewInstance();
    }
}
