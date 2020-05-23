using UnityEngine;
using UnityEditor;
using Gaia.Internal;
using GaiaCommon1;

namespace Gaia
{
    public class GaiaWelcome : CustomWelcome
    {
        private EditorUtils m_editorUtils;

        public override AppConfig AppConfig { get { return PWApp.CONF; } }

        public override bool PositionChecked { get; set; }

        public GaiaWelcome()
        {
        }

        ~GaiaWelcome()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }
        }

        public override void WelcomeGUI()
        {
            if (m_editorUtils == null)
            {
                m_editorUtils = PWApp.GetEditorUtils(this);
            }
            m_editorUtils.Initialize();
            m_editorUtils.GUIHeader();

            m_editorUtils.Title("Welcome title");

            EditorGUILayout.Space();
            m_editorUtils.Text("Welcome message");

            GUILayout.Space(10f);
            if (m_editorUtils.ButtonCentered("Open Quick Start Button", GUILayout.Width(250f)))
            {
                Application.OpenURL("http://www.procedural-worlds.com/gaia/?section=tutorials");
            }
        }
    }
}
