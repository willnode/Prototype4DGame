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

    // This listener is used to gather information about two shapes colliding. This
    // can be used for game logic and sounds. Physics objects created in these
    // Callbacks will not be reported until the following frame. These Callbacks
    // can be called frequently, so make them efficient.
    public interface IContactListener
    {
        void BeginContact(Contact contact);
        void EndContact(Contact contact);
    }

    // This class represents general queries for points, AABBs and Raycasting.
    // ReportShape is called the moment a valid shape is found. The return
    // value of ReportShape controls whether to continue or stop the query.
    // By returning only true, all shapes that fulfill the query will be re-
    // ported.
    public delegate void QueryCallback(Shape shape);

    public class Scene
    {
        public Scene()
        {
            ContactManager = new ContactManager();
            Island = new Island();
            Bodies = new List<Body>();
            Stack = new Stack<Body>();
            Shapes = new Set<Shape>();
        }

        internal Island Island;
        internal Stack<Body> Stack;
        internal ContactManager ContactManager;
        internal List<Body> Bodies;
        internal Set<Shape> Shapes;

        // Run the simulation forward in time by dt
        public void Step(float Dt)
        {
            // Mitigate the old and look for new contacts
            ContactManager.TestCollisions();

            ContactManager.Broadphase.UpdatePairs();

            Island.Dt = Math.Min(Dt, Common.MAX_DT);

            // Build each active Island and then solve each built Island
            foreach (var seed in Bodies)
            {
                // Skip if this seed has been sweeped out
                // Skip if object static
                if ((seed.flags & (BodyFlags.Island | BodyFlags.Static)) > 0)
                    continue;

                // Seed must be awake & active
                if (!seed.IsAwake())
                    continue;

                // Clear the environemt
                Island.Clear();

                // Sweep related bodies
                CollectRelatedBodies(seed);

                // Nuke it
                Island.Solve();
            }

            // Update the broadphase AABBs
            // Clear all forces
            // Clear island marks
            foreach (var body in Bodies)
            {
                body.SynchronizeProxies();
                body.force = Vector4.zero;
                body.torque = Euler4.zero;
                body.flags &= ~BodyFlags.Island;
            }

        }

        private void CollectRelatedBodies(Body seed)
        {
            Stack.Clear(seed);

            // Mark seed as apart of Island
            seed.flags |= BodyFlags.Island;

            // Perform DFS (Depth First Search) on constraint graph
            while (Stack.Count > 0)
            {
                // Decrement stack to implement iterative backtracking
                Body body = Stack.Pop();
                Island.Add(body);

                // Awaken all bodies connected to the Island
                body.SetToAwake();

                // Do not search across static bodies to keep Island
                // formations as small as possible, however the static
                // body itself should be apart of the Island in order
                // to properly represent a full contact
                if ((body.flags & BodyFlags.Static) > 0)
                    continue;

                // Search all contacts connected to this body
                foreach (var edge in body.contactList)
                {
                    Contact contact = edge.contact;

                    // Skip contacts that have been added to an Island already
                    if ((contact.flags & ContactFlags.Island) > 0)
                        continue;

                    // Can safely skip this contact if it didn't actually collide with anything
                    if ((contact.flags & ContactFlags.Colliding) == 0)
                        continue;

                    // Skip sensors
                    if (contact.sensor)
                        continue;

                    // Mark Island flag and add to Island
                    contact.flags |= ContactFlags.Island;
                    Island.Add(contact);

                    // Attempt to add the other body in the contact to the Island
                    // to simulate contact awakening propogation
                    Body other = edge.other;

                    if ((other.flags & BodyFlags.Island) > 0)
                        continue;

                    Stack.Push(other);

                    other.flags |= BodyFlags.Island;
                }
            }
        }

        public void CreateBody(Body body)
        {
            if (body.scene != null) throw new ArgumentException();

            body.scene = this;

            Bodies.Add(body);
        }

        // Frees a body, removes all shapes associated with the body and frees
        // all shapes and contacts associated and attached to this body.
        public void RemoveBody(Body body)
        {
            ContactManager.RemoveContactsFromBody(body);

            body.RemoveAllShapes();

            // Remove body from scene Bodies
            Bodies.Remove(body);
        }

        // Removes all bodies from the scene.
        public void ClearBodies()
        {
            foreach (var body in Bodies)
                body.RemoveAllShapes();

            Bodies.Clear();
        }

        //// Sets the listener to report collision start/end. Provides the user
        //// with a pointer to an Contact. The Contact
        //// holds pointers to the two shapes involved in a collision, and the
        //// two bodies connected to each shape. The ContactListener will be
        //// called very often, so it is recommended for the funciton to be very
        //// efficient. Provide a NULL pointer to remove the previously set
        //// listener.
        //public void SetContactListener(ContactListener listener)
        //{
        //    ContactManager.ContactListener = listener;
        //}

        //public struct SceneQueryAABBWrapper : ITreeCallback
        //{
        //    public bool TreeCallback(int id)
        //    {
        //        Shape shape = broadPhase.Tree.GetShape(id);

        //        if (Bounds4.IsIntersecting(Aabb, shape.ComputeAABB(shape.body.GetTransform())))
        //        {
        //            return cb.ReportShape(shape);
        //        }

        //        return true;
        //    }

        //    internal QueryCallback cb;
        //    internal BroadPhase broadPhase;
        //    internal Bounds4 Aabb;
        //};

        //public struct SceneQueryPointWrapper : ITreeCallback
        //{
        //    public bool TreeCallback(int id)
        //    {
        //        Shape shape = broadPhase.Tree.GetShape(id);

        //        if (shape.TestPoint(shape.body.GetTransform(), Point))
        //        {
        //            return cb.ReportShape(shape);
        //        }

        //        return true;
        //    }

        //    internal QueryCallback cb;
        //    internal BroadPhase broadPhase;
        //    internal Vector4 Point;
        //};


        /// <summary>
        /// Query the world to find any shapes intersecting a ray.
        /// </summary>
        public RaycastHit4 QueryRaycast(RaycastHit4 rayCast)
        {
            ContactManager.Broadphase.Tree.Query(ref rayCast);
            return rayCast;
        }
    }
}
