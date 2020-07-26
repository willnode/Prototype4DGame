using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Built-in modeler to create regular shapes in 4D.
    /// </summary>
    /// <remarks>
    /// See online resources [here] to learn more about regular shapes in 4D.
    /// </remarks>
    public partial class Primitive4 : Modeler4
    {
        /// <summary>
        /// Primitive shapes selections
        /// </summary>
        public enum Shape
        {
            /// <summary> 1-cell </summary>
            Hyperplane,
            /// <summary> 5-cell </summary>
            Pentatope,
            /// <summary> 8-cell </summary>
            Tesseract,
            /// <summary> 16-cell </summary>
            Hexdecahedroid,
            /// <summary> 24-cell </summary>
            Icosatetrahedroid,
            /// <summary> 120-cell </summary>
            Hecatonicosahedroid,
            /// <summary> 600-cell </summary>
            Hexacosidedroid,
        }

        /// <summary>
        /// Selected shape to be generated
        /// </summary>
        /// <seealso cref="Shape"/>
        public Shape shape = Shape.Tesseract;
        /// <summary>
        /// Radius (size) of the model
        /// </summary>
        public float radius = 0.5f;


        // Golden ratio
        const float _GR = 1.6180339887498948482045868343656f;
        // Square roots
        const float _SQRT3 = 1.7320508075688772935274463415059f;
        const float _SQRT5 = 2.2360679774997896964091736687313f;
        const float _SQRT6 = 2.4494897427831780981972840747059f;
        const float _SQRT10 = 3.1622776601683793319988935444327f;


        /// 
        [HideInDocs]
        public override void CreateModel(Buffer4 buffer)
        {
            // Each shape done in separate methods..
            switch (shape)
            {
                case Shape.Hyperplane:
                    MakeHyperplane(buffer);
                    break;
                case Shape.Pentatope:
                    MakePentatope(buffer);
                    break;
                case Shape.Tesseract:
                    MakeTesseract(buffer);
                    break;
                case Shape.Hexdecahedroid:
                    MakeHexdecahedroid(buffer);
                    break;
                case Shape.Icosatetrahedroid:
                    MakeIcosatetrahedroid(buffer);
                    break;
                case Shape.Hecatonicosahedroid:
                    MakeHecatonicosahedroid(buffer);
                    break;
                case Shape.Hexacosidedroid:
                    MakeHexacosidedroid(buffer);
                    break;
            }
        }

        void MakeHyperplane(Buffer4 buffer)
        {
            // Permutation of (-+1, 0, -+1, -+1)
            for (int w = -1; w <= 1; w += 2)
                for (int x = -1; x <= 1; x += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        var v = new Vector4(x, 0, z, w);
                        buffer.AddVertex(v * radius);
                    }

            // This is 3D grid, so can be done with .SequenceGrid
            buffer.SequenceGrid(2, 2, 2);

            if (renderer4.profile.HasUV())
            {
                if (buffer.simplex == SimplexMode.Tetrahedron)
                    Buffer4Template.DrawCubeUV(buffer);
                else if (buffer.simplex == SimplexMode.Triangle)
                    for (int i = 0; i < 6; i++)
                        Buffer4Template.DrawQuadUV(buffer); // six faces
            }
        }

        static Vector4[] _pentatopeVertices = {
            new Vector4(1 / _SQRT6, -1 / _SQRT10, 1 / _SQRT3, -1),
            new Vector4(1 / _SQRT6, -1 / _SQRT10, 1 / _SQRT3, 1),
            new Vector4(1 / _SQRT6, -1 / _SQRT10, -2 / _SQRT3, 0),
            new Vector4(-Mathf.Sqrt(3 / 2f), -1 / _SQRT10, 0, 0),
            new Vector4(0, 2 * Mathf.Sqrt(2 / 5f), 0, 0),
        };

        void MakePentatope(Buffer4 buffer)
        {
            // Vertices according to the wiki...
            for (int a = 0; a < 5; a++)
                buffer.AddVertex(_pentatopeVertices[a] * radius);

            // Notice the pattern...
            buffer.AddTrimid(1, 2, 3, 4);
            buffer.AddTrimid(0, 2, 3, 4);
            buffer.AddTrimid(0, 1, 3, 4);
            buffer.AddTrimid(0, 1, 2, 4);
            buffer.AddTrimid(0, 1, 2, 3);
        }

        /// <summary>
        /// Make tesseract.
        /// </summary>
        void MakeTesseract(Buffer4 buffer)
        {
            // Permutation of (-+1, -+1, -+1, -+1)
            // Idk why the order have to be like this?
            for (int w = -1; w <= 1; w += 2)
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        for (int z = -1; z <= 1; z += 2)
                        {
                            var v = new Vector4(x, y, z, w);
                            buffer.AddVertex(v * radius);
                        }

            // This is 4D Grid, and can be done with .SequenceGrid actually
            //buffer.AddCube(00, 02, 06, 04, 08, 10, 14, 12);
            //buffer.AddCube(01, 03, 07, 05, 09, 11, 15, 13);
            //buffer.AddCube(00, 01, 03, 02, 04, 05, 07, 06);
            //buffer.AddCube(08, 09, 11, 10, 12, 13, 15, 14);
            //buffer.AddCube(00, 01, 03, 02, 08, 09, 11, 10);
            //buffer.AddCube(04, 05, 07, 06, 12, 13, 15, 14);
            //buffer.AddCube(00, 01, 05, 04, 08, 09, 13, 12);
            //buffer.AddCube(02, 03, 07, 06, 10, 11, 15, 14);

            buffer.SequenceGrid(2, 2, 2, 2);

            if (renderer4.profile.HasUV())
            {
                if (buffer.simplex == SimplexMode.Triangle)
                    for (int i = 0; i < 24; i++)
                        Buffer4Template.DrawQuadUV(buffer); // 24 faces
                else if (buffer.simplex == SimplexMode.Tetrahedron)
                    for (int i = 0; i < 8; i++)
                        Buffer4Template.DrawCubeUV(buffer);
            }
        }

        void MakeHexdecahedroid(Buffer4 buffer)
        {
            for (int a = 0; a < 4; a += 1)
                for (int b = -1; b <= 1; b += 2)
                {
                    var v = Vector4.zero;
                    v[a] = b;
                    buffer.AddVertex(v * radius);
                }

            buffer.AddTrimid(0, 2, 4, 6);
            buffer.AddTrimid(1, 3, 5, 7);

            buffer.AddTrimid(1, 2, 4, 6);
            buffer.AddTrimid(0, 3, 4, 6);
            buffer.AddTrimid(0, 2, 5, 6);
            buffer.AddTrimid(0, 2, 4, 7);

            buffer.AddTrimid(0, 3, 5, 7);
            buffer.AddTrimid(1, 2, 5, 7);
            buffer.AddTrimid(1, 3, 4, 7);
            buffer.AddTrimid(1, 3, 5, 6);

            buffer.AddTrimid(1, 3, 4, 6);
            buffer.AddTrimid(1, 2, 5, 6);
            buffer.AddTrimid(1, 2, 4, 7);
            buffer.AddTrimid(0, 3, 5, 6);
            buffer.AddTrimid(0, 3, 4, 7);
            buffer.AddTrimid(0, 2, 5, 7);

        }

        // Dual pyramid spec: a & b is the pole, the rest is the meridian
        void AddDualPyramid(Buffer4 buffer, int a, int b, int c, int d, int e, int f)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    buffer.AddPoint(a, b, c, d, e, f);
                    break;
                case SimplexMode.Line:
                    buffer.AddQuad(c, d, e, f);
                    buffer.AddBySequence(SequenceMode.LineFan, a, c, d, e, f);
                    buffer.AddBySequence(SequenceMode.LineFan, b, c, d, e, f);
                    break;
                case SimplexMode.Triangle:
                    buffer.AddPolygon(a, c, d, e, f);
                    buffer.AddPolygon(b, c, d, e, f);
                    break;
                case SimplexMode.Tetrahedron:
                    buffer.AddPyramid(a, d, c, f, e);
                    buffer.AddPyramid(b, c, d, e, f);
                    break;
            }
        }

        void MakeIcosatetrahedroid(Buffer4 buffer)
        {
            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                        for (int w = -1; w <= 1; w += 2)
                        {
                            buffer.AddVertex(new Vector4(x, y, z, w) * 0.5f * radius);
                        }

            for (int a = 0; a < 4; a += 1)
                for (int b = -1; b <= 1; b += 2)
                {
                    buffer.AddVertex(new Vector4(a, b) * radius);
                }

            // North/south
            AddDualPyramid(buffer, 16, 22, 0, 2, 6, 4);
            AddDualPyramid(buffer, 17, 22, 8, 10, 14, 12);
            AddDualPyramid(buffer, 18, 22, 0, 2, 10, 8);
            AddDualPyramid(buffer, 19, 22, 4, 6, 14, 12);
            AddDualPyramid(buffer, 20, 22, 0, 4, 12, 8);
            AddDualPyramid(buffer, 21, 22, 2, 6, 14, 10);

            AddDualPyramid(buffer, 16, 23, 1, 3, 7, 5);
            AddDualPyramid(buffer, 17, 23, 9, 11, 15, 13);
            AddDualPyramid(buffer, 18, 23, 1, 3, 11, 9);
            AddDualPyramid(buffer, 19, 23, 5, 7, 15, 13);
            AddDualPyramid(buffer, 20, 23, 1, 5, 13, 9);
            AddDualPyramid(buffer, 21, 23, 3, 7, 15, 11);

            //Equator
            AddDualPyramid(buffer, 16, 20, 0, 4, 5, 1);
            AddDualPyramid(buffer, 16, 21, 2, 6, 7, 3);
            AddDualPyramid(buffer, 17, 20, 8, 12, 13, 9);
            AddDualPyramid(buffer, 17, 21, 10, 14, 15, 11);

            AddDualPyramid(buffer, 16, 19, 4, 5, 7, 6);
            AddDualPyramid(buffer, 17, 19, 12, 13, 15, 14);
            AddDualPyramid(buffer, 17, 18, 8, 9, 11, 10);
            AddDualPyramid(buffer, 16, 18, 0, 1, 3, 2);

            AddDualPyramid(buffer, 18, 20, 0, 1, 9, 8);
            AddDualPyramid(buffer, 18, 21, 2, 3, 11, 10);
            AddDualPyramid(buffer, 19, 20, 4, 5, 13, 12);
            AddDualPyramid(buffer, 19, 21, 6, 7, 15, 14);
        }


    }
}