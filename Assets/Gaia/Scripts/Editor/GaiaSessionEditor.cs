using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Editor for sessions
    /// </summary>
    [CustomEditor(typeof(GaiaSession))]
    public class GaiaSessionEditor : Editor
    {

        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        GUIStyle m_descWrapStyle;
        GaiaSession m_session;
        private bool m_showTooltips = true;

        void OnEnable()
        {
            //Get the settings and update tooltips
            GaiaSettings settings = Gaia.GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                m_showTooltips = settings.m_showTooltips;
            }

            //Get our resource
            m_session = (GaiaSession)target;
        }

        public override void OnInspectorGUI()
        {
            //Get our resource
            m_session = (GaiaSession)target;

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

            //Set up the description wrap style
            if (m_descWrapStyle == null)
            {
                m_descWrapStyle = new GUIStyle(GUI.skin.textArea);
                m_descWrapStyle.wordWrap = true;
            }

            //Create a nice text intro
            GUILayout.BeginVertical("Gaia Session", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Contains the data used to backup, share and play back sessions. Use the session manager to view, edit or play back sessions.", m_wrapStyle);
            GUILayout.Space(4);
            GUILayout.EndVertical();

            //Make some space
            GUILayout.Space(4);

            //Wrap it up in a box
            GUILayout.BeginVertical(m_boxStyle);
            GUILayout.BeginVertical("Summary:", m_boxStyle);
            GUILayout.Space(20);

            //Display the basic details
            EditorGUILayout.LabelField("Name", m_session.m_name);
            EditorGUILayout.LabelField("Description", m_session.m_description, m_wrapStyle);
            EditorGUILayout.LabelField("Created", m_session.m_dateCreated);
            EditorGUILayout.LabelField("Dimensions", string.Format("w{0} d{1} h{2} meters", m_session.m_terrainWidth, m_session.m_terrainDepth, m_session.m_terrainHeight));
            EditorGUILayout.LabelField("Sea Level", string.Format("{0} meters", m_session.m_seaLevel));
            EditorGUILayout.LabelField("Locked", m_session.m_isLocked.ToString());

            Texture2D previewImage = m_session.GetPreviewImage();
            if (previewImage != null)
            {
                //Get aspect ratio and available space and display the image
                float width = Screen.width - 43f;
                float height = previewImage.height * (width / previewImage.width);
                GUILayout.Label(previewImage, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height));
            }

            GUILayout.EndVertical();

            //Iterate through the operations
            GUILayout.BeginVertical("Operations:", m_boxStyle);
            GUILayout.Space(20);

            if (m_session.m_operations.Count == 0)
            {
                GUILayout.Space(5);
                GUILayout.Label("No operations yet...");
                GUILayout.Space(5);
            }
            else
            {
                GaiaOperation op;
                EditorGUI.indentLevel++;
                for (int opIdx = 0; opIdx < m_session.m_operations.Count; opIdx++)
                {
                    op = m_session.m_operations[opIdx];

                    if (op.m_isActive)
                    {
                        op.m_isFoldedOut = EditorGUILayout.Foldout(op.m_isFoldedOut, op.m_description, true);
                    }
                    else
                    {
                        op.m_isFoldedOut = EditorGUILayout.Foldout(op.m_isFoldedOut, op.m_description + " [inactive]", true);
                    }

                    if (op.m_isFoldedOut)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("Description", op.m_description, m_wrapStyle);
                        EditorGUILayout.LabelField("Created", op.m_operationDateTime);
                        EditorGUILayout.LabelField("Active", op.m_isActive.ToString());
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }


        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_showTooltips && m_tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }

        /// <summary>
        /// The tooltips
        /// </summary>
        static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            { "Sea Level", "The sea level the session will be rendered at. Changing this will also change the resource files when it is played." },
            { "Locked", "When activated then this stamp is locked and no further changes can be made." },
            { "Delete", "Delete the step." },
            { "Apply", "Apply the step to the relevant object, but don't execute it. Great for seeing how something was configured." },
            { "Play", "Apply the step and play it in the scene." },
        
            { "Flatten Terrain", "Flatten all terrains." },
            { "Smooth Terrain", "Smooth all terrains." },
            { "Clear Trees", "Clear trees on all terrains and reset all tree spawners." },
            { "Clear Details", "Clear details on all terrains." },

            { "Terrain Helper", "Show the terrain helper controls." },
            { "Play Session", "Play the session from end to end." },
            { "Export Resources", "Export the embedded session resources to the Assest\\Gaia Sessions\\SessionName directory." },
            { "Session", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        };
    }
}