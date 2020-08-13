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
using Engine4.Internal;
using UnityEngine;

namespace Engine4.Physics.Internal
{
    public static class Collide
    {

        //--------------------------------------------------------------------------------------------------
        static void BoxToBox(Manifold m, Box a, Box b)
        {
            Matrix4x5 atx = a.GetWorldTransform();
            Matrix4x5 btx = b.GetWorldTransform();
            Vector4 eA = a.extent, eB = b.extent;

            // B's frame input A's space
            Matrix4 E = btx.rotation / atx.rotation, C = Matrix4.Abs(E);
            Vector4 T = btx.position / atx;

            // Query states
            float s, S = float.MinValue;
            Vector4 N = new Vector4();
            int axis = -1;

            // SAT from A
            for (int i = 0; i < 4; i++)
            {
                if ((s = Math.Abs(T[i]) - (eA[i] + Vector4.Dot(C[i], eB))) > S)
                {
                    if ((S = s) > 0) return;
                    N = atx.rotation.Column(i);
                    axis = i;
                }
            }

            // SAT from B
            for (int i = 0; i < 4; i++)
            {
                if ((s = Math.Abs(Vector4.Dot(T, E.Column(i))) - (eB[i] + Vector4.Dot(C.Column(i), eA))) > S)
                {
                    if ((S = s) > 0) return;
                    N = btx.rotation.Column(i);
                    axis = i + 4;
                }
            }

            if (Vector4.Dot(N, btx.position - atx.position) < 0)
                N *= -1;
            
            // Don't know why have to unflag this?
            if (axis >= 4)
            {
                Utility.Swap(ref atx, ref btx);
                Utility.Swap(ref eA, ref eB);
                N = -N;
            }

            // Compute reference and incident edge information necessary for clipping               

            Algorithm.ComputeIncidentFace(btx, eB, N, Algorithm.incident);

            Algorithm.ComputeReferenceEdgesAndBasis(eA, atx, N, out Matrix4 basis, out Vector4 e);

            Algorithm.Clip(atx.position, e, basis, Algorithm.incident, m);

            m.normal = axis >= 4 ? - N : N;
        }

        //--------------------------------------------------------------------------------------------------
        static void BoxToSphere(Manifold m, Box a, Sphere b) //CHECK
        {
            Matrix4x5 atx = a.GetWorldTransform();
            Matrix4x5 btx = b.GetWorldTransform();

            Bounds4 eAABB = new Bounds4(a.extent);

            float r2 = b.radius * b.radius, d2;

            // Vector4 from center A to center B relative to A's space
            Vector4 t = btx.position / atx;

            Vector4 n = eAABB.Clamp(t), d = t - n;

            if ((d2 = Vector4.LengthSq(d)) > r2) return;

            m.MakeContact(atx.rotation * d, atx * n, r2 - d2);
        }

        //--------------------------------------------------------------------------------------------------
        static void SphereToSphere(Manifold m, Sphere a, Sphere b) //CHECK
        {
            // For simplicity we can ignore rotation and calculate directly in world space.

            Vector4 ap = a.body.Tx * a.local.position;
            Vector4 bp = b.body.Tx * b.local.position;

            Vector4 d = bp - ap;

            float r = a.radius + b.radius, r2 = r * r, d2;

            if ((d2 = Vector4.LengthSq(d)) > r2) return;

            m.MakeContact(d, (ap + bp) * 0.5f, r2 - d2);

        }

        //--------------------------------------------------------------------------------------------------
        static void BoxToCapsule(Manifold m, Box a, Capsule b)
        {
            Matrix4x5 atx = a.GetWorldTransform();
            Matrix4x5 btx = b.GetWorldTransform();
            float r = b.radius, eB = b.extent;
            Vector4 eA = a.extent;

            // B's frame input A's space
            Matrix4 C = btx.rotation / atx.rotation;
            Vector4 T = btx.position / atx;

            // Query states
            float s, S = float.MinValue; int axis = -1;
            Vector4 N = new Vector4(), A, B, oA, oB;

            // SAT from A
            for (int i = 0; i < 4; i++)
            {
                if ((s = Math.Abs(T[i]) - (eA[i] + (Math.Abs(C.Column(i).y) * eB))) > S)
                {
                    if ((S = s) > r) return;
                    N = atx.rotation.Column(axis = i);
                }
            }

            if (Vector4.Dot(N, btx.position - atx.position) < 0)
                N = -N;

            Plane4 H = new Plane4() { normal = N, distance = eA[axis] };

            var p = Algorithm.EdgePlaneContact(out A, out B, H, oA = btx * new Vector4(1, eB), oB = btx * new Vector4(1, -eB));

            if (p)
            {
                m.MakeContact(atx.position + H.Project(oA), atx.position + oA - N * r);
                m.MakeContact(atx.position + H.Project(oB), atx.position + oB - N * r);
                m.normal = N;
            }
            else
            {
                m.MakeContact(N, atx.position + (A + B) * 0.5f, S);
            }
        }

        //--------------------------------------------------------------------------------------------------
        static void SphereToCapsule(Manifold m, Sphere a, Capsule b)
        {
            Matrix4x5 atx = a.GetWorldTransform();
            Matrix4x5 btx = b.GetWorldTransform();

            Vector4 P, Q;
            float rA = a.radius, rB = b.radius, d2;
            float r = rA + rB, eB = b.extent, r2 = r * r;

            Q = Algorithm.EdgeVertexContact(btx * new Vector4(1, eB), btx * new Vector4(1, -eB), P = atx.position);

            if ((d2 = Vector4.LengthSq(Q - P)) > r2) return;

            m.MakeContact(Q - P, (P + Q) * 0.5f, r2 - d2);

        }

        //--------------------------------------------------------------------------------------------------
        static void CapsuleToCapsule(Manifold m, Capsule a, Capsule b) // CHECK
        {
            Matrix4x5 atx = a.GetWorldTransform();
            Matrix4x5 btx = b.GetWorldTransform();

            Vector4 oA = new Vector4(0, a.extent, 0, 0), A;
            Vector4 oB = new Vector4(0, b.extent, 0, 0), B;

            float r = a.radius + b.radius, r2 = r * r, d2;

            var p = Algorithm.EdgesContact(out A, out B, atx * oA, atx * -oA, btx * oB, btx * -oB);

            if ((d2 = Vector4.DistanceSq(A, B)) > r2) return;

            if (p)
            {
                // Two contact because capsules are parallel to each other
                m.MakeContact(atx * oA, btx * oB);
                m.MakeContact(atx * -oA, btx * -oB);
                m.normal = Vector4.Normalize(B - A);
            }
            else
                m.MakeContact(B - A, (A + B) * 0.5f, r2 - d2);
        }

        //--------------------------------------------------------------------------------------------------
        public static void ComputeCollision(Manifold m, Shape a, Shape b)
        {
            bool inv;

            if (inv = a.type > b.type)
                Utility.Swap(ref a, ref b);

            if (a.type == ShapeType.Box)
            {
                if (b.type == ShapeType.Box) BoxToBox(m, a as Box, b as Box);
                else if (b.type == ShapeType.Sphere) BoxToSphere(m, a as Box, b as Sphere);
                else if (b.type == ShapeType.Capsule) BoxToCapsule(m, a as Box, b as Capsule);
            }
            else if (a.type == ShapeType.Sphere)
            {
                if (b.type == ShapeType.Sphere) SphereToSphere(m, a as Sphere, b as Sphere);
                else if (b.type == ShapeType.Capsule) SphereToCapsule(m, a as Sphere, b as Capsule);
            }
            else if (a.type == ShapeType.Capsule)
            {
                if (b.type == ShapeType.Capsule) CapsuleToCapsule(m, a as Capsule, b as Capsule);
            }

            if (inv) m.normal *= -1;
        }

    }
}
