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

    // Contact solver
    public class ContactSolver
    {
        internal ContactSolver(Island island)
        {
            Contacts = island.Contacts;
            Velocities = island.Velocities;
        }

        public void PreSolve(float dt)
        {
            for (int i = 0; i < Contacts.Count; ++i)
            {
                Contact c = Contacts[i];
                ContactState cs = c.state;
                ContactStateUnit u;

                cs.UpdateInput(c);

                if (cs.contacts == 0) continue;

                float nm = cs.A.m + cs.B.m, dv;

                Vector4 vA = Velocities[cs.A.index].v, vB = Velocities[cs.B.index].v;
                Euler4 wA = Velocities[cs.A.index].w, wB = Velocities[cs.B.index].w;

                for (int j = 0; j < cs.contacts; j++)
                {

                    u = cs.units[j];

                    // Precalculate bias factor

                    u.bias = -Common.BAUMGARTE * (1 / dt) * Math.Min(0, u.depth + Common.PENETRATION_SLOP);

                    if ((dv = Vector4.Dot(vB + Euler4.Cross(wB, u.rB) - vA - Euler4.Cross(wA, u.rA), cs.vectors[0])) < -1)
                        u.bias -= cs.restitution * dv;

                    for (int k = Common.ENABLE_FRICTION ? 4 : 1; k-- > 0;)
                    {
                        // Precalculate vector masses
                        Euler4 raCt = Euler4.Cross(u.rA, cs.vectors[k]);
                        Euler4 rbCt = Euler4.Cross(u.rB, cs.vectors[k]);

                        var tm = nm + Euler4.Dot(raCt, cs.A.i * raCt) + Euler4.Dot(rbCt, cs.B.i * rbCt);
                        u.masses[k] = Utility.Invert(tm);
                        u.impulses[k] = 0;
                    }
                }
            }
        }

        public void Solve()
        {
            for (int i = 0; i < Contacts.Count; ++i)
            {
                ContactState cs = Contacts[i].state;
               
                if (cs.contacts == 0) continue;

                Vector4 vA = Velocities[cs.A.index].v, vB = Velocities[cs.B.index].v;
                Euler4 wA = Velocities[cs.A.index].w, wB = Velocities[cs.B.index].w;

                for (int j = 0; j < cs.contacts; j++)
                {
                    ContactStateUnit u = cs.units[j];

                    // relative velocity at contact
                    Vector4 dv = vB + Euler4.Cross(wB, u.rB) - vA - Euler4.Cross(wA, u.rA);

                    // Friction
                    for (int k = Common.ENABLE_FRICTION ? 4 : 1; k-- > 0;)
                    {
                        float lambda, oldP = u.impulses[k];

                        // Clamp frictional impulse
                        if (k == 0)
                        {
                            lambda = (-Vector4.Dot(dv, cs.vectors[k]) + u.bias) * u.masses[k];
                            lambda = Math.Max(0, oldP + lambda);
                        }
                        else
                        {
                            lambda = -Vector4.Dot(dv, cs.vectors[k]) * u.masses[k];
                            var c = cs.friction * u.impulses[0];
                            lambda = Utility.Clamp(-c, c, oldP + lambda);
                        }
                        
                        // Apply friction impulse
                        Vector4 impulse = cs.vectors[k] * ((u.impulses[k] = lambda) - oldP);

                        vA -= impulse * cs.A.m;
                        wA -= cs.A.i * Euler4.Cross(u.rA, impulse);

                        vB += impulse * cs.B.m;
                        wB += cs.B.i * Euler4.Cross(u.rB, impulse);
                    }
                }

                Velocities[cs.A.index] = new VelocityState(vA, wA);
                Velocities[cs.B.index] = new VelocityState(vB, wB);
            }
        }

        public List<Contact> Contacts;
        public List<VelocityState> Velocities;
    };

}
