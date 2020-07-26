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

namespace Engine4.Physics.Internal
{
    public class ContactManager
    {
        public ContactManager()
        {
            Broadphase = new BroadPhase(this);
            contactList = new Set<Contact>();
            ContactListener = null;
        }

        // Add a new contact contact for a pair of objects
        // unless the contact contact already exists
        public void AddContact(Shape A, Shape B)
        {
            Body bodyA = A.body, bodyB = B.body;

            if (!Body.CanCollide(bodyA, bodyB))
                return;

            // Search for existing matching contact 
            if (contactList.Contains(A.hash ^ B.hash))
                return;

            // Create new contact
            var contact = Contact.Stack.Count == 0 ? new Contact() : Contact.Stack.Pop();
            contact.Setup(A, B);
            contactList.Add(contact);

            bodyA.SetToAwake();
            bodyB.SetToAwake();
        }

        // Remove a specific contact
        public void RemoveContact(Contact contact)
        {
            Body A = contact.bodyA;
            Body B = contact.bodyB;

            // Remove from A
            Utility.RemoveFast(A.contactList, contact.edgeA);

            // Remove from B
            Utility.RemoveFast(B.contactList, contact.edgeB);

            A.SetToAwake();
            B.SetToAwake();

            // Remove contact from the manager
            contactList.Remove(contact);
            Contact.Stack.Push(contact);
        }

        // Remove all contacts from a body
        public void RemoveContactsFromBody(Body body)
        {
            for (int i = body.contactList.Count; i-- > 0;)
            {
                RemoveContact(body.contactList[i].contact);
            }
        }

        public void RemoveFromBroadphase(Body body)
        {
            foreach (var shape in body.shapes)
            {
                Broadphase.RemoveShape(shape);
            }
        }

        // Remove contacts without broadphase overlap
        // Solves contact manifolds
        public void TestCollisions()
        {

            for (int h = contactList.Count; h-- > 0;)
            {
                var contact = contactList[h];
                Shape A = contact.A, B = contact.B;
                Body bodyA = A.body, bodyB = B.body;

                contact.flags &= ~ContactFlags.Island;

                if (!bodyA.IsAwake() && !bodyB.IsAwake())
                {
                    continue;
                }

                if (!Body.CanCollide(bodyA, bodyB))
                {
                    RemoveContact(contact);
                    continue;
                }

                // Check if contact should persist
                if (!Broadphase.TestOverlap(A.broadPhaseIndex, B.broadPhaseIndex))
                {
                    RemoveContact(contact);
                    continue;
                }

                contact.SolveCollision();

                if (ContactListener != null)
                    CheckCollision(contact);
            }
        }

        void CheckCollision(Contact c)
        {
            bool a = (c.flags & ContactFlags.Colliding) > 0;
            bool b = (c.flags & ContactFlags.WasColliding) > 0;

            if (a & !b) { ContactListener.BeginContact(c); c.flags |= ContactFlags.WasColliding; }
            else if (b & !a) { ContactListener.EndContact(c); c.flags &= ~ContactFlags.WasColliding; }
        }

        internal Set<Contact> contactList;
        internal BroadPhase Broadphase;
        internal IContactListener ContactListener;
    }
}