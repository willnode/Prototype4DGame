using Engine4.Rendering;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Analytics;

namespace Engine4.Internal
{
    [ExecuteInEditMode]
    public class ProjectorJob : MonoBehaviour4
    {
        public Queue<ProjectUnit>[] units = null;
        public Thread[] workers = null;
        public Projector4[] projectors = null;
        public int[] identifier = null;
        private int currentIndex = 0;
        private int totalThread = 0;
        AutoResetEvent[] blockade = null;

        void OnEnable()
        {
            Initialize();
        }

        void OnDisable()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var t in workers)
            {
                t.Abort();
            }
            workers = null;
            projectors = null;
            units = null;
        }

        public void AddJob(ProjectUnit unit)
        {
            lock (units[currentIndex])
            {
                units[currentIndex].Enqueue(unit);
            }
            blockade[currentIndex].Set();
            currentIndex = (currentIndex + 1) % totalThread;
        }

        public void Initialize()
        {
            currentIndex = 0;
            totalThread = Environment.ProcessorCount;
            units = new Queue<ProjectUnit>[totalThread];
            blockade = new AutoResetEvent[totalThread];
            workers = new Thread[totalThread];
            projectors = new Projector4[totalThread];
            identifier = new int[totalThread];

            for (int i = 0; i < totalThread; i++)
            {
                units[i] = new Queue<ProjectUnit>();
                blockade[i] = new AutoResetEvent(false);
                StartWork(i);
            }
        }

        private void StartWork(int index)
        {
            var t = new Thread(new ThreadStart(() => Worker(index, identifier[index] = Utility.Random())));
            t.IsBackground = true;
            workers[index] = t;
            t.Start();
        }

        public void SyncProjector<T>(T template) where T : Projector4
        {
            if (workers == null) return;

            var components = GetComponents<T>();

            var iter = components.Length;

            for (int i = 0; i < projectors.Length; i++)
            {
                if (iter == 0)
                    projectors[i] = gameObject.AddComponent<T>();
                else
                    projectors[i] = components[--iter];

                Runtime.CopyComponent(template, projectors[i]);
                projectors[i].Setup(viewer4.viewerToWorldMatrix);
            }
        }

        void Worker(int index, int id)
        {
            try
            {
                while (identifier[index] == id)
                {

                    // Prevent race between threads
                    ProjectUnit unit = new ProjectUnit();

                    blockade[index].WaitOne();

                    lock (units[index])
                    {
                        if (units[index].Count != 0)
                        {
                            unit = units[index].Dequeue();
                        }
                    }

                    if (unit.buffer != null)
                    {
                        projectors[index].Project(unit.buffer, unit.matrix, unit.visualizer);
                        unit.renderer.SignalHyperExecution();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                identifier[index] = -1; // Debugging aid
            }
        }

    }

    public struct ProjectUnit
    {
        public Buffer4 buffer; public Matrix4x5 matrix; public IVisualizer visualizer; public Renderer4 renderer;

        public ProjectUnit(Buffer4 b, Matrix4x5 m, IVisualizer v, Renderer4 r)
        {
            buffer = b; matrix = m; visualizer = v; renderer = r;
        }
    }
}
