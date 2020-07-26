using System;
using UnityEngine;

namespace Engine4.Internal
{
    public static partial class Buffer4Extension
    {
        /// <summary> Transform vertices since last Align() </summary>
        public static void Transform (this Buffer4 buffer, Matrix4x5 matrix)
        {
            var vertices = buffer.m_Vertices;
            for (int i = buffer.offset; i < buffer.m_VerticesCount; i++)
            {
                vertices[i] = matrix * vertices[i];
            }
        }

        /// <summary> Translate and Transform vertices since last Align() </summary>
        public static void Transform(this Buffer4 buffer, Matrix4x5 matrix, Vector4 center)
        {
            var vertices = buffer.m_Vertices;
            for (int i = buffer.offset; i < buffer.m_VerticesCount; i++)
            {
                vertices[i] = matrix * (vertices[i] + center);
            }
        }

        /// <summary> Set a solid color (without uv) to remaining indices </summary>
        public static void Colorize(this Buffer4 buffer, Color color)
        {
            var prof = new VertexProfile(color);
            while (buffer.m_ProfilesCount < buffer.m_IndicesCount)
                buffer.AddProfile(prof);
        }

        ///// <summary> Set the color of vertices since last Align() </summary>
        //public static void Colorize(this Buffer4 buffer, Gradient color)
        //{
        //    if (!buffer.useProfile) return;
        //    var profiles = buffer.profiles;
        //    var offset = buffer.offset;
        //    for (int i = offset; i < profiles.Count; i++)
        //    {

        //        var c = profiles[i];
        //        c.color = color.Evaluate(Mathf.InverseLerp(offset, profiles.Count - 1, i));
        //        profiles[i] = c;
        //    }
        //}
        /// <summary>
        /// Automatically add vertices to indices buffer 
        /// since last Align() using given sequencing preset.
        /// </summary>
        public static void Sequence(this Buffer4 buffer, SequenceMode mode)
        {
            Sequence(buffer, mode, 0, -1);
        }

        /// <summary>
        /// Automatically add vertices to indices buffer 
        /// since last Align() using given sequencing preset.
        /// </summary>
        public static void Sequence(this Buffer4 buffer, SequenceMode mode, int start = 0, int count = -1)
        {
            buffer.offset += start;
            var end = count < 0 ? buffer.m_VerticesCount - buffer.offset : count;
            switch (mode)
            {
                case SequenceMode.Points:
                    for (int i = 0; i < end;) buffer.AddPoint(i++);
                    break;

                case SequenceMode.Lines:
                    for (int i = 0; i < end;) buffer.AddSegment(i++, i++);
                    break;

                case SequenceMode.LineStrip:
                    for (int i = 1; i < end;) buffer.AddSegment(i - 1, i++);
                    break;

                case SequenceMode.LineFan:
                    for (int i = 1; i < end;) buffer.AddSegment(0, i++);
                    break;

                case SequenceMode.LineLoop:
                    for (int i = 0; i < end;) buffer.AddSegment(i, (++i % end));
                    break;

                case SequenceMode.Triangles:
                    for (int i = 0; i < end;) buffer.AddTriangle(i++, i++, i++);
                    break;

                case SequenceMode.TriangleStrip:
                    for (int i = 2; i < end;) buffer.AddTriangle(i - 2, i - 1, i++);
                    break;

                case SequenceMode.TriangleFan:
                    for (int i = 1; i < end;) buffer.AddTriangle(0, i - 1, i++);
                    break;

                case SequenceMode.Quads:
                    for (int i = 0; i < end;) buffer.AddQuad(i++, i++, i++, i++);
                    break;

                case SequenceMode.QuadStrip:
                    for (int i = 2; i < end; i += 2) buffer.AddQuad(i - 2, i - 1, i + 1, i);
                    break;

                case SequenceMode.Polygon:
                    for (int i = 1; i < end;) buffer.AddTriangle(0, i, (++i % end));
                    break;

                case SequenceMode.Trimids:
                    for (int i = 0; i < end;) buffer.AddTrimid(i++, i++, i++, i++);
                    break;

                case SequenceMode.TrimidStrip:
                    for (int i = 3; i < end;) buffer.AddTrimid(i - 3, i - 2, i - 1, i++);
                    break;

                case SequenceMode.TrimidFan:
                    for (int i = 1; i < end;) buffer.AddTrimid(0, i++, i++, i++);
                    break;

                case SequenceMode.PyramidFan:
                    for (int i = 1; i < end;) buffer.AddPyramid(0, i++, i++, i++, i++);
                    break;

                case SequenceMode.PrismFan:
                    for (int i = 2; i < end; i += 2) buffer.AddPrism(i++, i++, 0, i + 1, i, 1);
                    break;

                case SequenceMode.Cubes:
                    for (int i = 0  ; i < end;) buffer.AddCube(i++, i++, i++, i++, i++, i++, i++, i++);
                    break;

                case SequenceMode.CubeStrip:
                    for (int i = 4; i < end;) buffer.AddCube(i - 4, i - 3, i - 2, i - 1, i++, i++, i++, i++);
                    break;

            }
            buffer.offset -= start;
        }



        /// <summary>
        /// Special sequencing tool when dealing with 1D, 2D, 3D or 4D grid vertices.
        /// </summary>
        /// <remarks>
        /// Set the grid count in the parameter.
        /// The nested order is for(x) => for(y) => for(z) => for(w).
        /// the starting point is always from the last Align()
        /// </remarks>
        public static void SequenceGrid(this Buffer4 buffer, int x, int y = 1, int z = 1, int w = 1)
        { 
#if UNITY_EDITOR
            // Naive checks
            if (Math.Min(Math.Min(x, w), Math.Min(y, z)) < 1) throw new ArgumentOutOfRangeException("Invalid sequence grid size!");
            if (x * y * z * w > buffer.m_VerticesCount - buffer.offset) throw new ArgumentOutOfRangeException("The grid size is out of range!");
#endif

             _seqY = y; _seqZ = z; _seqW = w;// _seqX = x;

            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    for (int i = 0; i < x; i++)
                        for (int j = 0; j < y; j++)
                            for (int k = 0; k < z; k++)
                                for (int l = 0; l < w; l++)
                                    buffer.AddPoint(SequenceIndex(i, j, k, l));

                    break;
                case SimplexMode.Line:
                    for (int i = 0; i < x; i++)
                        for (int j = 0; j < y; j++)
                            for (int k = 0; k < z; k++)
                                for (int l = 0; l < w; l++)
                                {
                                    var N = SequenceIndex(i, j, k, l);
                                    if (i > 0) buffer.AddSegment(SequenceIndex(i - 1, j, k, l), N); // X edges
                                    if (j > 0) buffer.AddSegment(SequenceIndex(i, j - 1, k, l), N); // Y edges
                                    if (k > 0) buffer.AddSegment(SequenceIndex(i, j, k - 1, l), N); // Z edges
                                    if (l > 0) buffer.AddSegment(SequenceIndex(i, j, k, l - 1), N); // W edges
                                }
                    break;
                case SimplexMode.Triangle:
                    for (int i = 0; i < x; i++)
                    {
                        int I = SequenceIndex(i, 0, 0, 0), pI = SequenceIndex(i - 1, 0, 0, 0);
                        for (int j = 0; j < y; j++)
                        {
                            int J = SequenceIndex(0, j, 0, 0), pJ = SequenceIndex(0, j - 1, 0, 0);
                            for (int k = 0; k < z; k++)
                            {
                                int K = SequenceIndex(0, 0, k, 0), pK = SequenceIndex(0, 0, k - 1, 0);
                                for (int l = 0; l < w; l++)
                                {
                                    int L = SequenceIndex(0, 0, 0, l), pL = SequenceIndex(0, 0, 0, l - 1), N = I + J + K + L;
                                    
                                    if (k > 0 & j > 0) buffer.AddQuad(N, I + J + pK + L, I + pJ + pK + L, I + pJ + K + L); // ZY faces
                                    if (i > 0 & j > 0) buffer.AddQuad(N, pI + J + K + L, pI + pJ + K + L, I + pJ + K + L); // XY faces
                                    if (i > 0 & k > 0) buffer.AddQuad(N, I + J + pK + L, pI + J + pK + L, pI + J + K + L); // XZ faces
                                    if (i > 0 & l > 0) buffer.AddQuad(N, I + J + K + pL, pI + J + K + pL, pI + J + K + L); // WX faces
                                    if (j > 0 & l > 0) buffer.AddQuad(N, I + J + K + pL, I + pJ + K + pL, I + pJ + K + L); // WY faces
                                    if (k > 0 & l > 0) buffer.AddQuad(N, I + J + K + pL, I + J + pK + pL, I + J + pK + L); // WZ faces
                                }
                            }
                        }
                    }
                    break;
                case SimplexMode.Tetrahedron:
                    for (int i = 0; i < x; i++)
                    {
                        int I = SequenceIndex(i, 0, 0, 0), pI = SequenceIndex(i - 1, 0, 0, 0);
                        for (int j = 0; j < y; j++)
                        {
                            int J = SequenceIndex(0, j, 0, 0), pJ = SequenceIndex(0, j - 1, 0, 0);
                            for (int k = 0; k < z; k++)
                            {
                                int K = SequenceIndex(0, 0, k, 0), pK = SequenceIndex(0, 0, k - 1, 0);
                                for (int l = 0; l < w; l++)
                                {
                                    int L = SequenceIndex(0, 0, 0, l), pL = SequenceIndex(0, 0, 0, l - 1), N = I + J + K + L;

                                    if (i > 0 & j > 0 & k > 0) buffer.AddCube(N, pI + J + K + L, pI + pJ + K + L, I + pJ + K + L, I + J + pK + L, pI + J + pK + L, pI + pJ + pK + L, I + pJ + pK + L); // XYZ cube
                                    if (i > 0 & j > 0 & l > 0) buffer.AddCube(N, pI + J + K + L, pI + pJ + K + L, I + pJ + K + L, I + J + K + pL, pI + J + K + pL, pI + pJ + K + pL, I + pJ + K + pL); // XYW cube
                                    if (i > 0 & k > 0 & l > 0) buffer.AddCube(N, pI + J + K + L, pI + J + pK + L, I + J + pK + L, I + J + K + pL, pI + J + K + pL, pI + J + pK + pL, I + J + pK + pL); // XZW cube
                                    if (j > 0 & k > 0 & l > 0) buffer.AddCube(N, I + pJ + K + L, I + pJ + pK + L, I + J + pK + L, I + J + K + pL, I + pJ + K + pL, I + pJ + pK + pL, I + J + pK + pL); // YZW cube
                                }
                            }
                        }
                    }
                    break;
            }
        }

        static int _seqW, _seqZ, _seqY;// _seqX,

        static int SequenceIndex(int i, int j, int k, int l)
        {
            return l + _seqW * (k + _seqZ * (j + _seqY * i));
        }
        
    }
    /// <summary> Sequincing preset to define the order of indices </summary>
    /// <remarks> It's recommended to choose based on the simplex type </remarks>
    /// <seealso cref="Buffer4Extension.Sequence(Buffer4, SequenceMode)"/>
    public enum SequenceMode
    {
        /// <summary> Individual Points: 0 - 1 - 2 - 3 ... n </summary>
        Points = 0,
        /// <summary> Individual Segments: 0,1 - 2,3 - 4,5 ... n-1,n </summary>
        Lines,
        /// <summary> Continous Segments: 0,1 - 1,2 - 2,3 ... n-1,n </summary>
        LineStrip,
        /// <summary> Branched Segments: 0,1 - 0,2 - 0,3 ... 0,n </summary>
        LineFan,
        /// <summary> Continous Cycled Segments: 0,1 - 1,2 - 2,3 ... n,0 </summary>
        LineLoop,
        /// <summary> Individual Triangles: 0,1,2 - 3,4,5 ... n-2,n-1,n </summary>
        Triangles,
        /// <summary> Continous Triangles: 0,1,2 - 1,2,3 ... n-2,n-1,n </summary>
        TriangleStrip,
        /// <summary> Branched Triangles: 0,1,2 - 0,2,3 ... 0,n-1,n </summary>
        TriangleFan,
        /// <summary> Individual Quads: 0,1,2,3 - 4,5,6,7 ... n-3,n-2,n-1,n </summary>
        Quads,
        /// <summary> Continous Quads: 0,1,3,2 - 2,3,5,4 ... n-3,n-2,n,n-1 </summary>
        QuadStrip,
        /// <summary> Branched Cycled Triangles: 0,1,2 - 0,2,3 ... 0,n,1 </summary>
        Polygon,
        /// <summary> Individual Trimids: 0,1,2,3 - 4,5,6,7 ... n-3,n-2,n-1,n </summary>
        Trimids,
        /// <summary> Continous Trimids: 0,1,2,3 - 1,2,3,4 ... n-3,n-2,n-1,n </summary>
        TrimidStrip,
        /// <summary> Branched Trimids: 0,1,2,3 - 0,4,5,6 ... 0,n-2,n-1,n </summary>
        TrimidFan,
        /// <summary> Branched Pyramids: 0,1,2,3,4 - 0,5,6,7,8 ... 0,n-3,n-2,n-1,n </summary>
        PyramidFan,
        /// <summary> Branched Prism: 2,3,0,5,4,1 - 6,7,0,9,8,1 ... n-3,n-2,0,n,n-1,1 </summary>
        PrismFan,
        /// <summary> Individual Cubes: 0,1,2,3,4 - 0,5,6,7,8 ... 0,n-3,n-2,n-1,n </summary>
        Cubes,
        /// <summary> Continous Cube: 0,1,2,3,4 - 0,5,6,7,8 ... 0,n-3,n-2,n-1,n </summary>
        CubeStrip,
    }
}
