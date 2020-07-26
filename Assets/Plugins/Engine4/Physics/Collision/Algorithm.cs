
using Engine4.Internal;
using System;

namespace Engine4.Physics.Internal
{
    // Common geometry algorithms
    internal static class Algorithm
    {

        //--------------------------------------------------------------------------------------------------
        // Get nearest point from two edges, return true if parallel
        internal static bool EdgesContact(out Vector4 CA, out Vector4 CB, Vector4 PA, Vector4 QA, Vector4 PB, Vector4 QB)
        {
            Vector4 DA = QA - PA;
            Vector4 DB = QB - PB;
            Vector4 r = PA - PB;
            float a = Vector4.Dot(DA, DA);
            float e = Vector4.Dot(DB, DB);
            float f = Vector4.Dot(DB, r);
            float c = Vector4.Dot(DA, r);

            float b = Vector4.Dot(DA, DB);
            float denom = a * e - b * b;

            float TA, TB;
            if (denom < 1e-4)
            {
                // Parallel, get another idea to do it.
                var M = (PA + PB + QA + QB) / 4;
                TA = Utility.Clamp01(Vector4.Dot(M - PA, DA) / (a + 1e-5f));
                TB = Utility.Clamp01(Vector4.Dot(M - PB, DB) / (e + 1e-5f));
            }
            else
            {
                TA = Utility.Clamp01((b * f - c * e) / denom);
                TB = Utility.Clamp01((b * TA + f) / e);
            }

            CA = PA + DA * TA;
            CB = PB + DB * TB;

            return denom < 1e-4;
        }

        internal static bool EdgePlaneContact(out Vector4 CA, out Vector4 CB, Plane4 HA, Vector4 PB, Vector4 QB)
        {
            Vector4 u = QB - PB;
            Vector4 w = PB - HA.origin;
            float D = Vector4.Dot(HA.normal, u);
            float N = -Vector4.Dot(HA.normal, w);

            if (D * D < 1e-4)
            {
                // Parallel, get another idea to do it.
                CB = CA = HA.origin;
                return true;
            }
            else
            {
                CB = PB + u * Utility.Clamp01(N / D);
                CA = HA.Project(CB);
                return false;
            }
        }

        internal static Vector4 EdgeVertexContact(Vector4 A, Vector4 B, Vector4 P)
        {
            var AB = B - A;
            var D = Vector4.Dot(P - A, AB) / Vector4.Dot(AB, AB);
            return A + AB * Utility.Clamp01(D);
        }

        /* Convex collision algorithm - soon
       //--------------------------------------------------------------------------------------------------
       // Get nearest point along the edge
       

       internal static Vector4 EdgeVertexContact(Vector4 A, Vector4 B, Vector4 P, out int I)
       {
           var AB = B - A;
           var D = Vector4.Dot(P - A, AB) / Vector4.Dot(AB, AB);

           I = (D < 0 || D > 1) ? 1 : 2;
           return A + Clamp01(D) * (AB);
       }



       //--------------------------------------------------------------------------------------------------
       // Get nearest point along the triangle

       internal static Vector4 TriangleVertexContact(Vector4 A, Vector4 B, Vector4 C, Vector4 P)
       {
           int t;
           return TriangleVertexContact(A, B, C, P, out t);
       }

       internal static Vector4 TriangleVertexContact(Vector4 A, Vector4 B, Vector4 C, Vector4 P, out int I)
       {
           var plane = new Plane4(A, B, C);

           var Q = plane.Project(P);

           Vector4 AB = B - A, AC = A - C, QA = A - Q, QB = B - Q, QC = C - Q;

           var ABC = Vector4.Cross(AB, AC);

           var u = Vector4.Dot(Vector4.Cross(QC, QB), ABC);
           var v = Vector4.Dot(Vector4.Cross(QA, QC), ABC);
           var w = Vector4.Dot(Vector4.Cross(QB, QA), ABC);

           // var d = Vector4.Dot(ABC, ABC);
           // var uvw = u + v + w;
           // Assert(Math.Abs(uvw / d - 1) < 1e-4);

           if (u >= 0 && v >= 0 && w >= 0)
           {
               // Inside the triangle
               I = 4;
               return Q;
           }
           else if (u < 0 ^ v < 0 ^ w < 0)
           {
               // Edge area
               I = 2;
               if (u < 0) return EdgeVertexContact(B, C, Q);
               if (v < 0) return EdgeVertexContact(C, A, Q);
               if (w < 0) return EdgeVertexContact(A, B, Q);
           }
           else
           {
               // Corner area
               I = 1;
               if (u >= 0) return A;
               if (v >= 0) return B;
               if (w >= 0) return C;
           }
           return Q;
       }

       internal static int VertexIndex(Vector4[] Vs, Vector4 V)
       {
           for (int i = 0; i < Vs.Length; i++)
           {
               if (Vector4.DistanceSq(Vs[i], V) < 1e-4)
                   return i;
           }
           Assert(false);
           return -1;
       }

       //--------------------------------------------------------------------------------------------------
       // Get nearest point inside convex shape
       internal static Vector4 ConvexVertexContact(Vector4[] Vs, Vector4 Q)
       {
           var Vb = SetPool<int>.Pop(); // Blacklist data, use Set for speedup
           var V = ListPool<VertexHull>.Pop(); // Convex vertices data
           var v = ConvexVertexContact(Vs, Q, Vb, V);
           SetPool<int>.Push(Vb);
           ListPool<VertexHull>.Push(V);
           return v;
       }

       static Vector4 ConvexVertexContact(Vector4[] Vs, Vector4 Q, Set<int> Vb, List<VertexHull> V)
       {
           Assert(Vs.Length > 2);

           // Setup the instrument
           Vector4 Vc = new Vector4(), Vr = new Vector4();
           Plane4 H;
           int Ti;

           // Get median (don't have to be median, but must be a point inside convex)
           for (int i = 0; i < Vs.Length; i++)
           {
               Vc += Vs[i];
           }

           Vc /= Vs.Length;
           H = new Plane4(Vc - Q, Q);

           // Arcquire list of vertex data
           for (int i = 0; i < Vs.Length; i++)
           {
               V.Add(new VertexHull(Vr = Vs[i], i, H.Distance(Vs[i])));

           }

           // From this point, no arithmatic calculation necessary,
           // Just some magic algorithms. Lets' get dizzy!

           // -- Points on faces

           do
           {

               int A = -1, B = -1, C = -1;
               float dA = float.MaxValue, dB = float.MaxValue, dC = float.MaxValue;

               // Get three nearest vertices
               for (int i = 0; i < Vs.Length; i++)
               {
                   if (Vb.Contains(i)) continue;

                   var dV = V[i].distance;

                   if (dV < dA) { Chain(i, ref A, ref B, ref C); Chain(dV, ref dA, ref dB, ref dC); }
                   else if (dV < dB) { Chain(i, ref B, ref C); Chain(dV, ref dB, ref dC); }
                   else if (dV < dC) { C = i; dC = dV; }
               }

               // Get the magic
               Vr = TriangleVertexContact(V[A].vector, V[B].vector, V[C].vector, Q, out Ti);

               // R must be parallel to triangle in order to be 'valid'
               if (Ti == 4)
                   return Vr;
               else
               {
                   // One of its vertex must be invalid. (get the furthest)
                   if (dA > dB && dA > dC)
                       Vb.Add(V[A].index);
                   else if (dB > dC)
                       Vb.Add(V[B].index);
                   else
                       Vb.Add(V[C].index);
               }

           } while (Vb.Count + 2 < Vs.Length);

           // -- Points on edges

           Vb.Clear();

           do
           {

               int A = -1, B = -1;
               float dA = float.MaxValue, dB = float.MaxValue;

               // Get two nearest vertices
               for (int i = 0; i < Vs.Length; i++)
               {
                   if (Vb.Contains(i)) continue;

                   var dV = V[i].distance;

                   if (dV < dA) { Chain(i, ref A, ref B); Chain(dV, ref dA, ref dB); }
                   else if (dV < dB) { B = i; dB = dV; }
               }

               // Get the magic
               Vr = EdgeVertexContact(V[A].vector, V[B].vector, Q, out Ti);

               // R must be parallel to edge in order to be 'valid'
               if (Ti == 2)
                   return Vr;
               else
               {
                   // One of its vertex must be invalid. (get the furthest)
                   if (dA > dB)
                       Vb.Add(V[A].index);
                   else
                       Vb.Add(V[B].index);
               }

           } while (Vb.Count + 1 < Vs.Length);

           // -- Point on corner. Just find the nearest point!

           {

               int A = -1;
               float dA = float.MaxValue;

               // Get two nearest vertices
               for (int i = 0; i < Vs.Length; i++)
               {
                   var dV = V[i].distance;

                   if (dV < dA) { A = i; dA = dV; }
               }

               // Get the magic
               return V[A].vector;
           }
       }

       //--------------------------------------------------------------------------------------------------
       // Get nearest point between edge and convex shape
       internal static void ConvexEdgeContact(out Vector4 CV, out Vector4 CP, Vector4[] Vs, Vector4 PA, Vector4 PB)
       {
           Vector4 A = ConvexVertexContact(Vs, PA);
           Vector4 B = ConvexVertexContact(Vs, PB);
           EdgesContact(out CV, out CP, A, B, PA, PB);
       }

       internal static bool IsItValidMatches(Vector4 Q, Vector4 Vc, Vector4 A, Vector4 B, Vector4 C)
       {
           var h = new Plane4(A, B, C);
           return h.IsSameSide(Q, Vc);
       }

       internal static bool IsItValidMatches(Vector4 Q, Vector4 Vc, Vector4 A, Vector4 B)
       {
           var h = new Plane4(A, B, Vector4.Cross(A - B, Vc - B));
           return h.IsSameSide(Q, Vc);
       }

       //internal static Vector4[] MinkowskiSubtract

       // Shared array instance, hence no parallel support
       static Vector4[] cubeAABB = new Vector4[8];

       internal static Vector4[] AABBToConvex(Vector4 extent)
       {
           for (int i = 0; i < 8; i++)
               cubeAABB[i] = Bounds4.AABBVertices[i] * extent;

           return cubeAABB;
       }

       internal static Vector4[] AABBToConvex(Bounds4 aabb)
       {
           Vector4 c = aabb.Center, e = aabb.Extent;
           for (int i = 0; i < 8; i++)
               cubeAABB[i] = Bounds4.AABBVertices[i] * e + c;

           return cubeAABB;
       }

       internal static Vector4[] OBBToConvex(OBB obb)
       {
           return OBBToConvex(obb, true);
       }

       internal static Vector4[] OBBToConvex(OBB obb, bool calculatePosition)
       {
           var tx = obb.Tx;
           if (!calculatePosition)
               tx.position = Vector4.zero;
           for (int i = 0; i < 8; i++)
               cubeAABB[i] = tx * (Bounds4.AABBVertices[i] * obb.E);

           return cubeAABB.Clone() as Vector4[];
       }

       internal struct VertexHull
       {
           public Vector4 vector;
           public int index;
           public float distance;

           public VertexHull(Vector4 v, int i, float d)
           {
               vector = v;
               index = i;
               distance = d;
           }
       }

       */

        //--------------------------------------------------------------------------------------------------
        // Return (W-axis) oriented box extent & rotation necessary for clipping
        public static void ComputeReferenceEdgesAndBasis(Vector4 eR, Matrix4x5 rtx, Vector4 n, int axis, out Matrix4 basis, out Vector4 e)
        {
            n = n / rtx.rotation;

            axis = axis % 4;

            //{
            //    e = new Vector4();
            //    e[0] = eR[(axis + 1) % 4];
            //    e[1] = eR[(axis + 2) % 4];
            //    e[2] = eR[(axis + 3) % 4];
            //    e[3] = eR[axis];
            //}
            //{
            //    basis = new Matrix4();
            //    basis.Column0 = rtx.rotation.Column((axis + 1) % 4);
            //    basis.Column1 = rtx.rotation.Column((axis + 2) % 4);
            //    basis.Column2 = rtx.rotation.Column((axis + 3) % 4);
            //    basis.Column3 = rtx.rotation.Column(axis);
            //    if (Utility.Sign(n[axis]) < 0)
            //        basis = Matrix4.zero - basis;
            //}
            var rotate = Matrix4.Transpose(Matrix4.LookAt(/*new Vector4(axis, Utility.Sign(n[axis]))*/n));
            basis = rotate * rtx.rotation;
            e = Vector4.Abs(rotate * eR);
        }

        //--------------------------------------------------------------------------------------------------
        // Get the most potential face candidate that Incidents N
        // That is: The face that most parallel to N
        public static void ComputeIncidentFace(Matrix4x5 itx, Vector4 e, Vector4 n, Vector4[] result)
        {
            n = (n / itx.rotation);

            int i = Vector4.MaxPerElemIdx(Vector4.Abs(n)), k = 0;

            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        var r = e;
                        r[i] *= -Utility.Sign(n[i]);
                        r[(i + 1) % 4] *= x;
                        r[(i + 2) % 4] *= y;
                        r[(i + 3) % 4] *= z;
                        result[k++] = itx * r;
                    }
        }

        //--------------------------------------------------------------------------------------------------
        // Axis-aligned clipping algo
        public static int Orthographic(float e, int axis, Vector4[] input, int length, Vector4[] result)
        {
            int count = 0;

            for (int i = 0; i < length; ++i)
            {
                Vector4 a = input[i], b = input[(i + 1) % length];

                float xa = a[axis], xb = b[axis],  da = Math.Abs(xa) - e, db = Math.Abs(xb) - e;

                if (da < 0 && db < 0)
                    result[count++] = b;
                else if (da < 0 ^ db < 0)
                {
                    // Intersection point between A and B
                    var cv = a + (b - a) * (da / (da - db));

                    if (da < 0)
                        result[count++] = cv;
                    else
                    {
                        result[count++] = cv;
                        result[count++] = b;
                    }
                }
                //else if (xa * xb < 0)
                //{
                //    result[count++] = a + (b - a) * ((e - xa) / (da - db));
                //    result[count++] = a + (b - a) * ((-e - xa) / (da - db));
                //}
            }

            return count;
        }

        static Vector4[] input = new Vector4[Common.MULTICONTACT_COUNT];
        static Vector4[] result = new Vector4[Common.MULTICONTACT_COUNT];
        static internal Vector4[] incident = new Vector4[8];

        //--------------------------------------------------------------------------------------------------
        // Four axis clipping via (an iterative algorithm) sutherland hodgman clipping
        // http://www.randygaul.net/2014/10/27/sutherland-hodgman-clipping/
        public static void Clip(Vector4 rPos, Vector4 e, Matrix4 basis, Vector4[] incident, Manifold m)
        {
            int count = 8;
            float d;

            for (int i = 0; i < 8; ++i)
                input[i] = (incident[i] - rPos) / basis;

            // Clip extent X
            if ((count = Orthographic(e.x, 0, input, count, result)) == 0)
                return;

            // Clip extent Y
            if ((count = Orthographic(e.y, 1, result, count, input)) == 0)
                return;

            // Clip extent Z
            if ((count = Orthographic(e.z, 2, input, count, result)) == 0)
                return;

            for (int i = 0; i < count; ++i)
            {
                // Clip extent W
                if ((d = result[i].w - e.w) < 0f)
                {
                    m.MakeContact(basis * result[i] + rPos, d);
                }
            }
        }
    }
}
