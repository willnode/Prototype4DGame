//--------------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------------------

using Engine4.Internal;
/**
Engine4.Physics.Internal Physics Engine (c) 2017 Wildan Mubarok

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software. If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not
be misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/
using System;
using System.Collections.Generic;

namespace Engine4.Physics.Internal
{
    /// Dynamic Bounds4 Tree
    /// This is simplest implementation of Bounds4 tree. 
    public class DynamicTree
    {
        public class Node
        {
            /// -1 is root
            public int parent = -1;
            /// -1 is empty
            public int left = -1;
            /// -1 is empty
            public int right = -1;

            /// Count of branches this node containing.
            /// 0 is the leaf (tip of the tree)
            /// -1 is deallocated (not part of the tree)
            public int height = -1;

            /// Used only when deallocated.
            /// Return the next free node
            public int next;

            /// Bounds4 of this node
            public Bounds4 aabb;

            /// Shape attached to this node (leaf only)
            public Shape data;

            public override string ToString()
            {
                return string.Format(" [ {0}, {1} ] ({2}) ", left, right, height);
            }
            
            public void Replace(int find, int replace)
            {
                if (left == find) left = replace;
                else right = replace;
            }

            public void Set(int l, int r, int p) { left = l; right = r; parent = p; }
            public void Set(int l, int r) { left = l; right = r; }

            public Node(int next) { this.next = next; }
        }

        public int Root = -1;
        public int FreeNode = 0;

        public Node[] Nodes = new Node[0];

        public DynamicTree() { Resize(64); }

        // Provide tight-Bounds4
        public int Insert(Bounds4 aabb, Shape data)
        {

            var n = AllocateNode();
            Nodes[n].aabb = Fatten(aabb);
            Nodes[n].data = data;
            Nodes[n].height = 0;

            InsertLeaf(n);

            return n;
        }

        public void Remove(int id)
        {

            RemoveLeaf(id);

            DeallocateNode(id);

        }

        public bool Update(int id, Bounds4 aabb)
        {
            if (Nodes[id].aabb.Contains(aabb))
                return false;

            RemoveLeaf(id);

            Nodes[id].aabb = Fatten(aabb);

            InsertLeaf(id);

            return true;

        }

        public Shape GetShape(int id)
        {
            return Nodes[id].data;
        }

        public Bounds4 GetFatAABB(int id)
        {
            return Nodes[id].aabb;
        }
        
        Stack<int> stack = new Stack<int>();

        public void Query(ITreeCallback cb, Bounds4 aabb)
        {

            stack.Clear(Root);

            while (stack.Count > 0)
            {
                int id = stack.Pop();

                Node n = Nodes[id];

                if (Bounds4.IsIntersecting(aabb, n.aabb))
                {
                    if (n.height == 0)
                    {
                        cb.TreeCallback(id);
                    }
                    else
                    {
                        stack.Push(n.left);
                        stack.Push(n.right);
                    }
                }
            }
        }

        public void Query(ref RaycastHit4 ray)
        {

            stack.Clear(Root);

            Vector4 p0 = ray.ray.origin;
            Vector4 p1 = ray.ray.direction;

            while (stack.Count > 0)
            {
                int id;

                if ((id = stack.Pop()) == -1)
                    continue;

                Node n = Nodes[id];
                if (!n.aabb.Raycast(p0, p1)) continue;

                if (n.height == 0)
                {
                    GetShape(id).Raycast(ref ray);
                }
                else
                {
                    stack.Push(n.left);
                    stack.Push(n.right);
                }
            }
        }

        void InsertLeaf(int id)
        {
            if (Root == -1)
            {
                // id must be zero at this time..
                Root = id;
                return;
            }

            // Find the closest leaf

            Vector4 c = Nodes[id].aabb.center;
            Node N = Nodes[Root], R, L, P;
            int n = Root, r, l, p;

            while (N.height > 0)
            {
                R = Nodes[r = N.right];
                L = Nodes[l = N.left];
                if (Bounds4.Compare(c, L.aabb, R.aabb) >= 0) { N = L; n = l; }
                else { N = R; n = r; }
            }

            {
                P = Nodes[p = AllocateNode()];
                // Make P becomes parent of N and ID
                Utility.Swap(ref P.left, ref N.left);
                Utility.Swap(ref P.right, ref N.right);
                Utility.Swap(ref P.parent, ref N.parent);
              
                // Validate grandparent
                if (P.parent == -1) Root = p;
                else Nodes[P.parent].Replace(n, p);
                
                // Validate childs
                P.Set(id, n);
                Nodes[id].parent = N.parent = p;

                SyncHierarchy(p);
            }
        }

        void RemoveLeaf(int id)
        {
            if (id == Root)
            {
                Root = -1;
                return;
            }

            Node N = Nodes[id], P = Nodes[N.parent], S;
            int s = (P.left == id) ? P.right : P.left, p = N.parent;

            S = Nodes[s];
            // Make S become part of grandparent N, detach N & P
            Utility.Swap(ref S.parent, ref P.parent);

            // Validate grandparent
            if (S.parent == -1) Root = s;
            else Nodes[S.parent].Replace(p, s);
            
            DeallocateNode(p);

            SyncHierarchy(S.parent);
        }

        // Correct Bounds4 hierarchy heights and AABBs starting at supplied
        void SyncHierarchy(int index)
        {
            while (index != -1)
            {
                Node n = Nodes[index];

                n.height = Math.Max(Nodes[n.left].height, Nodes[n.right].height) + 1;
                n.aabb = Bounds4.Combine(Nodes[n.left].aabb, Nodes[n.right].aabb);

                index = n.parent;
            }
        }

        int AllocateNode()
        {
            var F = Nodes[FreeNode];
            var f = FreeNode;

            if (F.next == Nodes.Length)
                Resize(Nodes.Length << 1);

            FreeNode = F.next;
            return f;
        }

        void DeallocateNode(int index)
        {
            // Use it as the next free node
            var N = Nodes[index];
            N.Set(-1, -1, -1);
            N.data = null;
            N.next = FreeNode;
            FreeNode = index;
        }

        void Resize(int length)
        {
            // Resize the array
            int I = Nodes.Length, J = length;
            Array.Resize(ref Nodes, J);
            for (int i = I; i < J; i++)
            {
                Nodes[i] = new Node(i + 1);
            }
        }
        
        public static Bounds4 Fatten(Bounds4 aabb)
        {
            Vector4 v = new Vector4(0.5f);

            return new Bounds4(aabb.min - v, aabb.max + v);
        }


    }
}
