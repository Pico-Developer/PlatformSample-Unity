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

        public static void Run(Action act)
        {
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