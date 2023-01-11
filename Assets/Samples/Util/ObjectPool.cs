using System.Collections.Generic;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class ObjectPool
    {
        private LinkedList<GameObject> pool = new LinkedList<GameObject>();
        private static GameObject globalPool;

        public ObjectPool()
        {
            if (globalPool == null)
            {
                globalPool = new GameObject();
            }
        }

        public GameObject Get(GameObject prefab)
        {
            if (pool.Count == 0)
            {
                return Object.Instantiate(prefab);
            }

            var obj = pool.First.Value;
            pool.RemoveFirst();
            obj.SetActive(true);
            return obj;
        }

        public void Put(GameObject obj)
        {
            obj.SetActive(false);
            pool.AddLast(obj);
            obj.transform.SetParent(globalPool.transform, false);
        }
    }
}