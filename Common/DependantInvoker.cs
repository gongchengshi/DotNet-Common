using System;
using System.Collections.Generic;

namespace Gongchengshi
{
    // In asynchronous programming, there is a need to only invoke a method once after a number of asynchronous 
    // calls have called back.  Add is called when the call is made and Fullfill is called in the 
    // callback.  Once the adds and fullfills correspond with each other the specified method is executed.

    /// <summary>
    /// Uses ids identify callbacks with the asynchronous call.
    /// </summary>
    public class IDDependantInvoker
    {
        public IDDependantInvoker(Action methodToExecute)
        {
            _methodToExecute = methodToExecute;
        }

        private int _dependencyCount = 0;
        private ISet<int> _ids = new HashSet<int>();
        private Action _methodToExecute;

        public int Add()
        {
            int id;
            unchecked
            {
                // It's not reasonable to think that there would be more than int.max outstanding calls.
                id = _dependencyCount++;
            }
            _ids.Add(id);
            return id;
        }

        public void Fullfill(int id)
        {
            _ids.Remove(id);
            if (_ids.Count == 0)
            {
                _methodToExecute();
                _dependencyCount = 0;
            }
        }
    }

    /// <summary>
    /// Less strict than IDDependantInvoker. The number of calls to Add must match the number of calls to Fullfilled
    /// before the method is executed.
    /// 
    /// </summary>
    public class CountDependantInvoker
    {
        public CountDependantInvoker(Action methodToExecute)
        {
            _methodToExecute = methodToExecute;
        }

        private int _dependencyCount = 0;
        private Action _methodToExecute;

        public void Add()
        {
            ++_dependencyCount;
        }

        public void Fullfill()
        {
            if (--_dependencyCount == 0)
            {
                _methodToExecute();
            }
        }
    }

    /// <summary>
    /// The specified number of unique objects must be provided to Fullfilled before the method is executed.
    /// </summary>
    public class UniqueObjectDependantInvoker
    {
        public UniqueObjectDependantInvoker(Action methodToExecute, int numDependancies)
        {
            _methodToExecute = methodToExecute;
            _dependancies = new object[numDependancies];
        }

        private Action _methodToExecute;
        private object[] _dependancies;

        public void Fullfill(object dependancy)
        {
            for (int i = 0; i < _dependancies.Length; ++i)
            {
                if (_dependancies[i] == null)
                {
                    _dependancies[i] = dependancy;
                    break;
                }

                if (Array.ReferenceEquals(_dependancies[i], dependancy))
                {
                    break;
                }
            }

            if(_dependancies[_dependancies.Length - 1] != null)
            {
                _methodToExecute();
            }
        }
    }
}
