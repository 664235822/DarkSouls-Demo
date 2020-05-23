using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gaia
{
    [CustomEditor(typeof(TerrainHelper))]
    public class TerrainHelperEditor : Editor
    {
        GUIStyle m_boxStyle;

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            GUILayout.Space(10f);

            //Terraform section
            GUILayout.BeginVertical("Terrain Controller", m_boxStyle);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            var helper = Selection.activeGameObject.GetComponent<TerrainHelper>();

            if (GUILayout.Button("Flatten"))
            {
                TerrainHelper.Flatten();
            }

            if (GUILayout.Button("Smooth"))
            {
                helper.Smooth();
            }

            if (GUILayout.Button("Stitch"))
            {
                TerrainHelper.Stitch();
            }

            if (GUILayout.Button("Clear Trees"))
            {
                TerrainHelper.ClearTrees();
            }

            if (GUILayout.Button("Clear Details"))
            {
                TerrainHelper.ClearDetails();
            }


            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
        }
    }
}