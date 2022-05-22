using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.Rendering;

namespace Editor.Tools
{
// By passing `typeof(MeshFilter)` as the second argument, we register VertexTool as a CustomEditor tool to be presented
// when the current selection contains a MeshFilter component.
    [EditorTool("Show Vertices", typeof(MeshFilter))]
    class StationEditorTool : EditorTool
    {
        struct TransformAndPositions
        {
            public Transform transform;
            public Vector3[] positions;
        }

        IEnumerable<TransformAndPositions> m_Vertices;
        GUIContent m_ToolbarIcon;

        public override GUIContent toolbarIcon
        {
            get
            {
                if (m_ToolbarIcon == null)
                    m_ToolbarIcon = new GUIContent(
                        AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/gear.png"),
                        "Vertex Visualization Tool");
                return m_ToolbarIcon;
            }
        }

        void OnEnable()
        {
            ToolManager.activeToolChanged += ActiveToolDidChange;
        }

        void OnDisable()
        {
            ToolManager.activeToolChanged -= ActiveToolDidChange;
        }

        void ActiveToolDidChange()
        {
            if (!ToolManager.IsActiveTool(this))
                return;

            m_Vertices = targets.Select(x =>
            {
                return new TransformAndPositions()
                {
                    transform = ((MeshFilter)x).transform,
                    positions = ((MeshFilter)x).sharedMesh.vertices
                };
            }).ToArray();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            var evt = Event.current;

            if (evt.type == EventType.Repaint)
            {
                var zTest = Handles.zTest;
                Handles.zTest = CompareFunction.LessEqual;

                foreach (var entry in m_Vertices)
                {
                    foreach (var vertex in entry.positions)
                    {
                        var world = entry.transform.TransformPoint(vertex);
                        Handles.DotHandleCap(0, world, Quaternion.identity, HandleUtility.GetHandleSize(world) * .05f, evt.type);
                    }
                }

                Handles.zTest = zTest;
            }
        }
    }
}