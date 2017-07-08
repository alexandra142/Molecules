using System;
using System.Collections.Generic;
using System.Linq;

namespace Molecules
{
    public class Pool<T> : IPool<T>
    {
        private static List<T> _freeInstances = new List<T>();
        private static HashSet<T> _usedInstances = new HashSet<T>();
        private readonly IFactory<T> _generator;
        private readonly uint _capacity;
        private int _count = _freeInstances.Count + _usedInstances.Count;

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
            lock (_syncGive)
            {
                if (_freeInstances.Any())
                    return GiveExistingFree();
            }
            return Create(false);
        }

        public T GiveWait()
        {
            lock (_syncGive)
            {
                if (_freeInstances.Any())
                    return GiveExistingFree();
                if (_count < _capacity)
                    return Create(false);
                while (_freeInstances.Any())
                {
                }
                return GiveExistingFree();
            }
        }

        public T GiveFree()
        {
            lock (_syncGive)
            {
                return _freeInstances.Any() ? GiveExistingFree() : default(T);
            }
        }

        public void ReturnInstance(T instance)
        {
            lock (_syncGive)
            {
                if (!_usedInstances.Contains(instance))
                    throw new Exception("Cannot return this instance.");
                _usedInstances.Remove(instance);
                _freeInstances.Add(instance);
            }
        }
        #endregion public method

        #region private method
        private T Create(bool free)
        {
            lock (_syncGive)
            {
                if (_count >= _capacity)
                    throw new Exception("Cannot create a new instance. The Pool si full.");
                T t = _generator.NewInstance();
                //_count++;
                if (free)

                    _freeInstances.Add(t);
                else
                    _usedInstances.Add(t);

                return t;
            }

        }

        private T GiveExistingFree()
        {
            T t = _freeInstances.FirstOrDefault();
            _freeInstances.Remove(t);
            _usedInstances.Add(t);

            return t;
        }
        #endregion private method
    }
}
