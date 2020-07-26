using Math = UnityEngine.Mathf;

namespace Engine4.Internal
{
    /// <summary>
    /// Collection of premade, reusable primitive shapes
    /// </summary>
    public static class Buffer4Template
    {

        /// <summary>
        /// Draw hypercube to the buffer
        /// </summary>
        public static void MakeHypercube(Buffer4 buffer, Vector4 extent)
        {
            buffer.Align();

            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                        for (int w = -1; w <= 1; w += 2)
                        {
                            var v = new Vector4(x, y, z, w);
                            buffer.AddVertex(v * extent);
                        }

            buffer.SequenceGrid(2, 2, 2, 2);
        }

        /// <summary>
        /// Add basic cube UV profile
        /// </summary>
        public static void DrawCubeUV(Buffer4 buffer)
        {
            buffer.AddCube(new VertexProfile(new UnityEngine.Vector4(0, 0, 1, 0)),
                    new VertexProfile(new UnityEngine.Vector4(0, 0, 0, 0)),
                    new VertexProfile(new UnityEngine.Vector4(1, 0, 0, 0)),
                    new VertexProfile(new UnityEngine.Vector4(1, 0, 1, 0)),
                    new VertexProfile(new UnityEngine.Vector4(0, 1, 1, 0)),
                    new VertexProfile(new UnityEngine.Vector4(0, 1, 0, 0)),
                    new VertexProfile(new UnityEngine.Vector4(1, 1, 0, 0)),
                    new VertexProfile(new UnityEngine.Vector4(1, 1, 1, 0)));
        }

        /// <summary>
        /// Add basic quad UV profile
        /// </summary>
        public static void DrawQuadUV(Buffer4 buffer)
        {
            buffer.AddQuad(new VertexProfile(new UnityEngine.Vector4(1, 0, 0, 0)),
                new VertexProfile(new UnityEngine.Vector4(0, 0, 0, 0)),
                new VertexProfile(new UnityEngine.Vector4(0, 1, 0, 0)),
                new VertexProfile(new UnityEngine.Vector4(1, 1, 0, 0)));
        }


        /// <summary>
        /// Draw subdividable hypersphere to the buffer
        /// </summary>
        public static void MakeHypersphere(Buffer4 buffer, float radius, int subdivision = 8)
        {

            buffer.Align();

            float invSub = Math.PI / subdivision;

            for (int x = 0; x <= subdivision * 2; x++)
                for (int y = 0; y <= subdivision; y++)
                    for (int z = 0; z <= subdivision; z++)
                    {
                        float lat = y * invSub, lon = x * invSub, azm = z * invSub;
                        float clat = Math.Cos(lat), slat = Math.Sin(lat);
                        float clon = Math.Cos(lon), slon = Math.Sin(lon);
                        float cazm = Math.Cos(azm), sazm = Math.Sin(azm);

                        buffer.AddVertex(new Vector4(
                            sazm * slat * slon, sazm * clat,
                            sazm * slat * clon, cazm) * radius);
                    }

            buffer.SequenceGrid(subdivision * 2 + 1, subdivision + 1, subdivision + 1);

        }


        /// <summary>
        /// Draw hypersphere meridians to the buffer
        /// </summary>
        public static void MakeHypersphereMeridians(Buffer4 buffer, float radius, int subdivision = 24)
        {

            var aligned = buffer.Snapshot();

            buffer.Align();

            float invSub = 2 * Math.PI / subdivision;

            for (int x = 0; x <= subdivision * 2; x++)
                for (int y = 0; y <= subdivision; y++)
                {
                    float lat = y * invSub, lon = x * invSub;
                    float clat = Math.Cos(lat), slat = Math.Sin(lat);
                    float clon = Math.Cos(lon), slon = Math.Sin(lon);

                    buffer.AddVertex(new Vector4(slat * slon, clat, slat * clon, 0) * radius);
                }

            buffer.SequenceGrid(subdivision * 2 + 1, subdivision + 1);
            buffer.Align();

            for (int x = 0; x <= subdivision * 2; x++)
                for (int z = 0; z <= subdivision; z++)
                {
                    float lon = x * invSub, azm = z * invSub;
                    float clon = Math.Cos(lon), slon = Math.Sin(lon);
                    float cazm = Math.Cos(azm), sazm = Math.Sin(azm);

                    buffer.AddVertex(new Vector4(sazm * slon, 0, sazm * clon, cazm) * radius);
                }

            buffer.SequenceGrid(subdivision * 2 + 1, subdivision + 1);
            buffer.Align();

            for (int y = 0; y <= subdivision; y++)
                for (int z = 0; z <= subdivision; z++)
                {
                    float lat = y * invSub, azm = z * invSub;
                    float clat = Math.Cos(lat), slat = Math.Sin(lat);
                    float cazm = Math.Cos(azm), sazm = Math.Sin(azm);

                    buffer.AddVertex(new Vector4(sazm * slat, sazm * clat, 0, cazm) * radius);
                }

            buffer.SequenceGrid(subdivision + 1, subdivision + 1);

            buffer.offset = aligned;
        }

        /// <summary>
        /// Draw frustum to the buffer
        /// </summary>
        public static void MakeHyperfrustum(Buffer4 buffer, float min, float max, float focal, float perspectiveness)
        {

            buffer.Align();

            var nearmedian = new Vector4(3, min);
            var nearratio = Math.Lerp(min / focal, 1, perspectiveness);
            var nearextent = new Vector4(nearratio, nearratio, nearratio, 0) * 0.99f;
            var farmedian = new Vector4(3, max);
            var farratio = Math.Lerp(max / focal, 1, perspectiveness);
            var farextent = new Vector4(farratio, farratio, farratio, 0) * 0.99f;

            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                        for (int w = -1; w <= 1; w += 2)
                        {
                            var v = new Vector4(x, y, z, 0);
                            if (w == -1)
                                buffer.AddVertex(v * nearextent + nearmedian);
                            else
                                buffer.AddVertex(v * farextent + farmedian);
                        }

            buffer.SequenceGrid(2, 2, 2, 2);
        }

    }
}
