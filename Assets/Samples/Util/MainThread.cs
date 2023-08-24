using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class MainThread : MonoBehaviour
    {
        static MainThread x;
        private ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();

        MainThread()
        {
            x = this;
        }

        public static void RegisterGameObject()
        {
            var name = "Pico.Platform.Runner";
            GameObject g = GameObject.Find(name);
            if (g == null)
            {
                g = new GameObject(name);
            }

            if (g.GetComponent<MainThread>() == null)
            {
                g.AddComponent<MainThread>();
            }
        }

        public static void Run(Action act)
        {
            if (FindObjectOfType<MainThread>() == null)
            {
                RegisterGameObject();
            }

            x.tasks.Enqueue(act);
        }

        private void Update()
        {
            while (!tasks.IsEmpty)
            {
                if (tasks.TryDequeue(out var act))
                {
                    if (act != null)
                    {
                        act();
                    }
                }
            }
        }
    }
}