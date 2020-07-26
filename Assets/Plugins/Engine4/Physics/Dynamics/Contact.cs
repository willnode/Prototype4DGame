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
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Engine4.Physics.Internal
{

    public class Contact
    {
        public Shape A, B;
        public Body bodyA, bodyB;

        public ContactEdge edgeA;
        public ContactEdge edgeB;

        public float friction;
        public float restitution;
        public bool sensor;
        internal int hash;

        public Manifold manifold;
        public ContactState state;

        public ContactFlags flags;

        public Contact()
        {
            manifold = new Manifold();
            edgeA = new ContactEdge();
            edgeB = new ContactEdge();
            state = new ContactState();
        }

        public void Setup(Shape A, Shape B)
        {
            this.A = A;
            this.B = B;
            bodyA = A.body;
            bodyB = B.body;
            friction = Common.MixFriction(A, B);
            restitution = Common.MixRestitution(A, B);
            sensor = A.sensor || B.sensor;
            hash = A.hash ^ B.hash;

            manifold.Setup(A, B);
            edgeA.Setup(bodyA, bodyB, this);
            edgeB.Setup(bodyB, bodyA, this);
            state.Setup(this);
        }

        public void SolveCollision()
        {
            manifold.contacts = 0;

            Collide.ComputeCollision(manifold, A, B);

            //{
            //    if (manifold.contacts > 0)
            //        Debug.LogWarning("Contact N: " + manifold.normal + " D:" + manifold.depth[0]);
            //}

            if (manifold.contacts > 0)
                flags |= (flags & ContactFlags.Colliding) > 0 ? ContactFlags.WasColliding : ContactFlags.Colliding;
            else
                flags &= (flags & ContactFlags.Colliding) > 0 ? ~ContactFlags.Colliding : ~ContactFlags.WasColliding;
        }

        static internal Stack<Contact> Stack = new Stack<Contact>();

        public override int GetHashCode()
        {
            // The hash is commutative between A and B
            return hash;
        }
    }


    public class Manifold
    {

        public Shape A;
        public Shape B;

        /// Normal that points from A to B
        public Vector4 normal;
        public Vector4[] position = new Vector4[Common.MULTICONTACT_COUNT];
        public float[] depth = new float[Common.MULTICONTACT_COUNT];
        public int contacts;

        public void Setup(Shape A, Shape B) { this.A = A; this.B = B; }

        public void MakeContact(Vector4 n, Vector4 p, float d)
        {
            normal = Vector4.Normalize(n);
            depth[contacts] = d;
            position[contacts] = p;
            contacts++;
        }

        public void MakeContact(Vector4 p, float d)
        {
            depth[contacts] = d;
            position[contacts] = p;
            contacts++;
        }


        public void MakeContact(Vector4 p0, Vector4 p1)
        {
            depth[contacts] = Vector4.LengthSq(p1 - p0);
            position[contacts] = (p0 + p1) * 0.5f;
            contacts++;
        }

        public void MakeContact(Vector4 n)
        {
            normal = Vector4.Normalize(n);
        }

    }

    public class ContactEdge
    {
        public Body other;
        public Contact contact;

        public void Setup(Body b, Body o, Contact c)
        {
            b.contactList.Add(this);
            other = o;
            contact = c;
        }
    }

    public class ContactState
    {

        public int contacts;

        /// Normal, Tangent, Bitangent, Tritangent
        public Vector4[] vectors = new Vector4[4];
        public readonly ContactStateUnit[] units = new ContactStateUnit[Common.MULTICONTACT_COUNT];

        public float restitution;
        public float friction;

        public readonly ContactStateBody A = new ContactStateBody();
        public readonly ContactStateBody B = new ContactStateBody();

        public void UpdateInput(Contact c)
        {
            Manifold m = c.manifold;

            if ((contacts = m.contacts) > 0)
            {

                A.Update(c.bodyA);
                B.Update(c.bodyB);

                for (int i = 0; i < contacts; i++)
                {
                    (units[i] ?? (units[i] = new ContactStateUnit())).Update(m, c, i);
                }

                vectors[0] = m.normal;

                if (Common.ENABLE_FRICTION)
                    Vector4.ComputeBasis(vectors);
            }
        }

        public void Setup(Contact c)
        {
            friction = c.friction;
            restitution = c.restitution;
        }
    }

    public class ContactStateUnit
    {
        public float depth;
        public float bias; // Restitution + baumgarte
        // Per-axis
        public float[] impulses = new float[4];
        public float[] masses = new float[4];
        // COM to CP
        public Vector4 rA;
        public Vector4 rB;

        public void Update(Manifold m, Contact c, int i)
        {
            rA = m.position[i] - c.bodyA.P;
            rB = m.position[i] - c.bodyB.P;

            depth = m.depth[i];
            bias = 0;
        }
    }

    public class ContactStateBody
    {
        /// Inertia matrix
        public Tensor4 i;
        /// mass
        public float m;
        /// Island index
        public int index;

        public void Update(Body body)
        {
            this.m = body.invMass;
            index = body.islandIndex;
            i = body.invInertiaWorld;
        }
    }

    [Flags]
    public enum ContactFlags
    {
        Colliding = 0x00000001, // Set when contact collides during a step
        WasColliding = 0x00000002, // Set when two objects stop colliding
        Island = 0x00000004, // For internal marking during island forming
    };


}
