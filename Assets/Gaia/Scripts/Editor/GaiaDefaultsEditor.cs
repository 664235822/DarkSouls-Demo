using UnityEngine;
using UnityEditor;

namespace Gaia {

    /// <summary>
    /// Editor for Gaia defaults
    /// </summary>
    [CustomEditor(typeof(GaiaDefaults))]
    public class GaiaDefaultsEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            //Get our resource
            GaiaDefaults defaults = (GaiaDefaults)target;

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.wordWrap = true;
            }

            //Create a nice text intro
            GUILayout.BeginVertical("Gaia Defaults", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("These defaults will be used by Gaia when creating new terrains, and new resources files.", m_wrapStyle);
            GUILayout.EndVertical();

            //Check for and fix any issues with the settings in the editor
            string defaultIssues = defaults.GetAndFixDefaults();
            if (!string.IsNullOrEmpty(defaultIssues))
            {
                Debug.LogWarning(defaultIssues);
            }

            DrawDefaultInspector();
        }
    }
}
