using Engine4.Internal;
using UnityEngine;
using Math = UnityEngine.Mathf;

namespace Engine4.Rendering
{
    /// <summary>
    /// Built-in modeler to create a sphere in 4D.
    /// </summary>
    /// <remarks>
    /// This sphere has a configurable subdivision and radius and extent,
    /// So it can be extended to a cube, sphere, cylinder, or rounded cube.
    /// </remarks>
    public class Sphere4 : Modeler4
    {
        /// <summary> Subdivision level </summary>
        /// <remarks>
        /// Internally the subdivision level is multiplied by 2 so it's always even.
        /// Be advised that setting too much subdivision level could broken the output mesh.
        /// </remarks>
        [Range(1, 10)]
        public int subdivision = 2;

        /// <summary>
        /// Radius of the sphere
        /// </summary>
        public float radius = 0.5f;
        /// <summary>
        /// Extent of the cube
        /// </summary>
        public Vector4 extent;

        // Temporary values

        bool useedge;
        int subedge;
        int subdiv;
        float subinv;
        const float epsilon = 1e-3f;

        /// 
        [HideInDocs]
        public override void CreateModel(Buffer4 buffer)
        {
            subdiv = subdivision * 2; // Real sphere subdivision (it's always even)

            subinv = Math.PI / subdiv; // Angle advancement ratio

            subedge = subdivision; // Threshhold to signal extent cases

            useedge = Vector4.LengthSq(extent) > 1e-4f; // Do we need to add more verts for extents?

            for (int x = 0; x <= subdiv * 2; x++) // Loop until 2*pi
            {
                if (useedge && x % subedge == 0) // Every 90 deg
                {
                    LoopLatitude(buffer, x * subinv - epsilon);
                    LoopLatitude(buffer, x * subinv + epsilon);
                }
                else
                    LoopLatitude(buffer, x * subinv);
            }

            if (useedge) buffer.SequenceGrid(subdiv * 2 + 6, subdiv + 4, subdiv + 4);
            else buffer.SequenceGrid(subdiv * 2 + 1, subdiv + 1, subdiv + 1);
        }

        private void LoopLatitude(Buffer4 buffer, float x)
        {
            for (int y = 0; y <= subdiv; y++) // Loop until pi
            {
                if (useedge && y % subedge == 0) // Every 90 deg
                {
                    LoopAzimuth(buffer, x, y * subinv - epsilon);
                    LoopAzimuth(buffer, x, y * subinv + epsilon);
                }
                else
                    LoopAzimuth(buffer, x, y * subinv);
            }
        }

        private void LoopAzimuth(Buffer4 buffer, float x, float y)
        {
            for (int z = 0; z <= subdiv; z++) // Loop until pi
            {
                if (useedge && z % subedge == 0) // Every 90 deg
                {
                    LoopMethod(buffer, x, y, z * subinv - epsilon);
                    LoopMethod(buffer, x, y, z * subinv + epsilon);
                }
                else
                    LoopMethod(buffer, x, y, z * subinv);
            }
        }

        private void LoopMethod(Buffer4 buffer, float x, float y, float z)
        {
            var C = GetCoordinate(x, y, z);
            buffer.AddVertex(C * radius + extent * Vector4.Sign(C));
        }


        Vector4 GetCoordinate(float lon, float lat, float azm)
        {

            float clat = Math.Cos(lat), slat = Math.Sin(lat), clon = Math.Cos(lon),
                slon = Math.Sin(lon), cazm = Math.Cos(azm), sazm = Math.Sin(azm);

            // Spherical coordinate upgraded in 4D according to wiki
            return new Vector4(sazm * slat * slon, sazm * clat, sazm * slat * clon, cazm);
        }


    }
}
