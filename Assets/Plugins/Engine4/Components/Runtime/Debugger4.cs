using UnityEngine;
using Engine4.Internal;
using System.Collections.Generic;

namespace Engine4.Rendering
{
    /// <summary>
    /// Useful debugging feature for diagnosing modeler
    /// </summary>
    public class Debugger4 : MonoBehaviour4
    {
        /// <summary>
        /// What need to to be shown for indice?
        /// </summary>
        public enum IndiceGizmoShow {
            /// <summary> None. </summary>
            None,
            /// <summary> Index. </summary>
            Index,
            /// <summary> Color. </summary>
            Color,
            /// <summary> UV. </summary>
            UV,
            /// <summary> UV2. </summary>
            UV2,
            /// <summary> UV3. </summary>
            UV3
        }
        /// <summary>  Minimum filter  </summary>
        [Range(0, 1)]
        public float min;
        /// <summary> Maximum filter </summary>
        [Range(0, 1)]
        public float max = 1;
#if UNITY_EDITOR


        /// <summary> Show vertex gizmo. </summary>
        public bool vertexGizmo;
        /// <summary> Show vertex label. </summary>
        public bool vertexLabels;

        /// <summary> Show kind of indice gizmo </summary>
        public IndiceGizmoShow indiceGizmo;

        /// <summary> Show indice label </summary>
        public bool indiceLabel;

        Vector3 GetViewNormal(Vector3 p)
        {
            var cam = Runtime.GetCurrentCamera();
            return cam ? (cam.position - p).normalized : Vector3.up;
        }

        Plane GetViewPlane()
        {
            var cam = Runtime.GetCurrentCamera();
            return cam ? new Plane(cam.forward, cam.position) : new Plane();
        }

        string FormattedStr(string i, Vector3 p)
        {
            int key = Utility.VectorToKey(p), iter;
            locations[key] = (iter = locations.GetValue(key, 0)) + 1;
            return new string('\n', iter) + i;
        }

        Dictionary<int, int> locations = new Dictionary<int, int>();

        void OnDrawGizmosSelected()
        {
            if (vertexGizmo)
                VertexGizmo();
            if (indiceGizmo != IndiceGizmoShow.None)
                TriangleGizmo();
        }

        void VertexGizmo()
        {

            locations.Clear();
            var buff = renderer4._buffer;
            var project = viewer4.projector;
            var viewmodel = viewer4.worldToViewerMatrix * transform4.localToWorldMatrix;
            var vertex = buff.m_Vertices; var v4c = buff.m_VerticesCount;
            var sz = UnityEditor.HandleUtility.GetHandleSize(project.Project((viewmodel * Vector4.zero))) * 0.05f;
            //var plne = new Plane(GetViewNormal(transform.position), transform.position);
            //            var cull = GetViewPlane();

            for (int i = 0; i < v4c; i++)
            {
                var f = i / (float)v4c;
                if (f < min || f > max)
                    continue;
                var p = project.Project(viewmodel * vertex[i]);

                //if (vertexFacing && !plne.GetSide(p) || !cull.GetSide(p))
                //    continue;

                Gizmos.color = Utility.Hue(f);
                Gizmos.DrawCube(p, Vector3.one * sz);
                if (vertexLabels)
                    UnityEditor.Handles.Label(p, FormattedStr(i.ToString(), p));
            }
        }

        // -------------------------------------------------------------------


        void TriangleGizmo()
        {
            locations.Clear();
            var buff = renderer4._buffer;
            var project = viewer4.projector;
            var viewmodel = viewer4.worldToViewerMatrix * transform4.localToWorldMatrix;
            var vertex = buff.m_Vertices;
            var profil = buff.m_Profiles;
            var indice = buff.m_Indices; var i4c = buff.m_IndicesCount;
            var sz = Vector3.one * UnityEditor.HandleUtility.GetHandleSize(project.Project((viewmodel * Vector4.zero))) * 0.05f;
            Vector3 p; int i;
            switch (renderer4._buffer.simplex)
            {
                case SimplexMode.Point:
                    for (i = 0; i < i4c; i++)
                    {
                        var f = i / (float)i4c; if (f < min || f > max) continue;
                        if (indiceGizmo == IndiceGizmoShow.Index) Gizmos.color = Utility.Hue(f);
                        else SetColor(profil[i]);
                        p = project.Project(viewmodel * vertex[indice[i]]);
                        Gizmos.DrawCube(p, sz);
                    }
                    break;
                case SimplexMode.Line:
                    for (i = 0; i < i4c; i += 2)
                    {
                        var f = i / (float)i4c; if (f < min || f > max) continue;
                        var c = CenterOf(vertex[indice[i]], vertex[indice[i + 1]]);
                        if (indiceGizmo == IndiceGizmoShow.Index) Gizmos.color = Utility.Hue(f);
                        for (int j = 0; j < 2; j++)
                        {
                            SetColor(profil[i + j]);
                            Gizmos.DrawCube(p = project.Project(viewmodel * SlightOf(vertex[indice[i + j]], c)), sz);
                            if (indiceLabel) UnityEditor.Handles.Label(p, FormattedStr(profil[i + j].uv.ToString("0.0"), p));
                        }
                    }
                    break;
                case SimplexMode.Triangle:
                    for (i = 0; i < i4c; i += 3)
                    {
                        var f = i / (float)i4c; if (f < min || f > max) continue;
                        var c = CenterOf(vertex[indice[i]], vertex[indice[i + 1]], vertex[indice[i + 2]]);
                        if (indiceGizmo == IndiceGizmoShow.Index) Gizmos.color = Utility.Hue(f);
                        for (int j = 0; j < 3; j++)
                        {
                            SetColor(profil[i + j]);
                            Gizmos.DrawCube(p = project.Project(viewmodel * SlightOf(vertex[indice[i + j]], c)), sz);
                            if (indiceLabel) UnityEditor.Handles.Label(p, FormattedStr(profil[i + j].uv.ToString("0.0"), p));
                        }
                    }
                    break;
                case SimplexMode.Tetrahedron:
                    for (i = 0; i < i4c; i += 4)
                    {
                        var f = i / (float)i4c; if (f < min || f > max) continue;
                        var c = CenterOf(vertex[indice[i]], vertex[indice[i + 1]], vertex[indice[i + 2]], vertex[indice[i + 3]]);
                        if (indiceGizmo == IndiceGizmoShow.Index) Gizmos.color = Utility.Hue(f);
                        for (int j = 0; j < 4; j++)
                        {
                            SetColor(profil[i + j]);
                            Gizmos.DrawCube(p = project.Project(viewmodel * SlightOf(vertex[indice[i + j]], c)), sz);
                            if (indiceLabel) UnityEditor.Handles.Label(p, FormattedStr(profil[i + j].uv.ToString("0.0"), p));
                        }
                    }
                    break;
            }

        }

        Vector4 CenterOf(Vector4 a, Vector4 b)
        {
            return (a + b) / 2f;
        }

        Vector4 CenterOf(Vector4 a, Vector4 b, Vector4 c)
        {
            return (a + b + c) / 3f;
        }

        Vector4 CenterOf(Vector4 a, Vector4 b, Vector4 c, Vector4 d)
        {
            return (a + b + c + d) / 4f;
        }


        Vector4 SlightOf(Vector4 v, Vector4 center)
        {
            return v;// Vector4.Lerp(v, center, 0.0f);
        }

        void SetColor(VertexProfile p)
        {
            switch (indiceGizmo)
            {
                case IndiceGizmoShow.Color: Gizmos.color = p.color; break;
                case IndiceGizmoShow.UV: Gizmos.color = SyncA(p.uv); break;
                case IndiceGizmoShow.UV2: Gizmos.color = SyncA(p.uv2); break;
                case IndiceGizmoShow.UV3: Gizmos.color = SyncA(p.uv3); break;
                default: return;
            }
        }

        Color SyncA(Vector4 uv)
        {
            return new Color(uv.x, uv.y, uv.z, 1);
        }

        [ContextMenu("Dump mesh to Clipboard (CSV)")]
        void DumpV3()
        {
            GUIUtility.systemCopyBuffer = Runtime.Dump(GetComponent<MeshFilter>().sharedMesh);
        }
#endif

    }

}