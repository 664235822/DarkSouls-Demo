// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GaiaCommon1
{
    public class PWWelcomeScreen : EditorWindow, IPWEditor
    {
        public List<AppConfig> DefaultWelcomeList
        {
            set
            {
                m_defaultWelcomeList = value;
                m_defaultTabCount = 0;

                for (int i = 0; i < m_defaultWelcomeList.Count; i++)
                {
                    EditorPrefs.SetInt(m_defaultWelcomeList[i].Name + "WelcomeLastShown", Utils.GetFrapoch());
                    m_prodNamesForDefaultWelcome = (i == 0) ? m_defaultWelcomeList[i].Name : ", " + m_defaultWelcomeList[i].Name;
                    m_defaultTabCount = 1;
                }
            }
        }
        public List<CustomWelcome> CustomWelcomeList
        {
            set
            {
                m_customWelcomeList = value;
                for (int i = 0; i < m_customWelcomeList.Count; i++)
                {
                    EditorPrefs.SetInt(m_customWelcomeList[i].AppConfig.Name + "WelcomeLastShown", Utils.GetFrapoch());
                    m_customWelcomeList[i].repaintDelegate -= Repaint;
                    m_customWelcomeList[i].repaintDelegate += Repaint;
                }
            }
        }

        public bool PositionChecked { get; set; }

        [SerializeField] private Vector2 m_windowScrollPos = Vector2.right;

        private EditorUtils m_editorUtils;
        private float m_openingTime = -1f;
        private bool showAtStartup = true;
        private List<AppConfig> m_defaultWelcomeList = new List<AppConfig>();
        private List<CustomWelcome> m_customWelcomeList = new List<CustomWelcome>();
        private string m_prodNamesForDefaultWelcome;
        private int m_defaultTabCount;

        // Our products buttons
        private Texture2D m_prodImgASo;
        private Texture2D m_prodImgASk;
        private Texture2D m_prodImgCTS;
        private Texture2D m_prodImgGaia;
        private Texture2D m_prodImgGeNa;
        private Texture2D m_prodImgPegasus;
        private Texture2D m_prodImgPP;
        private Texture2D m_prodImgSECTR;
        private float m_maxProdImgWidth;

        // and links
        private const string LINK_ASO = "https://assetstore.unity.com/packages/tools/audio/ambient-sounds-interactive-soundscapes-142132";
        private const string LINK_ASK = "https://assetstore.unity.com/packages/tools/particles-effects/ambient-skies-skies-post-fx-lighting-145817";
        private const string LINK_CTS = "https://assetstore.unity.com/packages/tools/terrain/cts-2019-complete-terrain-shader-140806";
        private const string LINK_GAIA = "https://assetstore.unity.com/packages/tools/terrain/gaia-terrain-scene-generator-42618";
        private const string LINK_GENA = "https://assetstore.unity.com/packages/tools/terrain/gena-2-terrain-scene-spawner-127636";
        private const string LINK_PEGASUS = "https://assetstore.unity.com/packages/tools/animation/pegasus-65397";
        private const string LINK_PP = "https://assetstore.unity.com/packages/tools/terrain/path-painter-127506";
        private const string LINK_SECTR = "https://assetstore.unity.com/packages/tools/terrain/sectr-complete-2019-144433";


        PWWelcomeScreen()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        #region Main delegates

        private void OnEnable()
        {
            // Too expand height to 3 rows. Will allow a smaller minSize for ancient PCs in OnEditorUpdate().
            minSize = new Vector2(980, 920);

            if (m_editorUtils == null)
            {
                m_editorUtils = new EditorUtils(PWConst.COMMON_APP_CONF, this);
            }

            LoadImage("AmbientSkies", ref m_prodImgASk);
            LoadImage("AmbientSounds", ref m_prodImgASo);
            LoadImage("CTS", ref m_prodImgCTS);
            LoadImage("Gaia", ref m_prodImgGaia);
            LoadImage("GeNa", ref m_prodImgGeNa);
            LoadImage("Pegasus", ref m_prodImgPegasus);
            LoadImage("PathPainter", ref m_prodImgPP);
            LoadImage("SECTR", ref m_prodImgSECTR);
        }

        private void LoadImage(string id, ref Texture2D targetTexture)
        {
            if (targetTexture == null)
            {
                targetTexture = Resources.Load(id + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                m_maxProdImgWidth = targetTexture.width > m_maxProdImgWidth ? targetTexture.width : m_maxProdImgWidth;
            }
        }

        private void OnDestroy()
        {
            if (m_editorUtils != null)
            {
                m_editorUtils.Dispose();
            }

            for (int i = 0; i < m_defaultWelcomeList.Count; i++)
            {
                SetShowPrefForApp(m_defaultWelcomeList[i]);
            }

            for (int i = 0; i < m_customWelcomeList.Count; i++)
            {
                SetShowPrefForApp(m_customWelcomeList[i].AppConfig);
                m_customWelcomeList[i].repaintDelegate -= Repaint;
            }
        }

        /// <summary>
        /// Need the delay otherwise the calling window can cover this
        /// </summary>
        private void OnEditorUpdate()
        {
            if (m_openingTime < 0f)
            {
                m_openingTime = Time.realtimeSinceStartup;
                return;
            }

            if (Time.realtimeSinceStartup - m_openingTime > 1f)
            {
                Focus();
                // Allow smaller height for aciant PCs.
                minSize = new Vector2(980, 675);
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        #endregion

        #region Unity GUI
        private void InitGUI()
        {
        }

        protected void OnGUI()
        {
            m_editorUtils.Initialize(); // Do not remove this!
            m_windowScrollPos = EditorGUILayout.BeginScrollView(m_windowScrollPos);
            {
                Welcome();

                GUILayout.Space(6f);
                OurProducts();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                m_editorUtils.LeftCheckbox("Show welcome again checkbox", ref showAtStartup);

                GUILayout.FlexibleSpace();
                if (m_editorUtils.Button("Close button", GUILayout.Width(150f)))
                {
                    Close();
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            InitGUI();
        }
        #endregion

        #region UI Sections

        private void Welcome()
        {
            // This is really just a hack
            // Not yet implemented!
            if (m_customWelcomeList.Count + m_defaultTabCount > 1)
            {
                //m_editorUtils.Tabs();
            }
            else
            {
                if (m_customWelcomeList.Count == 1)
                {
                    m_customWelcomeList[0].WelcomeGUI();
                }
                else
                {
                    DefaultWelcome();
                }
            }
        }

        private void DefaultWelcome()
        {
            if (m_defaultWelcomeList.Count < 1)
            {
                return;
            }
            else if (m_defaultWelcomeList.Count == 1)
            {
                m_editorUtils.GUIHeader(m_defaultWelcomeList[0]);
            }
            else
            {
                m_editorUtils.GUIHeader("Procedural Worlds");
            }

            m_editorUtils.TitleNonLocalized(m_editorUtils.GetTextValue("Welcome title") + m_prodNamesForDefaultWelcome + "!");

            EditorGUILayout.Space();
            m_editorUtils.Text("Welcome message");

            GUILayout.Space(15f);
            if (m_editorUtils.ButtonCentered("Recommend Tutorials", GUILayout.Width(250f)))
            {
                Application.OpenURL(PWConst.COMMON_APP_CONF.TutorialsLink);
            }
        }

        private void OurProducts()
        {
            // Later we can get this from the website
            m_editorUtils.Heading("Our Products");

            float spacing = 10f;
            float descriptionHeight = 45f;

            GUILayout.Space(spacing);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    // Ambient Skies
                    if (m_editorUtils.ClickableImage(m_prodImgASk))
                    {
                        Application.OpenURL(LINK_ASK);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("ASkiesDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));

                    GUILayout.Space(spacing);

                    // Gaia
                    if (m_editorUtils.ClickableImage(m_prodImgGaia))
                    {
                        Application.OpenURL(LINK_GAIA);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("GaiaDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));

                    GUILayout.Space(spacing);

                    // Pegasus
                    if (m_editorUtils.ClickableImage(m_prodImgPegasus))
                    {
                        Application.OpenURL(LINK_PEGASUS);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("PegasusDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));
                }
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    // Ambient Sounds
                    if (m_editorUtils.ClickableImage(m_prodImgASo))
                    {
                        Application.OpenURL(LINK_ASO);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("ASoDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));

                    GUILayout.Space(spacing);

                    // GeNa
                    if (m_editorUtils.ClickableImage(m_prodImgGeNa))
                    {
                        Application.OpenURL(LINK_GENA);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("GeNaDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));

                    GUILayout.Space(spacing);

                    // SECTR
                    if (m_editorUtils.ClickableImage(m_prodImgSECTR))
                    {
                        Application.OpenURL(LINK_SECTR);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("SECTRDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));
                }
                GUILayout.EndVertical();

                GUILayout.Space(spacing);
                GUILayout.BeginVertical();
                {
                    // CTS
                    if (m_editorUtils.ClickableImage(m_prodImgCTS))
                    {
                        Application.OpenURL(LINK_CTS);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("CTSDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));

                    GUILayout.Space(spacing);

                    // PP
                    if (m_editorUtils.ClickableImage(m_prodImgPP))
                    {
                        Application.OpenURL(LINK_PP);
                    }
                    m_editorUtils.ClickableImgDescriptionBold("PPDescription", GUILayout.Width(m_maxProdImgWidth), GUILayout.Height(descriptionHeight));
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Helper methods

        private void SetShowPrefForApp(AppConfig appConfig)
        {
            EditorPrefs.SetBool("Show" + appConfig.Name + "Welcome", showAtStartup);
        }

        #endregion
    }
}
