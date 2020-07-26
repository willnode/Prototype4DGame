using System.Collections.Generic;
using UnityEngine;

namespace Engine4.Internal
{
    
    /// Clever combination between Dictionary with List. Uses hash as main storage. 
    /// Pros: Superfast Add, Remove, Contains (+ with keys), Iteration. All op is O(1)
    /// Cons: Every object MUST HAVE unique hash code, and it doesn't preserve the order
    /// Implemented exclusively for ContactManager (and integers)
    public class Set<T>
    {
        // Actual object
        List<T> _list = new List<T>();
        // Hash, and index in the list
        Dictionary<int, int> _hash = new Dictionary<int, int>();

        public T this[int index]
        {
            get { return _list[index]; }
        }

        public int Count { get { return _list.Count; } }

        public void Clear () { _list.Clear(); _hash.Clear(); }

        public void Add (T t)
        {
            _hash[t.GetHashCode()] = _list.Count;
            _list.Add(t);
        }

        public bool Contains(T t)
        {
            var id = t.GetHashCode();
            return _hash.ContainsKey(id) && _hash[id] >= 0;
        }

        public bool Contains(int hash)
        {
            int r;
            return _hash.TryGetValue(hash, out r) && r >= 0;
        }

        public void Remove (T t)
        {
            // Fast removal trick for dictionary & list
            int i = _hash[t.GetHashCode()];
            int l = _list.Count - 1;
            T b = _list[l];
            _hash[b.GetHashCode()] = i;
            _hash[t.GetHashCode()] = -1;
            _list[i] = b;
            _list.RemoveAt(l);
        }
    }

    // List Pooling
    internal static class ListPool<T> 
    {
        static Stack<List<T>> stack = new Stack<List<T>>();

        public static List<T> Pop ()
        {
            return stack.Count == 0 ? new List<T>() : stack.Pop();
        }

        public static void Push (List<T> t)
        {
            t.Clear();
            stack.Push(t);
        }
    }

    // List Pooling
    internal static class SetPool<T> where T : new()
    {
        static Stack<Set<T>> stack = new Stack<Set<T>>();

        public static Set<T> Pop()
        {
            return stack.Count == 0 ? new Set<T>() : stack.Pop();
        }

        public static void Push(Set<T> t)
        {
            t.Clear();
            stack.Push(t);
        }
    }

    internal class ArrayPool<T> : List<T[]> 
    {
        int size;

        public ArrayPool(int size)
        {
            this.size = size;
        }

        public T[] Pop()
        {
            if (Count == 0)
                return new T[size];
            else
            {
                var r = this[Count - 1];
                RemoveAt(Count - 1);
                return r;
            }
        }

        public void Push(T[] t)
        {
            Add(t);
        }
    }

    /// <summary>
    /// Stack list to generate resusable mesh
    /// </summary>
    public static class MeshPool
    {

        static Stack<Mesh> _pooledObject = new Stack<Mesh>();
        static int iter = 0;
        /// <summary>
        /// Get the mesh
        /// </summary>
        static public Mesh Get()
        {
            if (_pooledObject.Count == 0)
            {
                var m = new Mesh();
                m.name = "TemporaryMesh " + (iter++).ToString();
                m.hideFlags = HideFlags.DontSave;
                return m;
            }
            else
            {
                return _pooledObject.Pop();
            }

        }
        /// <summary>
        /// Release the mesh
        /// </summary>
        static public void Release(Mesh m)
        {
            m.Clear();
            _pooledObject.Push(m);
        }
    }
}
