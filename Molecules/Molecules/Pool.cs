using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Molecules
{
    public class Pool<T> : IPool<T>
    {
        public static List<T> FreeInstances = new List<T>();
        public static HashSet<T> UsedInstances = new HashSet<T>();

        private readonly IFactory<T> _generator;
        private readonly uint _capacity;

        //TOdo ako sa zmeni rychlost ked menit zamky?
        private readonly object _syncGiveNow = new object();
        private readonly object _syncGiveWait = new object();
        private readonly object _syncGiveFree = new object();
        private readonly object _syncGive = new object();

        #region constructor
        /// <summary>
        /// Pool without capacity
        /// </summary>
        /// <param name="generator"></param>
        public Pool(IFactory<T> generator) : this(generator, int.MaxValue)
        {
        }

        /// <summary>
        /// Pool with capacity
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="capacity"></param>
        public Pool(IFactory<T> generator, uint capacity) : this(generator, capacity, 0)
        {

        }

        /// <summary>
        /// Pool with capacity
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="capacity"></param>
        /// <param name="start">Number of instances for beginning</param>
        public Pool(IFactory<T> generator, uint capacity, uint start)
        {
            if (capacity < start)
                throw new ArgumentException("Cannot start with more instances then capacity");
            _generator = generator;
            _capacity = capacity;

            for (int i = 0; i < start; i++)
            {
                Create(true);
            }
        }
        #endregion constructor

        #region public method
        public T GiveNow()
        {
            if (FreeInstances.Any())
                return GiveExistingFree();
            return Create(false);
        }

        public T GiveWait()
        {
            if (FreeInstances.Any())
                return GiveExistingFree();
            if (FreeInstances.Count + UsedInstances.Count < _capacity)
                return Create(false);
            while (!FreeInstances.Any() || Process.GetCurrentProcess().Threads.Count < Environment.ProcessorCount)
            {
                if (Process.GetCurrentProcess().Threads.Count >= Environment.ProcessorCount)
                    return default(T);
            }
            return GiveExistingFree();
        }

        public T GiveFree()
        {
            return FreeInstances.Any() ? GiveExistingFree() : default(T);
        }

        public void ReturnInstance(T instance)
        {
            if (!UsedInstances.Contains(instance))
                throw new Exception("Cannot return this instance.");
            UsedInstances.Remove(instance);
            FreeInstances.Add(instance);
            _generator.Reset(instance);
        }
        #endregion public method

        #region private method
        private T Create(bool free)
        {
            if (FreeInstances.Count + UsedInstances.Count >= _capacity)
                throw new Exception("Cannot create a new instance. The Pool si full.");
            T t = _generator.NewInstance();
            if (free)

                FreeInstances.Add(t);
            else
                UsedInstances.Add(t);

            return t;
        }

        private T GiveExistingFree()
        {
            T t = FreeInstances.FirstOrDefault();
            FreeInstances.Remove(t);
            UsedInstances.Add(t);

            return t;
        }
        #endregion private method
    }
}
