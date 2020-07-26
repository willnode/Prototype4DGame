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
        public Queue<ProjectUnit> units = new Queue<ProjectUnit>();
        public Thread[] workers = null;
        public Projector4[] projectors = null;
        public int[] identifier = null;

        AutoResetEvent blockade = new AutoResetEvent(false);

        void OnEnable()
        {
            Initialize(viewer4.projector);
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
            units.Clear();
        }

        public void AddJob(ProjectUnit unit)
        {
            units.Enqueue(unit);
            blockade.Set();
        }

        public void Initialize<T>(T template) where T : Projector4
        {
            blockade.Reset();
            if (units.Count > 0)
                blockade.Set();

            var count = Environment.ProcessorCount;
            workers = new Thread[count];
            projectors = new Projector4[count];
            identifier = new int[count];

            for (int i = 0; i < count; i++)
            {
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

                    blockade.WaitOne();

                    lock (units)
                    {
                        if (units.Count != 0)
                        {
                            unit = units.Dequeue();
                        }
                    }

                    if (unit.buffer != null)
                    {
                        projectors[index].Project(unit.buffer, unit.matrix, unit.visualizer);
                        unit.renderer.SignalHyperExecution();
                    }
                }

                Debug.Log("Id not match, exiting");
            }
            catch (Exception)
            {
                throw;
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
