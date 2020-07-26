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
using UnityEngine;

namespace Engine4.Physics.Internal
{
    public struct VelocityState
    {
        public Euler4 w;
        public Vector4 v;

        public VelocityState(Vector4 v, Euler4 w) { this.v = v; this.w = w; }
    };

    public class Island
    {
        public void Solve()
        {
            // Apply gravity, Integrate velocities, create state buffers, calculate world inertia
            for (int i = 0; i < Bodies.Count; ++i)
            {
                Body body = Bodies[i];

                if ((body.flags & BodyFlags.Dynamic) > 0)
                {
                    if ((body.flags & BodyFlags.DirtyMass) > 0) body.CalculateMassData();

                    body.ApplyForce(Common.GRAVITY * body.gravityScale * body.mass);

                    // Calculate world space intertia tensor
                    body.invInertiaWorld = Tensor4.Transform(body.invInertiaModel, body.Tx.rotation);

                    // Integrate velocity
                    body.linearVelocity += (body.force * body.invMass) * Dt;
                    body.angularVelocity += (body.invInertiaWorld * body.torque) * Dt;

                    // Apply damping.
                    body.linearVelocity *= 1 / (1 + Dt * body.linearDamping);
                    body.angularVelocity *= 1 / (1 + Dt * body.angularDamping);

                    Velocities[i] = new VelocityState(body.linearVelocity, body.angularVelocity);
                }
            }

            // Create contact solver, pass in state buffers, create buffers for contacts
            // Initialize velocity constraint for normal + friction and warm start
            ContactSolver.PreSolve(Dt);

            // Solve contacts
            for (int h = 0; h < Common.ITERATIONS; ++h)
                ContactSolver.Solve();

            // Copy back state buffers
            // Integrate positions
            // Release Island tag for statics (so it can be used for other islands)
            for (int i = 0; i < Bodies.Count; ++i)
            {
                Body body = Bodies[i];
                VelocityState v = Velocities[i];

                if ((body.flags & BodyFlags.Static) > 0)
                {
                    body.flags &= ~BodyFlags.Island;
                    continue;
                }

                if (!Common.SIM_RANGE.Contains(body.P))
                {
                    body.Active = false;
                }

                // Integrate position
                body.P += (body.linearVelocity = v.v) * Dt;
                body.Tx.rotation = Matrix4.Euler((body.angularVelocity = v.w) * (Dt * Mathf.Rad2Deg)) * body.Tx.rotation;
            }

            if (Common.ALLOW_SLEEP)
            {
                // Find minimum sleep time of the entire island
                float minSleepTime = float.MaxValue;
                for (int i = 0; i < Bodies.Count; ++i)
                {
                    Body body = Bodies[i];

                    if ((body.flags & BodyFlags.Static) > 0)
                        continue;

                    float sqrLinVel = Vector4.LengthSq(body.linearVelocity);
                    float cbAngVel = Euler4.LengthSq(body.angularVelocity);

                    if (sqrLinVel > Common.SLEEP_LINEAR || cbAngVel > Common.SLEEP_ANGULAR)
                        body.sleepTime = minSleepTime = 0;
                    else
                        minSleepTime = Math.Min(minSleepTime, body.sleepTime += Dt);
                }

                // Put entire island to sleep so long as the minimum found sleep time
                // is below the threshold. If the minimum sleep time reaches below the
                // sleeping threshold, the entire island will be reformed next step
                // and sleep test will be tried again.
                if (minSleepTime > Common.SLEEP_TIME)
                {
                    for (int i = 0; i < Bodies.Count; ++i)
                        Bodies[i].SetToSleep();
                }
            }
        }

        public void Add(Body body)
        {
            body.islandIndex = Bodies.Count;
            Bodies.Add(body);
            Velocities.Add(new VelocityState());
        }

        public void Add(Contact contact)
        {
            Contacts.Add(contact);
        }

        public void Clear()
        {
            Bodies.Clear();
            Velocities.Clear();
            Contacts.Clear();
        }

        public Island()
        {
            Bodies = new List<Body>();
            Velocities = new List<VelocityState>();
            Contacts = new List<Contact>();
            ContactSolver = new ContactSolver(this);
        }

        public List<Body> Bodies;
        public List<VelocityState> Velocities;
        public List<Contact> Contacts;
        public ContactSolver ContactSolver;

        public float Dt;
    }
}
