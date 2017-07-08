namespace Molecules
{
    public interface IPool<T>
    {
        /// <summary>
        /// Return free T, if there is no free create a new one
        /// </summary>
        /// <returns></returns>
        T GiveNow();

        /// <summary>
        /// Return free T, if there is no free, wait and then return
        /// </summary>
        /// <returns></returns>
        T GiveWait();

        /// <summary>
        /// Return free T, if there is no free , return null
        /// </summary>
        /// <returns></returns>
        T GiveFree();

        //Returning an instance to the pool
        void ReturnInstance(T instance);
    }
}
