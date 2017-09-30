namespace Molecules
{
    public interface IFactory<T>
    {
        T NewInstance();
        void Reset(T instance);
    }
}
