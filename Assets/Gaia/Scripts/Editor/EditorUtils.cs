using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Gaia.Internal;

namespace Gaia
{
    /// <summary>
    /// Handy editor specific utils
    /// </summary>
    public class EditorUtilsOLD
    {
        #if UNITY_EDITOR
        private bool m_initialized = false;
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private GUIStyle m_titleStyle;
        private GUIStyle m_headingStyle;
        private GUIStyle m_bodyStyle;
        private GUIStyle m_linkStyle;
        private bool m_positionChecked = false;

        /// <summary>
        /// Access to position checked method
        /// </summary>
        public bool PositionChecked
        {
            get { return m_positionChecked; }
        }

        /// <summary>
        /// Initialize editor styles
        /// </summary>
        public void Initialize()
        {
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
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            if (m_bodyStyle == null)
            {
                m_bodyStyle = new GUIStyle(GUI.skin.label);
                m_bodyStyle.fontStyle = FontStyle.Normal;
                m_bodyStyle.wordWrap = true;
            }

            if (m_titleStyle == null)
            {
                m_titleStyle = new GUIStyle(m_bodyStyle);
                m_titleStyle.fontStyle = FontStyle.Bold;
                m_titleStyle.fontSize = 20;
            }

            if (m_headingStyle == null)
            {
                m_headingStyle = new GUIStyle(m_bodyStyle);
                m_headingStyle.fontStyle = FontStyle.Bold;
            }

            if (m_linkStyle == null)
            {
                m_linkStyle = new GUIStyle(m_bodyStyle);
                m_linkStyle.wordWrap = false;
                m_linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
                m_linkStyle.stretchWidth = false;
            }

            m_initialized = true;
        }

        /// <summary>
        /// Check and caclulate window adjustment for editor windows relative to scene & game view
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maximized"></param>
        /// <returns></returns>
        public Rect CheckPosition(Rect position, bool maximized)
        {
            if (!m_positionChecked)
            {
                m_positionChecked = true;
                if (!maximized)
                {
                    //Get scene position
                    Rect scenePosition = new Rect(0f, 0f, 800f, 600f);
                    if (SceneView.lastActiveSceneView != null)
                    {
                        scenePosition = SceneView.lastActiveSceneView.position;
                    }
                    //Check our position
                    if (position.x < scenePosition.xMin || position.x > scenePosition.xMax)
                    {
                        position.x = scenePosition.xMin + (((scenePosition.xMax - scenePosition.xMin) / 2f) - (position.width / 2f));
                    }
                    if (position.y < scenePosition.yMin || position.y > scenePosition.yMax)
                    {
                        position.y = scenePosition.yMin + 100f;
                    }
                }
            }
            return position;
        }

        /// <summary>
        /// Draw the intro
        /// </summary>
        /// <param name="name">Name of the intro</param>
        /// <param name="description">Description</param>
        /// <param name="url">Make description clickable if supplied</param>
        public void DrawIntro(string name, string description = "", string url = "")
        {
            if (!m_initialized)
            {
                Initialize();
            }

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            //Text intro
            GUILayout.BeginVertical(string.Format("{0} ({1})", name, PWApp.CONF.Version), m_boxStyle);
            GUILayout.Label("");
            if (!string.IsNullOrEmpty(description))
            {
                if (string.IsNullOrEmpty(url))
                {
                    DrawBody(description);
                }
                else
                {
                    if (DrawClickableBody(description))
                    {
                        Application.OpenURL(url);
                    }
                }
            }
            GUILayout.EndVertical();
        }


        /// <summary>
        /// Display an image - the image must be of type editor & legacy gui to display
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        public void DrawImage(Texture2D image, float width, float height)
        {
            GUILayout.Label(image, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Display text in title style
        /// </summary>
        /// <param name="text">Text to display</param>
        public void DrawTitle(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (!m_initialized)
            {
                Initialize();
            }
            GUILayout.Label(text, m_titleStyle);
        }

        /// <summary>
        /// Display text in header style
        /// </summary>
        /// <param name="text">Text to display</param>
        public void DrawHeader(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (!m_initialized)
            {
                Initialize();
            }
            GUILayout.Label(text, m_headingStyle);
        }

        /// <summary>
        /// Draw header text as clickable object
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">Display options</param>
        /// <returns>True if it was clicked</returns>
        public bool DrawClickableHeader(string text, params GUILayoutOption[] options)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            return DrawClickableHeader(GetContent(text), options);
        }

        /// <summary>
        /// Draw header text as clickable object
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">Display options</param>
        /// <returns>True if it was clicked</returns>
        public bool DrawClickableHeader(GUIContent text, params GUILayoutOption[] options)
        {
            if (text == null)
            {
                return false;
            }
            if (!m_initialized)
            {
                Initialize();
            }
            var position = GUILayoutUtility.GetRect(text, m_headingStyle, options);
            Handles.BeginGUI();
            Handles.color = m_headingStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, text, m_headingStyle);
        }

        /// <summary>
        /// Display text in body style
        /// </summary>
        /// <param name="text"></param>
        public void DrawBody(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            GUILayout.Label(text, m_bodyStyle);
        }

        /// <summary>
        /// Draw clickable body text
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <returns>True if it was clicked</returns>
        public bool DrawClickableBody(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            return DrawLinkBody(GetContent(text));
        }

        /// <summary>
        /// Draw clickable link body
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public bool DrawLinkBody(GUIContent text, params GUILayoutOption[] options)
        {
            if (text == null)
            {
                return false;
            }
            if (!m_initialized)
            {
                Initialize();
            }
            var position = GUILayoutUtility.GetRect(text, m_bodyStyle, options);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, text, m_bodyStyle);
        }

        /// <summary>
        /// Handy wrapper
        /// </summary>
        /// <param name="label"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool DrawLink(string label, params GUILayoutOption[] options)
        {
            if (string.IsNullOrEmpty(label))
            {
                return false;
            }
            return DrawLinkLabel(GetContent(label), options);
        }

        /// <summary>
        /// Draw link label
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public bool DrawLinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            if (label == null)
            {
                return false;
            }
            if (!m_initialized)
            {
                Initialize();
            }
            var position = GUILayoutUtility.GetRect(label, m_linkStyle, options);
            Handles.BeginGUI();
            Handles.color = m_linkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, m_linkStyle);
        }

        /// <summary>
        /// Draw a line in the space provided by the label
        /// </summary>
        /// <param name="label">Label - used for spacing / volume</param>
        /// <param name="options">Parameters - eg height / width</param>
        public void DrawBodyLine(GUIContent label, params GUILayoutOption[] options)
        {
            if (label == null)
            {
                return;
            }
            if (!m_initialized)
            {
                Initialize();
            }
            var position = GUILayoutUtility.GetRect(label, m_bodyStyle, options);
            Handles.BeginGUI();
            Handles.color = m_bodyStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.y), new Vector3(position.xMax, position.y));
            Handles.color = Color.white;
            Handles.EndGUI();
        }

        /// <summary>
        /// Draw a button that takes editor indentation into account
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <returns>True if clicked</returns>
        public bool DrawButton(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            return DrawButton(GetContent(text));
        }

        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="content">Content to display</param>
        /// <returns>True is clicked</returns>
        public bool DrawButton(GUIContent content)
        {
            TextAnchor oldalignment = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            Rect btnR = EditorGUILayout.BeginHorizontal();
            btnR.xMin += (EditorGUI.indentLevel * 18f);
            btnR.height += 20f;
            btnR.width -= 4f;
            bool result = GUI.Button(btnR, content);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(22);
            GUI.skin.button.alignment = oldalignment;
            return result;
        }

        /// <summary>
        /// Get a content label
        /// </summary>
        /// <param name="name">Name of the label</param>
        /// <returns>Label</returns>
        public GUIContent GetContent(string name)
        {
            return new GUIContent(name);
        }

        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name">Name of thing</param>
        /// <param name="tooltips">Tooltips of things</param>
        /// <returns>Content plus tool tip if it exists</returns>
        public GUIContent GetContent(string name, Dictionary<string, string> tooltips)
        {
            string tooltip = "";
            if (tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }

        /// <summary>
        /// Get the text of the content indexed by name, or the name if not found
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <param name="tooltips">Toolips dictionary</param>
        /// <returns></returns>
        public string GetContentText(string name, Dictionary<string, string> tooltips)
        {
            string tooltip = "";
            if (tooltips.TryGetValue(name, out tooltip))
            {
                return tooltip;
            }
            else
            {
                return name;
            }
        }
        #endif
    }
}
