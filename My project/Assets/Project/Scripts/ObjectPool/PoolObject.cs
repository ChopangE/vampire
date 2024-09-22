using System;
using UnityEngine;

namespace ObjectPooling
{
    public class PoolObject : MonoBehaviour, IPoolable<PoolObject>
    {
        private Action<PoolObject> returnToPool; //* 다시 Push할 때 일어날 액션

        protected virtual void OnDisable()
        {
            ReturnToPool();
        }

        public void Initialize(Action<PoolObject> returnAction)
        {
            //cache reference to return action
            this.returnToPool = returnAction;
        }

        public void ReturnToPool()
        {
            //invoke and return this object to pool
            returnToPool?.Invoke(this);
        }
    }
}