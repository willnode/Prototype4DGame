namespace Engine4.Internal
{
    public partial class Buffer4Extension
    {

        /// <summary> Add new profiles to remaining indice </summary>
        public static void AddProfile(this Buffer4 buffer, VertexProfile v0, VertexProfile v1)
        {
            buffer.EnsureProfiles(2);
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v0;
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v1;
        }
        
        /// <summary> Add new profiles to remaining indice </summary>
        public static void AddProfile(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2)
        {
            buffer.EnsureProfiles(3);
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v0;
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v1;
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v2;
        }
        
        /// <summary> Add new profiles to remaining indice </summary>
        public static void AddProfile(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2, VertexProfile v3)
        {
            buffer.EnsureProfiles(4);
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v0;
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v1;
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v2;
            buffer.m_Profiles[buffer.m_ProfilesCount++] = v3;
        }

        /// <summary> Push new profiles to temporary queue </summary>
        public static void PushProfile(this Buffer4 buffer, VertexProfile v0)
        {
            buffer.m_ProfilesQueue.Enqueue(v0);
        }

        /// <summary>
        /// Add an artbitrary point. (profile)
        /// </summary>
        public static void AddPoint(this Buffer4 buffer, VertexProfile v0)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    buffer.AddProfile(v0);
                    break;
            }
        }

        /// <summary>
        /// Add an artbitrary point. (profile from temporary queue)
        /// </summary>
        public static void AddPoint(this Buffer4 buffer)
        {
            AddPoint(buffer, buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Add a segment. (profile)
        /// </summary>
        public static void AddSegment(this Buffer4 buffer, VertexProfile v0, VertexProfile v1)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    buffer.AddProfile(v0, v1);
                    break;
                case SimplexMode.Line:
                    buffer.AddProfile(v0, v1);
                    break;
            }
        }

        /// <summary>
        /// Add a segment. (profile from temporary queue)
        /// </summary>
        public static void AddSegment(this Buffer4 buffer)
        {
            AddSegment(buffer, buffer.m_ProfilesQueue.Dequeue(), buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Add a flat triangle. (profile)
        /// </summary>
        public static void AddTriangle(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    buffer.AddProfile(v0, v1, v2);
                    break;
                case SimplexMode.Line:
                    buffer.AddProfile(v0, v1);
                    buffer.AddProfile(v1, v2);
                    buffer.AddProfile(v2, v0);
                    break;
                case SimplexMode.Triangle:
                    buffer.AddProfile(v0, v1, v2);
                    break;
            }
        }

        /// <summary>
        /// Add a flat triangle. (profile from temporary queue)
        /// </summary>
        public static void AddTriangle(this Buffer4 buffer)
        {
            AddTriangle(buffer, buffer.m_ProfilesQueue.Dequeue(), buffer.m_ProfilesQueue.Dequeue(), buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Add a flat quad. (profile)
        /// </summary>
        public static void AddQuad(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2, VertexProfile v3)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point:
                    buffer.AddProfile(v0, v1, v2, v3);
                    break;
                case SimplexMode.Line:
                    buffer.AddProfile(v0, v1);
                    buffer.AddProfile(v1, v2);
                    buffer.AddProfile(v2, v3);
                    buffer.AddProfile(v3, v0);
                    break;
                case SimplexMode.Triangle:
                    buffer.AddProfile(v0, v1, v2);
                    buffer.AddProfile(v2, v3, v0);
                    break;
            }
        }

        /// <summary>
        /// Add a flat quad. (profile from temporary queue)
        /// </summary>
        public static void AddQuad(this Buffer4 buffer)
        {
            AddQuad(buffer, 
                buffer.m_ProfilesQueue.Dequeue(), 
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(), 
                buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Add a trimid (triangle pyramid). (profile)
        /// </summary>
        public static void AddTrimid(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2, VertexProfile v3)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point: // 4 vertexes
                    buffer.AddProfile(v0, v1, v2, v3);
                    break;

                case SimplexMode.Line: // 6 edges
                    buffer.AddProfile(v0, v1);
                    buffer.AddProfile(v0, v2);
                    buffer.AddProfile(v0, v3);
                    buffer.AddTriangle(v1, v2, v3);

                    break;
                case SimplexMode.Triangle: // 4 faces                    
                    buffer.AddTriangle(v0, v1, v2);
                    buffer.AddTriangle(v0, v2, v3);
                    buffer.AddTriangle(v0, v3, v1);
                    buffer.AddTriangle(v1, v2, v3);
                    break;

                case SimplexMode.Tetrahedron: // 1 cell
                    buffer.AddProfile(v0, v1, v2, v3);
                    break;

            }
        }

        /// <summary>
        /// Add a trimid. (profile from temporary queue)
        /// </summary>
        public static void AddTrimid(this Buffer4 buffer)
        {
            AddTrimid(buffer,
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Helper to add a pyramid from 5 existing verts index. (profile)
        /// </summary>
        public static void AddPyramid(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2, VertexProfile v3, VertexProfile v4)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point: // 5 vertexes
                    buffer.AddProfile(v0, v1, v2);
                    buffer.AddProfile(v3, v4);
                    break;

                case SimplexMode.Line: // 8 edges
                    buffer.AddProfile(v0, v1);
                    buffer.AddProfile(v0, v2);
                    buffer.AddProfile(v0, v3);
                    buffer.AddProfile(v0, v4);
                    buffer.AddQuad(v1, v2, v3, v4);
                    break;
                case SimplexMode.Triangle: // 6 faces (3 quads)
                    buffer.AddTriangle(v0, v1, v2);
                    buffer.AddTriangle(v0, v2, v3);
                    buffer.AddTriangle(v0, v3, v4);
                    buffer.AddTriangle(v0, v4, v1);
                    buffer.AddQuad(v1, v2, v3, v4);
                    break;

                case SimplexMode.Tetrahedron: // 2 cell
                    buffer.AddProfile(v0, v1, v2, v3);
                    buffer.AddProfile(v0, v1, v3, v4);
                    break;

            }
        }

        /// <summary>
        /// Helper to add a pyramid from 5 existing verts index. (profile from temporary queue)
        /// </summary>
        public static void AddPyramid(this Buffer4 buffer)
        {
            AddPyramid(buffer,
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Helper to add a triangular prism from 6 existing verts index. (profile)
        /// </summary>
        public static void AddPrism(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2, VertexProfile v3, VertexProfile v4, VertexProfile v5)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point: // 6 vertexes
                    buffer.AddProfile(v0, v1, v2);
                    buffer.AddProfile(v3, v4, v5);
                    break;

                case SimplexMode.Line: // 10 edges
                    buffer.AddTriangle(v0, v1, v2);
                    buffer.AddTriangle(v3, v4, v5);

                    buffer.AddProfile(v0, v3);
                    buffer.AddProfile(v1, v4);
                    buffer.AddProfile(v2, v5);
                    break;
                case SimplexMode.Triangle: // 5 faces
                    buffer.AddTriangle(v0, v1, v2);
                    buffer.AddTriangle(v3, v4, v5);

                    buffer.AddQuad(v0, v1, v4, v3);
                    buffer.AddQuad(v1, v2, v5, v4);
                    buffer.AddQuad(v2, v1, v3, v5);
                    break;

                case SimplexMode.Tetrahedron: // 3 cells
                    buffer.AddProfile(v0, v1, v2, v3);
                    buffer.AddProfile(v3, v4, v5, v2);
                    buffer.AddProfile(v1, v4, v3, v2);
                    break;

            }
        }

        /// <summary>
        /// Helper to add a triangular prism from 6 existing verts index. (profile from temporary queue)
        /// </summary>
        public static void AddPrism(this Buffer4 buffer)
        {
            AddPrism(buffer,
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue());
        }

        /// <summary>
        /// Helper to add a cube from 8 existing verts index. (profile)
        /// </summary>
        public static void AddCube(this Buffer4 buffer, VertexProfile v0, VertexProfile v1, VertexProfile v2, VertexProfile v3, VertexProfile v4, VertexProfile v5, VertexProfile v6, VertexProfile v7)
        {
            switch (buffer.simplex)
            {
                case SimplexMode.Point: // 8 vertices
                    buffer.AddProfile(v0, v1, v2, v3);
                    buffer.AddProfile(v4, v5, v6, v7);
                    break;

                case SimplexMode.Line: // 12 edges
                    buffer.AddQuad(v0, v1, v2, v3);
                    buffer.AddQuad(v4, v5, v6, v7);

                    buffer.AddProfile(v0, v4);
                    buffer.AddProfile(v1, v5);
                    buffer.AddProfile(v2, v6);
                    buffer.AddProfile(v3, v7);
                    break;

                case SimplexMode.Triangle: //  12 faces (6 quads)
                    // topdown
                    buffer.AddQuad(v0, v1, v2, v3);
                    buffer.AddQuad(v4, v5, v6, v7);
                    // leftright
                    buffer.AddQuad(v0, v1, v5, v4);
                    buffer.AddQuad(v2, v3, v7, v6);
                    // frontback
                    buffer.AddQuad(v1, v2, v6, v5);
                    buffer.AddQuad(v0, v3, v7, v4);
                    break;

                case SimplexMode.Tetrahedron: // 5 cells
                    buffer.AddProfile(v0, v1, v2, v5);
                    buffer.AddProfile(v0, v3, v2, v7);
                    buffer.AddProfile(v0, v4, v5, v7);
                    buffer.AddProfile(v2, v5, v7, v6);
                    buffer.AddProfile(v0, v2, v5, v7);
                    break;
            }
        }

        /// <summary>
        /// Helper to add a cube from 6 existing verts index. (profile from temporary queue)
        /// </summary>
        public static void AddCube(this Buffer4 buffer)
        {
            AddCube(buffer,
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue(),
                buffer.m_ProfilesQueue.Dequeue());
        }
    }
}
