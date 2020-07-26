using System;

namespace Engine4.Internal
{
    /// <summary>
    /// Buffer4 extension helper and utilities
    /// </summary>
    public static partial class Buffer4Extension
    {

        // Regex find: ([\(,])(?!int|mode|v)(.+?)([\)\,])
        // Regex replace: $1v[$2]$3

        /// <summary>
        /// Add by given sequence
        /// </summary>
        public static void AddBySequence(this Buffer4 buffer, SequenceMode mode, params int[] v)
        {
            // Copy of the original sequence

            var end = v.Length;
            switch (mode)
            {
                case SequenceMode.Points:
                    for (int i = 0; i < end;) buffer.AddPoint(v[i++]);
                    break;

                case SequenceMode.Lines:
                    for (int i = 0; i < end;) buffer.AddSegment(v[i++], v[i++]);
                    break;

                case SequenceMode.LineStrip:
                    for (int i = 1; i < end;) buffer.AddSegment(v[i - 1], v[i++]);
                    break;

                case SequenceMode.LineFan:
                    for (int i = 1; i < end;) buffer.AddSegment(v[0], v[i++]);
                    break;

                case SequenceMode.LineLoop:
                    for (int i = 0; i < end;) buffer.AddSegment(v[i], (v[++i % end]));
                    break;

                case SequenceMode.Triangles:
                    for (int i = 0; i < end;) buffer.AddTriangle(v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.TriangleStrip:
                    for (int i = 2; i < end;) buffer.AddTriangle(v[i - 2], v[i - 1], v[i++]);
                    break;

                case SequenceMode.TriangleFan:
                    for (int i = 1; i < end;) buffer.AddTriangle(v[0], v[i - 1], v[i++]);
                    break;

                case SequenceMode.Quads:
                    for (int i = 0; i < end;) buffer.AddQuad(v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.QuadStrip:
                    for (int i = 2; i < end; i += 2) buffer.AddQuad(v[i - 2], v[i - 1], v[i + 1], v[i]);
                    break;

                case SequenceMode.Polygon:
                    for (int i = 1; i < end;) buffer.AddTriangle(v[0], v[i], (v[++i % end]));
                    break;

                case SequenceMode.Trimids:
                    for (int i = 0; i < end;) buffer.AddTrimid(v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.TrimidStrip:
                    for (int i = 3; i < end;) buffer.AddTrimid(v[i - 3], v[i - 2], v[i - 1], v[i++]);
                    break;

                case SequenceMode.TrimidFan:
                    for (int i = 1; i < end;) buffer.AddTrimid(v[0], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.PyramidFan:
                    for (int i = 1; i < end;) buffer.AddPyramid(v[0], v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.PrismFan:
                    for (int i = 2; i < end; i += 2) buffer.AddPrism(v[i++], v[i++], v[0], v[i + 1], v[i], v[1]);
                    break;

                case SequenceMode.Cubes:
                    for (int i = 0; i < end;) buffer.AddCube(v[i++], v[i++], v[i++], v[i++], v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.CubeStrip:
                    for (int i = 4; i < end;) buffer.AddCube(v[i - 4], v[i - 3], v[i - 2], v[i - 1], v[i++], v[i++], v[i++], v[i++]);
                    break;

            }
        }

        /// <summary>
        /// Add by given sequence
        /// </summary>
        public static void AddBySequence(this Buffer4 buffer, SequenceMode mode, params VertexProfile[] v)
        {
            // Copy of the original sequence

            var end = v.Length;
            switch (mode)
            {
                case SequenceMode.Points:
                    for (int i = 0; i < end;) buffer.AddPoint(v[i++]);
                    break;

                case SequenceMode.Lines:
                    for (int i = 0; i < end;) buffer.AddSegment(v[i++], v[i++]);
                    break;

                case SequenceMode.LineStrip:
                    for (int i = 1; i < end;) buffer.AddSegment(v[i - 1], v[i++]);
                    break;

                case SequenceMode.LineFan:
                    for (int i = 1; i < end;) buffer.AddSegment(v[0], v[i++]);
                    break;

                case SequenceMode.LineLoop:
                    for (int i = 0; i < end;) buffer.AddSegment(v[i], (v[++i % end]));
                    break;

                case SequenceMode.Triangles:
                    for (int i = 0; i < end;) buffer.AddTriangle(v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.TriangleStrip:
                    for (int i = 2; i < end;) buffer.AddTriangle(v[i - 2], v[i - 1], v[i++]);
                    break;

                case SequenceMode.TriangleFan:
                    for (int i = 1; i < end;) buffer.AddTriangle(v[0], v[i - 1], v[i++]);
                    break;

                case SequenceMode.Quads:
                    for (int i = 0; i < end;) buffer.AddQuad(v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.QuadStrip:
                    for (int i = 2; i < end; i += 2) buffer.AddQuad(v[i - 2], v[i - 1], v[i + 1], v[i]);
                    break;

                case SequenceMode.Polygon:
                    for (int i = 1; i < end;) buffer.AddTriangle(v[0], v[i], (v[++i % end]));
                    break;

                case SequenceMode.Trimids:
                    for (int i = 0; i < end;) buffer.AddTrimid(v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.TrimidStrip:
                    for (int i = 3; i < end;) buffer.AddTrimid(v[i - 3], v[i - 2], v[i - 1], v[i++]);
                    break;

                case SequenceMode.TrimidFan:
                    for (int i = 1; i < end;) buffer.AddTrimid(v[0], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.PyramidFan:
                    for (int i = 1; i < end;) buffer.AddPyramid(v[0], v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.PrismFan:
                    for (int i = 2; i < end; i += 2) buffer.AddPrism(v[i++], v[i++], v[0], v[i + 1], v[i], v[1]);
                    break;

                case SequenceMode.Cubes:
                    for (int i = 0; i < end;) buffer.AddCube(v[i++], v[i++], v[i++], v[i++], v[i++], v[i++], v[i++], v[i++]);
                    break;

                case SequenceMode.CubeStrip:
                    for (int i = 4; i < end;) buffer.AddCube(v[i - 4], v[i - 3], v[i - 2], v[i - 1], v[i++], v[i++], v[i++], v[i++]);
                    break;

            }
        }

        /// <summary>
        /// Add points.
        /// </summary>
        public static void AddPoint(this Buffer4 buffer, params int[] v)
        {
            AddBySequence(buffer, SequenceMode.Points, v);
        }

        /// <summary>
        /// Add segments.
        /// </summary>
        public static void AddSegment(this Buffer4 buffer, params int[] v)
        {
#if UNITY_EDITOR
            if (v.Length % 2 != 0) throw new ArgumentException("Invalid parameter count!");
#endif
            AddBySequence(buffer, SequenceMode.Lines, v);
        }

        /// <summary>
        /// Add triangles.
        /// </summary>
        public static void AddTriangle(this Buffer4 buffer, params int[] v)
        {
#if UNITY_EDITOR
            if (v.Length % 3 != 0) throw new ArgumentException("Invalid parameter count!");
#endif
            AddBySequence(buffer, SequenceMode.Triangles, v);
        }

        /// <summary>
        /// Add quads.
        /// </summary>
        public static void AddQuad(this Buffer4 buffer, params int[] v)
        {
#if UNITY_EDITOR
            if (v.Length % 4 != 0) throw new ArgumentException("Invalid parameter count!");
#endif
            AddBySequence(buffer, SequenceMode.Quads, v);
        }

        /// <summary>
        /// Add trimids.
        /// </summary>
        public static void AddTrimid(this Buffer4 buffer, params int[] v)
        {
#if UNITY_EDITOR
            if (v.Length % 4 != 0) throw new ArgumentException("Invalid parameter count!");
#endif
            AddBySequence(buffer, SequenceMode.Trimids, v);
        }

        /// <summary>
        /// Add polygon points/wires/surfaces.
        /// </summary>
        public static void AddPolygon(this Buffer4 buffer, params int[] v)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    AddBySequence(buffer, SequenceMode.Points, v);
                    break;
                case SimplexMode.Line:
                    AddBySequence(buffer, SequenceMode.LineLoop, v);
                    break;
                case SimplexMode.Triangle:
                    AddBySequence(buffer, SequenceMode.Polygon, v);
                    break;
            }
        }

        /// <summary>
        /// Add multiple vertexes.
        /// </summary>
        /// <remarks>
        /// The returning value is the vertex index for the first vertex.
        /// </remarks>
        public static int AddVertex(this Buffer4 buffer, params Vector4[] v)
        {
            buffer.EnsureVertices(v.Length);
            for (int i = 0; i < v.Length; i++)
            {
                buffer.AddVertex(v[i]);
            }
            return buffer.m_VerticesCount - 1 - buffer.offset - v.Length;

        }

        /// <summary>
        /// Append buffer content to another buffer
        /// </summary>
        public static void CopyTo(this Buffer4 origin, Buffer4 dest)
        {
            if (dest.simplex != origin.simplex)
                throw new InvalidOperationException("Can't copy with different simplex mode!");

            dest.EnsureVertices(origin.m_VerticesCount);
            dest.EnsureIndices(origin.m_IndicesCount);
            dest.EnsureProfiles(origin.m_ProfilesCount);
            Array.Copy(origin.m_Vertices, 0, dest.m_Vertices, dest.m_VerticesCount, origin.m_VerticesCount);
            Array.Copy(origin.m_Indices, 0, dest.m_Indices, dest.m_IndicesCount, origin.m_IndicesCount);
            Array.Copy(origin.m_Profiles, 0, dest.m_Profiles, dest.m_IndicesCount, origin.m_ProfilesCount);

            
            if (dest.m_VerticesCount > 0)
            {
                var offset = dest.m_VerticesCount;
                for (int i = dest.m_IndicesCount; i < dest.m_Indices.Length; i++)
                    dest.m_Indices[i] += offset;
            }

            dest.m_VerticesCount += origin.m_VerticesCount;
            dest.m_ProfilesCount += origin.m_ProfilesCount;
            dest.m_IndicesCount += origin.m_IndicesCount;
        }

        /// <summary>
        /// Append or overwrite buffer content to another buffer
        /// </summary>
        public static void CopyTo(this Buffer4 origin, Buffer4 dest, bool overwrite)
        {
            if (overwrite) dest.Clear(origin.simplex);
            CopyTo(origin, dest);

        }
    }
}
