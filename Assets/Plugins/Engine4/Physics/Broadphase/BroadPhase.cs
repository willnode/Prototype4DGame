//--------------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------------------

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
    public struct ContactPair
    {
        public int A;
        public int B;
    }

    public interface ITreeCallback
    {
        void TreeCallback(int id);
    }

    public class BroadPhase : ITreeCallback
    {
        public BroadPhase(ContactManager manager)
        {
            Manager = manager;

            PairBuffer = new List<ContactPair>();
            MoveBuffer = new List<int>();

        }

        public void InsertShape(Shape shape, Bounds4 aabb)
        {
            int id = Tree.Insert(aabb, shape);
            shape.broadPhaseIndex = id;
            BufferMove(id);
        }

        public void RemoveShape(Shape shape)
        {
            Tree.Remove(shape.broadPhaseIndex);
        }

        // Generates the contact list. 
        public void UpdatePairs()
        {
            PairBuffer.Clear();

            // Query the tree with all moving boxs
            for (int i = 0; i < MoveBuffer.Count; ++i)
            {
                CurrentIndex = MoveBuffer[i];

                Tree.Query(this, Tree.GetFatAABB(CurrentIndex));
            }

            // Reset the move buffer
            MoveBuffer.Clear();

            for (int i = 0; i < PairBuffer.Count; i++)
            {
                // Add contact to manager
                ContactPair pair = PairBuffer[i];
                Manager.AddContact(Tree.GetShape(pair.A), Tree.GetShape(pair.B));
            }
        }

        public void Update(int id, Bounds4 aabb)
        {
            if (Tree.Update(id, aabb))
                BufferMove(id);
        }

        public bool TestOverlap(int A, int B)
        {
            return Bounds4.IsIntersecting(Tree.GetFatAABB(A), Tree.GetFatAABB(B));
        }

        ContactManager Manager;

        List<ContactPair> PairBuffer;

        List<int> MoveBuffer;

        internal DynamicTree Tree = new DynamicTree();
        int CurrentIndex;

        void BufferMove(int id)
        {
            MoveBuffer.Add(id);
        }

        public void TreeCallback(int index)
        {
            // Cannot collide with self
            if (index == CurrentIndex)
                return;

            int iA = Math.Min(index, CurrentIndex);
            int iB = Math.Max(index, CurrentIndex);

            PairBuffer.Add(new ContactPair() { A = iA, B = iB });
        }
    }
}
