using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Built-in modeler to create a cylinder.
    /// </summary>
    /// <remarks>
    /// This component will copy the base mesh, duplicate, and extrude them overward,
    /// Resembling a 'cylinder', but with custom 'flat' mesh.
    /// </remarks>
    public class Cylinder4 : Modeler4
    {
       
        /// <summary>
        /// The base mesh as cylinder's 'surface'
        /// </summary>
        public Mesh mesh;
        /// <summary>
        /// Scale of the cylinder surface
        /// </summary>
        public float radius = 1;
        /// <summary>
        /// Height of the cylinder.
        /// </summary>
        public float height = 1;

        /// 
        [HideInDocs]
        public override void CreateModel(Buffer4 buffer)
        {
            // Check access
            if (!mesh || !mesh.isReadable)
                return;

            // Fetch data
            var v = ListPool<Vector3>.Pop();
            mesh.GetVertices(v);
            var t = ListPool<int>.Pop();
            mesh.GetTriangles(t, 0);
            var h = height * 0.5f;

            // Push vertices for north & south pole
            buffer.Align();
            for (int i = 0; i < v.Count; i++)
                buffer.AddVertex((v[i] * radius).SetW(-h));
            for (int i = 0; i < v.Count; i++)
                buffer.AddVertex((v[i] * radius).SetW(h));

            // How we connect them?
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    // Pretty straightforward...
                    buffer.Sequence(SequenceMode.Points);
                    break;
                case SimplexMode.Line:
                    // In every triangle...
                    for (int i = 0; i < t.Count; i += 3)
                    {
                        int a = t[i], b = t[i + 1], c = t[i + 2], o = v.Count;

                        // Two poles
                        buffer.AddTriangle(a, b, c);
                        buffer.AddTriangle(a + o, b + o, c + o);
                        // Meridians
                        buffer.AddSegment(a, a + o);
                        buffer.AddSegment(b, b + o);
                        buffer.AddSegment(c, c + o);
                    }
                    break;
                case SimplexMode.Triangle:
                    // Two poles
                    buffer.Sequence(SequenceMode.Triangles);
                    // Meridians
                    buffer.SequenceGrid(v.Count, 2); 
                    break;
                case SimplexMode.Tetrahedron:
                    for (int i = 0; i < t.Count; i += 3)
                    {
                        int a = t[i], b = t[i + 1], c = t[i + 2], o = v.Count;

                        // Two poles (n-gon trick)
                        buffer.AddTrimid(0, a, b, c);
                        buffer.AddTrimid(o, a + o, b + o, c + o);
                        // Meridians
                        buffer.AddPrism(a, b, c, a + o, b + o, c + o);
                    }
                    break;
            }

           

            ListPool<Vector3>.Push(v);
            ListPool<int>.Push(t);

        }
    }
}