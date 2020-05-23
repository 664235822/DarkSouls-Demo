// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using GaiaCommon1.Localization;

namespace GaiaCommon1
{
    /// <summary>
    /// Handy editor utils
    /// </summary>
    public partial class EditorUtils : IDisposable
    {
        #region Protected Data

        protected static readonly HashSet<string> ACCEPTED_BRUSH_EXTENSIONS = new HashSet<string>() { ".png", ".jpg" };

        protected AppConfig m_appConfig;
        protected IPWEditor m_parentEditor;
        protected string m_className;
        protected LanguagePack m_langPack;
        protected PWNews m_news;

        protected IDictionary<Action<bool>, bool[]> m_panelStatus = new Dictionary<Action<bool>, bool[]>();

        protected Texture2D[] m_BrushTextures;
        protected List<Texture2D> m_customBrushes;

        //Used to extract links from help text
        private const string LINK_REGEX_PATTERN = "([\\w\\W]*?)(<a href\\=\"([\\w\\W]+?)\">([\\w\\W]*?)<\\/a>\r{0,1}\n{0,1}|\\Z)";

        #endregion

        #region Public Data Access

        /// <summary>
        /// Is the localization data load complete
        /// </summary>
        public bool IsLocalizationReady { get; protected set; }

        /// <summary>
        /// Invoked when the localization data changes
        /// </summary>
        public Action OnLocalizationUpdate;

        /// <summary>
        /// Styles
        /// </summary>
        public CommonStyles Styles { get; protected set; }

        #endregion

        #region Constructors, Destructor, Disposal

        /// <summary>
        /// Create an editor utils object that can be used for common Editor stuff - DO make sure to Dispose() the instance.
        /// </summary>
        /// <param name="appConfig">Config of the App.</param>
        /// <param name="editorObj">The class that uses the utils. Just pass in "this".</param>
        /// <param name="customUpdateMethod">(Optional) The method to be called when the GUI needs to be updated. (Repaint will always be called.)</param>
        public EditorUtils(AppConfig appConfig, IPWEditor editorObj, System.Action customUpdateMethod = null)
        {
            m_appConfig = appConfig;
            if (m_appConfig != null)
            {
                Culture.AddToLanguageSet(m_appConfig.AvailableLanguages);
            }
            m_parentEditor = editorObj;

            // Initialize localization
            InitLocalization();

            OnLocalizationUpdate = null;
            if (customUpdateMethod != null)
            {
                OnLocalizationUpdate = customUpdateMethod;
            }
            OnLocalizationUpdate -= editorObj.Repaint;
            OnLocalizationUpdate += editorObj.Repaint;

            // Initialize news - Don't see a reason to allow AppConfig = null; Open a convo on Discord if you have a reason.
            //m_news = new PWNews(m_appConfig == null ? "" : m_appConfig.NewsURL);
            m_news = new PWNews(m_appConfig);
            if (m_news.ExistOnDisc)
            {
                // Load
                m_news = m_news.Load();
            }
            m_news.Update();

            // Load brush icons
            if (m_BrushTextures == null)
            {
                LoadBrushIcons();
            }
        }

        /// <summary>
        /// Initialize localization system
        /// </summary>
        protected void InitLocalization()
        {
            m_className = m_parentEditor.GetType().Name;
            if (m_className == "PWWelcomeScreen")
            {
                m_className += PWConst.VERSION_IN_FILENAMES;
            }
            LoadLocalizationData();

            if (m_langPack != null)
            {
                m_langPack.RemoveOnChangeAction(LoadLocalizationData);
                m_langPack.AddOnChangeAction(LoadLocalizationData);
            }

            Culture.LanguageChanged -= OnChangeLanguage;
            Culture.LanguageChanged += OnChangeLanguage;
        }

        /// <summary>
        /// Dispose of things
        /// </summary>
        public void Dispose()
        {
            if (Styles != null)
            {
                Styles.Dispose();
            }

            if (m_langPack != null)
            {
                m_langPack.RemoveOnChangeAction(LoadLocalizationData);
            }

            Culture.LanguageChanged -= OnChangeLanguage;
        }
        #endregion

        #region Localization connectors

        /// <summary>
        /// Need to sign this up to Culture.OnLanguageChange
        /// </summary>
        protected void OnChangeLanguage()
        {
            if (m_langPack != null)
            {
                m_langPack.RemoveOnChangeAction(LoadLocalizationData);
            }

            LoadLocalizationData();

            if (m_langPack != null)
            {
                m_langPack.AddOnChangeAction(LoadLocalizationData);
            }
        }

        /// <summary>
        /// Load localization data
        /// </summary>
        protected void LoadLocalizationData()
        {
            IsLocalizationReady = false;

            m_langPack = Culture.GetLanguagePackOrDefault(m_className);

            if (m_langPack == null)
            {
                m_langPack = new LanguagePack();
                return;
            }

            if (OnLocalizationUpdate != null)
            {
                OnLocalizationUpdate.Invoke();
            }

            IsLocalizationReady = true;
        }

        #endregion

        #region GUI Initialization

        /// <summary>
        /// Initialize editor styles
        /// </summary>
        public void Initialize()
        {
            if (Styles != null && Styles.Inited)
            {
                return;
            }

            if (Styles != null)
            {
                Styles.Dispose();
            }

            Styles = new CommonStyles();

            //Make sure that we have not disapeared off screen somewhere - reset if we have - fixes rare unity bug

            if (!m_parentEditor.PositionChecked)
            {
                m_parentEditor.position = CheckPosition(m_parentEditor.position);
            }
        }

        /// <summary>
        /// Check and caclulate window adjustment for editor windows relative to scene & game view
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maximized"></param>
        /// <returns></returns>
        public Rect CheckPosition(Rect position)
        {
            if (!m_parentEditor.PositionChecked)
            {
                if (!m_parentEditor.maximized)
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
                        position.x = scenePosition.xMin + ((scenePosition.width * 0.5f) - (position.width * 0.5f));
                    }
                    if (position.y < scenePosition.yMin || position.y > scenePosition.yMax)
                    {
                        position.y = scenePosition.yMin + 20f;
                    }
                }
                m_parentEditor.PositionChecked = true;
            }
            return position;
        }

        /// <summary>
        /// Loads textures for brush icons.
        /// </summary>
        private void LoadBrushIcons()
        {
            ArrayList arrayList = new ArrayList();
            int builtinBrushNum = 1;
            Texture builtinTexture;
            do
            {
                string fname = "pwub_builtin_" + builtinBrushNum.ToString() + PWConst.VERSION_IN_FILENAMES;
                builtinTexture = Resources.Load(fname) as Texture;
                if ((bool)((UnityEngine.Object)builtinTexture))
                {
                    arrayList.Add((object)builtinTexture);
                }
                ++builtinBrushNum;
            }
            while ((bool)((UnityEngine.Object)builtinTexture));
            int customBrushNum = 0;
            Texture customTexture;
            do
            {
                string fname = "brush_" + customBrushNum.ToString() + ".png";
                customTexture = (Texture)EditorGUIUtility.FindTexture(fname);
                if ((bool)((UnityEngine.Object)customTexture))
                {
                    arrayList.Add((object)customTexture);
                }
                ++customBrushNum;
            }
            while ((bool)((UnityEngine.Object)customTexture));
            m_BrushTextures = arrayList.ToArray(typeof(Texture2D)) as Texture2D[];
        }

        /// <summary>
        /// Adds the custom brushes to the brush icons.
        /// </summary>
        protected void SyncCustomBrushes()
        {
            if (m_customBrushes == null || m_customBrushes.Count < 1)
            {
                return;
            }

            List<Texture2D> brushList;
            if (m_BrushTextures != null)
            {
                brushList = new List<Texture2D>(m_BrushTextures);
            }
            else
            {
                brushList = new List<Texture2D>();
            }

            for (int i = 0; i < m_customBrushes.Count; i++)
            {
                brushList.Add(m_customBrushes[i]);
            }
            m_BrushTextures = brushList.ToArray();
        }

        /// <summary>
        /// Loads texture to add a custom brush icon and update the custom list.
        /// </summary>
        /// <param name="path">Path of the texture.</param>
        protected void AddCustomBrush(string path)
        {
            ArrayList arrayList;
            if (m_BrushTextures != null)
            {
                arrayList = new ArrayList(m_BrushTextures);
            }
            else
            {
                arrayList = new ArrayList();
            }

            EnsureReadableTexture(path);
            Texture texture = (Texture)AssetDatabase.LoadAssetAtPath<Texture>(path);
            if ((bool)((UnityEngine.Object)texture))
            {
                arrayList.Add((object)texture);
                m_customBrushes.Add(texture as Texture2D);
                m_BrushTextures = arrayList.ToArray(typeof(Texture2D)) as Texture2D[];

                //Signal Unity that the custom brush list has changed.
                GUI.changed = true;
            }
            else
            {
                Debug.LogWarningFormat("[{0}] Unable to load brush texture with path: '{1}'", m_appConfig.Name, path);
            }
        }

        /// <summary>
        /// Checks if texture is readable and makes it readable if not.
        /// </summary>
        protected void EnsureReadableTexture(string path)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer.isReadable == false)
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.LogFormat("[{0}] Brush textures need to be readable. Read/write was enabled on texture '{1}' because it was not readable.", m_appConfig.Name, Path.GetFileName(path));
            }
        }

        /// <summary>
        /// Removes a custom brush icon from both the custom list and the brushes.
        /// </summary>
        /// <param name="index">Index of the brush to be removed in the custom brush list.</param>
        protected void RemoveCustomBrush(int index)
        {
            if (index < 0 || index > m_customBrushes.Count - 1)
            {
                return;
            }

            List<Texture2D> brushList;
            if (m_BrushTextures != null)
            {
                brushList = new List<Texture2D>(m_BrushTextures);
            }
            else
            {
                brushList = new List<Texture2D>();
            }

            if (brushList.Remove(m_customBrushes[index]))
            {
                m_BrushTextures = brushList.ToArray();
            }
            else
            {
                Debug.LogWarningFormat("[{0}] Custom brush to be removed ('{1}') was not found amongst the brushes. Removing from the custom list...", m_appConfig.Name, m_customBrushes[index].name);
            }

            m_customBrushes.RemoveAt(index);
            //Signal Unity that the custom brush list has changed.
            GUI.changed = true;
        }

        /// <summary>
        /// Removes all the custom brush icons from both the custom list and the brushes.
        /// </summary>
        protected void ClearCustomBrushes()
        {
            List<Texture2D> brushList;
            if (m_BrushTextures != null)
            {
                brushList = new List<Texture2D>(m_BrushTextures);
            }
            else
            {
                brushList = new List<Texture2D>();
            }

            for (int i = 0; i < m_customBrushes.Count; i++)
            {
                if (brushList.Remove(m_customBrushes[i]))
                {
                    m_BrushTextures = brushList.ToArray();
                }
                else
                {
                    Debug.LogWarningFormat("[{0}] Custom brush to be removed ('{1}') was not found amongst the brushes. Removing from the custom list...", m_appConfig.Name, m_customBrushes[i].name);
                }
            }

            m_customBrushes.Clear();
            //Signal Unity that the custom brush list has changed.
            GUI.changed = true;
        }

        #endregion

        #region Main GUI elements

        /// <summary>
        /// Access to the language selector - Note: this is already included in the <seealso cref="GUIHeader()"/>
        /// </summary>
        public void LanguageDropdown()
        {
            Culture.LanguageSelectorGUI();
        }

        /// <summary>
        /// Draw the header for the GUI
        /// </summary>
        /// <param name="withLangDropdown">Display language drop down, or not</param>
        /// <param name="description">Description</param>
        /// <param name="url">Make description clickable if supplied</param>
        public void GUIHeader(bool withLangDropdown = true, string description = "", string url = "")
        {
            GUIHeader(m_appConfig, withLangDropdown, description, url);
        }

        /// <summary>
        /// Draw the header for the GUI using a custom <see cref="AppConfig"/>
        /// </summary>
        /// <param name="appconfig">Config of the app the header is for</param>
        /// <param name="withLangDropdown">Display language drop down, or not</param>
        /// <param name="description">Description</param>
        /// <param name="url">Make description clickable if supplied</param>
        public void GUIHeader(AppConfig appconfig, bool withLangDropdown = true, string description = "", string url = "")
        {
            GUIHeader(
                appconfig.Logo,
                string.Format("{0} ({1}-c{2})", appconfig.Name, appconfig.Version, PWConst.VERSION),
                withLangDropdown,
                description,
                url);
        }

        /// <summary>
        /// Draw a custom GUI header
        /// </summary>
        /// <param name="customText">Text to display (usually the product name and version)</param>
        /// <param name="withLangDropdown">Display language drop down, or not</param>
        /// <param name="description">Description</param>
        /// <param name="url">Make description clickable if supplied</param>
        public void GUIHeader(string customText, bool withLangDropdown = true, string description = "", string url = "")
        {
            GUIHeader(null, customText, withLangDropdown, description, url);
        }

        /// <summary>
        /// Draw a custom GUI header
        /// </summary>
        /// <param name="logo">Logo to be displayed</param>
        /// <param name="customText">Text to display (usually the product name and version)</param>
        /// <param name="withLangDropdown">Display language drop down, or not</param>
        /// <param name="description">Description</param>
        /// <param name="url">Make description clickable if supplied</param>
        public void GUIHeader(Texture2D logo, string customText, bool withLangDropdown = true, string description = "", string url = "")
        {
            if (Translate.Present)
            {
                customText += " -=TRANS=-";
            }
            if (Dev.Present)
            {
                customText += " -=DEV=-";
            }

            GUILayout.BeginHorizontal(Styles.header);
            {
                if (logo != null)
                {
                    //Logo and text intro
                    GUILayout.Label(logo, Styles.headerText, GUILayout.Width(Styles.headerText.fixedHeight));
                    GUILayout.Label(customText, Styles.headerText);
                }
                else
                {
                    //Text intro
                    GUILayout.Label(customText, Styles.headerText);
                }
                if (withLangDropdown)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical(GUILayout.Height(Styles.headerText.fixedHeight));
                    {
                        GUILayout.FlexibleSpace();
                        LanguageDropdown();
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndVertical();
                }

                if (!string.IsNullOrEmpty(description))
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        Text(description);
                    }
                    else
                    {
                        if (ClickableText(description))
                        {
                            Application.OpenURL(url);
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Add GUI Footer
        /// </summary>
        public void GUIFooter()
        {
            if (true)
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("", GUILayout.ExpandHeight(true));
                    GUILayout.BeginHorizontal(Styles.box);
                    {
                        //                GUILayout.BeginVertical();
                        //                GUILayout.Space(3f);
                        //                DrawImage(m_settings.m_latestNewsImage, 50f, 50f);
                        //                GUILayout.EndVertical();
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (ClickableHeadingNonLocalized(m_news.Title))
                                {
                                    Application.OpenURL(m_news.Url);
                                }

                                if (ClickableHeadingNonLocalized("Hide", GUILayout.Width(33f)))
                                {
                                    //m_settings.m_hideHeroMessage = true;
                                }
                            }
                            GUILayout.EndHorizontal();
                            ClickableText(new GUIContent(m_news.Body));
                        }
                        GUILayout.EndVertical();

                    }
                    GUILayout.EndHorizontal();

                }
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Draws the bar for the tabs.
        /// </summary>
        /// <param name="selected">Index of the selected tab.</param>
        /// <param name="labels">An array of the tab labels.</param>
        /// <param name="style">Style of the tabs.</param>
        /// <param name="selectedStyle">Style of the selected tab.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>Index of the selected tab</returns>
        public int TabBar(int selected, GUIContent[] labels, GUIStyle style, GUIStyle selectedStyle, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Styles.tabBar);
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    if (GUILayout.Button(labels[i], (i == selected) ? selectedStyle : style, options))
                    {
                        selected = i;
                    }
                }
            }
            GUILayout.EndHorizontal();
            return selected;
        }

        /// <summary>
        /// Create tab bar
        /// </summary>
        /// <param name="tabs">A <seealso cref="TabSet"/> object</param>
        /// <param name="style">Style of the tabs.</param>
        /// <param name="selectedStyle">Style of the selected tab.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Tabs(TabSet tabs, GUIStyle style, GUIStyle selectedStyle, params GUILayoutOption[] options)
        {
            tabs.ActiveTabIndex = TabBar(tabs.ActiveTabIndex, tabs.Labels, style, selectedStyle, options);

            GUILayout.BeginVertical(Styles.tabsFrame);
            {
                tabs.ActiveTabsScroll = GUILayout.BeginScrollView(tabs.ActiveTabsScroll, false, false);
                {
                    GUILayout.BeginVertical(Styles.tabsPanel);
                    {
                        tabs.ActiveTab.TabMethod.Invoke();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Create tab bar
        /// </summary>
        /// <param name="tabs">A <seealso cref="TabSet"/> object</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Tabs(TabSet tabs, params GUILayoutOption[] options)
        {
            tabs.ActiveTabIndex = TabBar(tabs.ActiveTabIndex, tabs.Labels, Styles.tab, Styles.tabSelected, options);

            GUILayout.BeginVertical(Styles.tabsFrame);
            {
                tabs.ActiveTabsScroll = GUILayout.BeginScrollView(tabs.ActiveTabsScroll, false, false);
                {
                    GUILayout.BeginVertical(Styles.tabsPanel);
                    {
                        tabs.ActiveTab.TabMethod.Invoke();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Create tab bar
        /// </summary>
        /// <param name="tabs">A <seealso cref="TabSet"/> object</param>
        /// <param name="style">Style of the tabs.</param>
        /// <param name="selectedStyle">Style of the selected tab.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void TabsNoBorder(TabSet tabs, GUIStyle style, GUIStyle selectedStyle, params GUILayoutOption[] options)
        {
            tabs.ActiveTabIndex = TabBar(tabs.ActiveTabIndex, tabs.Labels, style, selectedStyle, options);

            GUILayout.BeginVertical(Styles.tabsFrameBorderless);
            {
                tabs.ActiveTabsScroll = GUILayout.BeginScrollView(tabs.ActiveTabsScroll, false, false);
                {
                    GUILayout.BeginVertical(Styles.tabsPanel);
                    {
                        tabs.ActiveTab.TabMethod.Invoke();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Create tab bar
        /// </summary>
        /// <param name="tabs">A <seealso cref="TabSet"/> object</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void TabsNoBorder(TabSet tabs, params GUILayoutOption[] options)
        {
            tabs.ActiveTabIndex = TabBar(tabs.ActiveTabIndex, tabs.Labels, Styles.tab, Styles.tabSelected, options);

            GUILayout.BeginVertical(Styles.tabsFrameBorderless);
            {
                tabs.ActiveTabsScroll = GUILayout.BeginScrollView(tabs.ActiveTabsScroll, false, false);
                {
                    GUILayout.BeginVertical(Styles.tabsPanel);
                    {
                        tabs.ActiveTab.TabMethod.Invoke();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Create a Panel and return its status (true = open, false = closed)
        /// </summary>
        /// <param name="nameKey">Language pack key of the text label and help text of the panel.</param>
        /// <param name="contentMethod">Pass in the method that draws the content of this tab</param>
        /// <param name="defaultStatus">Should the panel be opened or closed by default?</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties of the panel.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>Panel status (true = open, false = closed).</returns>
        public bool Panel(string nameKey, Action<bool> contentMethod, bool defaultStatus = false, params GUILayoutOption[] options)
        {
            return Panel(GetContent(nameKey), nameKey, contentMethod, defaultStatus, options);
        }

        /// <summary>
        /// Create a Panel with custom label (not localized text, texture, etc) and return its status (true = open, false = closed)
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed as the panel's label.</param>
        /// <param name="panelLabel">Label content to be displayed for the panel.</param>
        /// <param name="helpKey">Language pack key of the help text to be displayed for the panel (when help is active).</param>
        /// <param name="contentMethod">Pass in the method that draws the content of this tab</param>
        /// <param name="defaultStatus">Should the panel be opened or closed by default?</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties of the panel.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>Panel status (true = open, false = closed).</returns>
        public bool Panel(GUIContent panelLabel, string helpKey, Action<bool> contentMethod, bool defaultStatus = false, params GUILayoutOption[] options)
        {
            if (!m_panelStatus.ContainsKey(contentMethod))
            {
                m_panelStatus[contentMethod] = new bool[]
                {
                    defaultStatus,
                    false,
                };
            }

            bool status = m_panelStatus[contentMethod][0];
            bool helpActive = m_panelStatus[contentMethod][1];

            GUILayout.BeginVertical(Styles.panelFrame, options);
            {
                GUILayout.BeginHorizontal();
                {
                    status = GUILayout.Toggle(status, status ? "-" : "+", Styles.panelLabel);
                    GUILayout.Space(-5f);
                    status = GUILayout.Toggle(status, panelLabel, Styles.panelLabel);
                    GUILayout.FlexibleSpace();
                    HelpToggle(ref helpActive);
                }
                GUILayout.EndHorizontal();

                if (helpActive)
                {
                    GUILayout.Space(2f);
                    InlineHelp(helpKey, helpActive);
                }

                if (status)
                {
                    GUILayout.BeginVertical(Styles.panel);
                    {
                        contentMethod.Invoke(helpActive);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();

            m_panelStatus[contentMethod][0] = status;
            m_panelStatus[contentMethod][1] = helpActive;
            return status;
        }

        /// <summary>
        /// Create a Panel with custom label (not localized text, texture, etc) and return its status (true = open, false = closed)
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed as the panel's label.</param>
        /// <param name="panelLabel">Label content to be displayed for the panel.</param>
        /// <param name="helpKey">Language pack key of the help text to be displayed for the panel (when help is active).</param>
        /// <param name="contentMethod">Pass in the method that draws the content of this tab</param>
        /// <param name="defaultStatus">Should the panel be opened or closed by default?</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties of the panel.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>Panel status (true = open, false = closed).</returns>
        public bool Panel(GUIContent panelLabel, Action<bool> contentMethod, bool defaultStatus = false, params GUILayoutOption[] options)
        {
            if (!m_panelStatus.ContainsKey(contentMethod))
            {
                m_panelStatus[contentMethod] = new bool[]
                {
                    defaultStatus,
                    false,
                };
            }

            bool status = m_panelStatus[contentMethod][0];
            bool helpActive = m_panelStatus[contentMethod][1];

            GUILayout.BeginVertical(Styles.panelFrame, options);
            {
                GUILayout.BeginHorizontal();
                {
                    status = GUILayout.Toggle(status, status ? "-" : "+", Styles.panelLabel);
                    GUILayout.Space(-5f);
                    status = GUILayout.Toggle(status, panelLabel, Styles.panelLabel);
                    GUILayout.FlexibleSpace();
                    HelpToggle(ref helpActive);
                }
                GUILayout.EndHorizontal();

                if (status)
                {
                    GUILayout.BeginVertical(Styles.panel);
                    {
                        contentMethod.Invoke(helpActive);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();

            m_panelStatus[contentMethod][0] = status;
            m_panelStatus[contentMethod][1] = helpActive;
            return status;
        }

        #endregion

        #region Images

        /// <summary>
        /// Display an image - the image must be of type editor & legacy gui to display
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        public void Image(Texture2D image, float width, float height)
        {
            GUILayout.Label(image, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Display an image - the image must be of type editor & legacy gui to display
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void Image(Texture2D image, params GUILayoutOption[] options)
        {
            GUILayout.Label(image, options);
        }

        #endregion

        #region Titles, Headings, Text

        /// <summary>
        /// Title text
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void Title(string key, params GUILayoutOption[] options)
        {
            GUILayout.Label(GetContent(key), Styles.title, options);
        }

        /// <summary>
        /// Title text - static, not localized
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void TitleNonLocalized(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, Styles.title, options);
        }

        /// <summary>
        /// Heading text - we can create Heading 1, Heading 2, etc. if needed
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void Heading(string key, params GUILayoutOption[] options)
        {
            GUILayout.Label(GetContent(key), Styles.heading);
        }

        /// <summary>
        /// Heading text - not localized - we can create Heading 1, Heading 2, etc. if needed
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void HeadingNonLocalized(string text, params GUILayoutOption[] options)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            GUILayout.Label(text, Styles.heading);
        }

        /// <summary>
        /// Text (body style)
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void Text(string key, params GUILayoutOption[] options)
        {
            GUILayout.Label(GetContent(key), Styles.body, options);
        }

        /// <summary>
        /// NON LOCALOZED Text (body style)
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void TextNonLocalized(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, Styles.body, options);
        }

        #endregion

        #region Links and clickables

        /// <summary>
        /// Heading text as a clickable object
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableHeading(string key, params GUILayoutOption[] options)
        {
            return ClickableHeadingNonLocalized(GetContent(key), options);
        }

        /// <summary>
        /// Heading text as a clickable object - not localized
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableHeadingNonLocalized(string text, params GUILayoutOption[] options)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            return ClickableHeadingNonLocalized(new GUIContent(text), options);
        }

        /// <summary>
        /// Heading text as a clickable object
        /// </summary>
        /// <param name="content">Text to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableHeadingNonLocalized(GUIContent content, params GUILayoutOption[] options)
        {
            if (content == null)
            {
                return false;
            }
            var position = GUILayoutUtility.GetRect(content, Styles.heading, options);
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = Styles.heading.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = oldColor;
            Handles.EndGUI();
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, content, Styles.heading);
        }

        /// <summary>
        /// Draw clickable body text
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableImage(Texture2D image, params GUILayoutOption[] options)
        {
            if (image == null)
            {
                return false;
            }
            GUIContent content = new GUIContent(image);

            var position = GUILayoutUtility.GetRect(content, Styles.clickImg, options);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, content, Styles.clickImg);
        }

        /// <summary>
        /// Draw clickable body text
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableText(string key, params GUILayoutOption[] options)
        {
            return ClickableText(GetContent(key), options);
        }

        /// <summary>
        /// Draw clickable body text - not localized
        /// </summary>
        /// <param name="content">GUI content to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns></returns>
        public bool ClickableText(GUIContent content, params GUILayoutOption[] options)
        {
            if (content == null)
            {
                return false;
            }
            var position = GUILayoutUtility.GetRect(content, Styles.body, options);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, content, Styles.body);
        }

        /// <summary>
        /// Draw Centered clickable body text
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableTextCentered(string key, params GUILayoutOption[] options)
        {
            return ClickableTextCentered(GetContent(key), options);
        }

        /// <summary>
        /// Draw Centered clickable body text - not localized
        /// </summary>
        /// <param name="content">GUI content to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns></returns>
        public bool ClickableTextCentered(GUIContent content, params GUILayoutOption[] options)
        {
            if (content == null)
            {
                return false;
            }
            var position = GUILayoutUtility.GetRect(content, Styles.centeredBody, options);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, content, Styles.centeredBody);
        }

        /// <summary>
        /// Draw Centered Bold clickable body text
        /// </summary>
        /// <param name="key">Language pack key of the text to be displayed</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if it was clicked</returns>
        public bool ClickableImgDescriptionBold(string key, params GUILayoutOption[] options)
        {
            return ClickableImgDescriptionBold(GetContent(key), options);
        }

        /// <summary>
        /// Draw Centered Bold clickable body text - not localized
        /// </summary>
        /// <param name="content">GUI content to display</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns></returns>
        public bool ClickableImgDescriptionBold(GUIContent content, params GUILayoutOption[] options)
        {
            if (content == null)
            {
                return false;
            }
            var position = GUILayoutUtility.GetRect(content, Styles.imgDescriptionBold, options);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, content, Styles.imgDescriptionBold);
        }

        /// <summary>
        /// Insert a link
        /// </summary>
        /// <param name="key">Language pack key of the link to be displayed. The tooltip is used as the URL just like with Non Localized links. </param>
        public void Link(string key)
        {

            LocalizationItem content;
            if (m_langPack.Items.TryGetValue(key, out content))
            {
                if (GUILayout.Button(new GUIContent(content.Val, content.Tooltip), Styles.link, GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL(content.Tooltip);
                }

                Rect r = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
                if (r.Contains(Event.current.mousePosition))
                {
                    BottomBorder(r, Styles.link.normal.textColor, false);
                }
            }
            else if (Dev.Present)
            {
                Debug.LogError(string.Format("Could not find link with the key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
            }

        }

        /// <summary>
        /// Insert a link
        /// </summary>
        /// <param name="text">Text to display for the link</param>
        /// <param name="url">The url the link points to</param>
        public void LinkNonLocalized(string text, string url)
        {
            if (GUILayout.Button(new GUIContent(text, url), Styles.link, GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(url);
            }

            Rect r = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
            if (r.Contains(Event.current.mousePosition))
            {
                BottomBorder(r, Styles.link.normal.textColor, false);
            }
        }

        #endregion

        #region Borders

        /// <summary>
        /// Draw bottom border for the last GUI element
        /// </summary>
        /// <param name="color">Color of the border</param>
        /// <param name="stretch">Should the border stretch accross the entire window?</param>
        public void BottomBorder(Color color, bool stretch = true)
        {
            Rect r = GUILayoutUtility.GetLastRect();
            BottomBorder(r, color, stretch);
        }

        /// <summary>
        /// Draw bottom border for a rect
        /// </summary>
        /// <param name="rect">Rect to draw the border for</param>
        /// <param name="color">Color of the border</param>
        /// <param name="stretch">Should the border stretch accross the entire window?</param>
        public void BottomBorder(Rect rect, Color color, bool stretch = true)
        {
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = color;
            if (stretch)
            {
                Handles.DrawLine(new Vector3(0, rect.yMax), new Vector3(EditorGUIUtility.currentViewWidth, rect.yMax));
            }
            else
            {
                Handles.DrawLine(new Vector3(rect.xMin, rect.yMax), rect.max);
            }
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        /// <summary>
        /// Draw top border for the last GUI element
        /// </summary>
        /// <param name="color">Color of the border</param>
        /// <param name="stretch">Should the border stretch accross the entire window?</param>
        public void TopBorder(Color color, bool stretch = true)
        {
            Rect r = GUILayoutUtility.GetLastRect();
            TopBorder(r, color, stretch);
        }

        /// <summary>
        /// Draw top border for a rect
        /// </summary>
        /// <param name="rect">Rect to draw the border for</param>
        /// <param name="color">Color of the border</param>
        /// <param name="stretch">Should the border stretch accross the entire window?</param>
        public void TopBorder(Rect rect, Color color, bool stretch = true)
        {
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = color;
            if (stretch)
            {
                Handles.DrawLine(new Vector3(0, rect.yMin), new Vector3(EditorGUIUtility.currentViewWidth, rect.yMin));
            }
            else
            {
                Handles.DrawLine(new Vector3(rect.xMin, rect.yMin), new Vector3(rect.xMax, rect.yMin));
            }
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        /// <summary>
        /// Draw top border in the space provided by an abstract label
        /// </summary>
        /// <param name="label">Label - used for spacing / volume</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void AbstractBorder(GUIContent label, Color color, params GUILayoutOption[] options)
        {
            if (label == null)
            {
                return;
            }
            var position = GUILayoutUtility.GetRect(label, Styles.body, options);
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = color;
            Handles.DrawLine(new Vector3(position.xMin, position.y), new Vector3(position.xMax, position.y));
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Draw a button - not localized (left aligned)
        /// </summary>
        /// <param name="text">Text label for the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonNonLocalized(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, options);
        }

        /// <summary>
        /// Draw a button - not localized (left aligned)
        /// </summary>
        /// <param name="content">Text image and tooltip for this button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonNonLocalized(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Button(content, options);
        }

        /// <summary>
        /// Draw a centered button
        /// </summary>
        /// <param name="key">Language pack key of the text to display on the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonCentered(string key, params GUILayoutOption[] options)
        {
            bool pressed;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                pressed = GUILayout.Button(GetContent(key), options);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            return pressed;
        }

        /// <summary>
        /// Draw a centered button - not localized
        /// </summary>
        /// <param name="text">Text label for the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonCenteredNonLocalized(string text, params GUILayoutOption[] options)
        {
            bool pressed;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                pressed = GUILayout.Button(text, options);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            return pressed;
        }

        /// <summary>
        /// Draw a centered button - not localized
        /// </summary>
        /// <param name="content">Text, image and tooltip for this button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonCenteredNonLocalized(GUIContent content, params GUILayoutOption[] options)
        {
            bool pressed;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                pressed = GUILayout.Button(content, options);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            return pressed;
        }

        /// <summary>
        /// Draw a right aligned button
        /// </summary>
        /// <param name="key">Language pack key of the text to display on the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonRight(string key, params GUILayoutOption[] options)
        {
            bool pressed;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                pressed = GUILayout.Button(GetContent(key), options);
            }
            GUILayout.EndHorizontal();
            return pressed;
        }

        /// <summary>
        /// Draw a right aligned button - not localized
        /// </summary>
        /// <param name="text">Text label for the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonRightNonLocalized(string text, params GUILayoutOption[] options)
        {
            bool pressed;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                pressed = GUILayout.Button(text, options);
            }
            GUILayout.EndHorizontal();
            return pressed;
        }

        /// <summary>
        /// Draw a right aligned button - not localized
        /// </summary>
        /// <param name="content">Text, image and tooltip for this button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public bool ButtonRightNonLocalized(GUIContent content, params GUILayoutOption[] options)
        {
            bool pressed;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                pressed = GUILayout.Button(content, options);
            }
            GUILayout.EndHorizontal();
            return pressed;
        }

        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="key">Language pack key of the text to display on the button</param>
        /// <returns>True if clicked</returns>
        public bool ButtonAutoIndent(string key)
        {
            return ButtonAutoIndent(GetContent(key));
        }

        /// <summary>
        /// Draw a button (left aligned)
        /// </summary>
        /// <param name="key">Language pack key of the text to display on the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public void ToggleButton(string key, ref bool value, params GUILayoutOption[] options)
        {
            ToggleButton(key, ref value, Styles.toggleButton, Styles.toggleButtonDown, options);
        }

        /// <summary>
        /// Draw a button (left aligned)
        /// </summary>
        /// <param name="key">Language pack key of the text to display on the button</param>
        /// <param name="style">Optional GUIStyle to use for the label.</param>
        /// <param name="styleDown">Optional GUIStyle to use for the label when the toggle is activated.</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public void ToggleButton(string key, ref bool value, GUIStyle style, GUIStyle styleDown, params GUILayoutOption[] options)
        {
            value = GUILayout.Toggle(value, GetContent(key), value ? styleDown : style, options);
        }

        /// <summary>
        /// Draw a button - not localized (left aligned)
        /// </summary>
        /// <param name="text">Text label for the button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public void ToggleButtonNonLocalized(string text, ref bool value, params GUILayoutOption[] options)
        {
            value = GUILayout.Toggle(value, text, value ? Styles.toggleButtonDown : Styles.toggleButton, options);
        }

        /// <summary>
        /// Draw a button - not localized (left aligned)
        /// </summary>
        /// <param name="content">Text image and tooltip for this button</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public void ToggleButtonNonLocalized(GUIContent content, ref bool value, params GUILayoutOption[] options)
        {
            value = GUILayout.Toggle(value, content, value ? Styles.toggleButtonDown : Styles.toggleButton, options);
        }

        /// <summary>
        /// Draw the help toggle
        /// </summary>
        /// <param name="value">A reference to the bool value (on/off).</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        /// <returns>True if clicked</returns>
        public void HelpToggle(ref bool value, params GUILayoutOption[] options)
        {
            value = GUILayout.Toggle(value, value ? Styles.helpOn : Styles.helpOff, Styles.helpToggle, options);
        }

        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="content">Text, image and tooltip for this button</param>
        /// <returns>True is clicked</returns>
        public bool ButtonAutoIndent(GUIContent content)
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
        /// Draw a delete button
        /// </summary>
        public bool DeleteButton()
        {
            return GUILayout.Button("\u00D7", Styles.deleteButton);
        }

        /// <summary>
        /// Draw a delete button
        /// </summary>
        public bool DeleteButton(Rect rect)
        {
            return GUI.Button(rect, "\u00D7", Styles.deleteButton);
        }

        #endregion

        #region Checkboxes

        /// <summary>
        /// Draw a checkbox (Same as <seealso cref="Checkbox(string, ref bool, GUILayoutOption[])"/>)
        /// </summary>
        /// <param name="labelKey">Language pack key of the label of the checkbox</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void Toggle(string labelKey, ref bool value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Toggle(GetContent(labelKey), value, options);
        }

        /// <summary>
        /// Draw a checkbox (Same as <seealso cref="Toggle(string, ref bool, GUILayoutOption[])"/>)
        /// </summary>
        /// <param name="labelKey">Language pack key of the label of the checkbox</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void Checkbox(string labelKey, ref bool value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Toggle(GetContent(labelKey), value, options);
        }

        /// <summary>
        /// Draw a checkbox that falls to the left of its label
        /// </summary>
        /// <param name="labelKey">Language pack key of the label of the checkbox</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="options">An option list of layout options that specify extra layouting properties for the label.
        /// Any values passed in here will override settings defined by the sytle.
        /// See Aslo: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth,
        /// GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight</param>
        public void LeftCheckbox(string labelKey, ref bool value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Toggle(value, GUILayout.MaxWidth(12f));
            GUILayout.Label(GetContent(labelKey), options);
        }

        #endregion

        #region Selection Grids

        /// <summary>
        /// Create a brush Selection Grid. Use <see cref="GetBrush(int, int)"/>(int index, int size) with the returned index to retrieve the brush.
        /// </summary>
        /// <param name="key">Language pack key of the label of the checkbox (optional)</param>
        /// <param name="selected">Index of the selected brush.</param>
        /// <param name="doubleClick">Will be true if the selected item was douple clicked.</param>
        /// <param name="customBrushes">The list that tracks/stores custom brushes.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>Index of the selected brush.</returns>
        public int BrushSelectionGrid(string key, int selected, out bool doubleClick, ref List<Texture2D> customBrushes)
        {
            return BrushSelectionGrid(GetContent(key), selected, out doubleClick, ref customBrushes);
        }

        /// <summary>
        /// Create a brush Selection Grid. Use <see cref="GetBrush(int, int)"/>(int index, int size) with the returned index to retrieve the brush.
        /// </summary>
        /// <param name="key">Language pack key of the label of the checkbox (optional)</param>
        /// <param name="selected">Index of the selected brush.</param>
        /// <param name="doubleClick">Will be true if the selected item was douple clicked.</param>
        /// <param name="customBrushes">The list that tracks/stores custom brushes.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>Index of the selected brush.</returns>
        public int BrushSelectionGrid(string key, int selected, out bool doubleClick, ref List<Texture2D> customBrushes, bool helpSwitch)
        {
            selected = BrushSelectionGrid(GetContent(key), selected, out doubleClick, ref customBrushes);
            InlineHelp(key, helpSwitch);
            return selected;
        }

        /// <summary>
        /// Create a brush Selection Grid. Use <see cref="GetBrush(int, int)"/>(int index, int size) with the returned index to retrieve the brush.
        /// </summary>
        /// <param name="content">Non localized label for the selection grid (optional).</param>
        /// <param name="selected">Index of the selected brush.</param>
        /// <param name="doubleClick">Will be true if the selected item was douple clicked.</param>
        /// <param name="customBrushes">The list that tracks/stores custom brushes.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>Index of the selected brush.</returns>
        public int BrushSelectionGrid(GUIContent content, int selected, out bool doubleClick, ref List<Texture2D> customBrushes)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(content, EditorStyles.boldLabel, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal((GUIStyle)"box");
                {
                    GUILayout.Label("Drag and drop custom brushes");
                    if (GUILayout.Button("Reset", EditorStyles.miniButtonRight))
                    {
                        ClearCustomBrushes();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(-5f);

            return BrushSelectionGrid(selected, out doubleClick, ref customBrushes);
        }

        /// <summary>
        /// Create a brush Selection Grid. Use <see cref="GetBrush(int, int)"/>(int index, int size) with the returned index to retrieve the brush.
        /// </summary>
        /// <param name="selected">Index of the selected brush.</param>
        /// <param name="doubleClick">Will be true if the selected item was douple clicked.</param>
        /// <param name="customBrushes">The list that tracks/stores custom brushes.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>Index of the selected brush.</returns>
        public int BrushSelectionGrid(int selected, out bool doubleClick, ref List<Texture2D> customBrushes)
        {
            // Init the custom stuff if it has not been yet
            if (m_customBrushes == null)
            {
                if (customBrushes == null)
                {
                    customBrushes = new List<Texture2D>();
                }
                m_customBrushes = customBrushes;
                SyncCustomBrushes();
            }

            Event evt = Event.current;
            selected = SelectionGrid(selected, (Texture[])m_BrushTextures, 32, Styles.gridList, "No brushes defined.", out doubleClick);
            Rect rect = GUILayoutUtility.GetLastRect();

            if (evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated)
            {
                if (!rect.Contains(evt.mousePosition))
                    return selected;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    //Handle paths
                    foreach (var path in DragAndDrop.paths)
                    {
                        if (!path.StartsWith("Assets"))
                        {
                            Debug.LogWarningFormat("[{0}] Dragged item does not seem to be part of the Unity project. Path: '{1}'", m_appConfig.Name, path);
                            continue;
                        }

                        //Check file type
                        string fileType = Path.GetExtension(path).ToLower();
                        if (ACCEPTED_BRUSH_EXTENSIONS.Contains(fileType) == false)
                        {
                            Debug.LogWarningFormat("[{0}] Extension of dragged item ({1}) is not an accepted brush extension (accepted extensions: {2}). Item path: '{3}'",
                                m_appConfig.Name, fileType, string.Join(", ", new List<string>(ACCEPTED_BRUSH_EXTENSIONS).ToArray()), path); //Need this odd solution because string.Join doesn't handle IEnumerables in older versions
                            continue;
                        }

                        AddCustomBrush(path);
                    }
                }
            }
            return selected;
        }

        /// <summary>
        /// Get a new brush with <paramref name="index"/> at the given <paramref name="size"/>.
        /// </summary>
        public UBrush GetBrush(int index, int size)
        {
            UBrush brush = new UBrush();
            brush.Load(m_BrushTextures[index], size);
            return brush;
        }

        /// <summary>
        /// Create a Selection Grid.
        /// </summary>
        /// <param name="selected">Index of the selected item.</param>
        /// <param name="textures">An array of selectable textures to display in the grid.</param>
        /// <param name="approxSize">Approximate grid size.</param>
        /// <param name="style">Style of the Selection Grid.</param>
        /// <param name="emptyString">String to display if the grid is empty.</param>
        /// <param name="doubleClick">Will be true if the selected item was douple clicked.</param>
        /// <returns>Index of the selected item.</returns>
        public int SelectionGrid(int selected, Texture[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            GUILayout.BeginVertical((GUIStyle)"box", GUILayout.MinHeight(10f));
            {
                doubleClick = false;
                if (textures.Length != 0)
                {
                    float num2 = (EditorGUIUtility.currentViewWidth - 20f) / (float)approxSize;
                    int num3 = (int)Mathf.Ceil((float)textures.Length / num2);
                    Rect aspectRect = GUILayoutUtility.GetAspectRect(num2 / (float)num3);
                    Event current = Event.current;
                    if (current.type == EventType.MouseDown && current.clickCount == 2 && aspectRect.Contains(current.mousePosition))
                    {
                        doubleClick = true;
                        current.Use();
                    }
                    selected = GUI.SelectionGrid(aspectRect, Math.Min(selected, textures.Length - 1), textures, Mathf.RoundToInt(EditorGUIUtility.currentViewWidth - 20f) / approxSize, style);
                }
                else
                {
                    GUILayout.Label(emptyString);
                }
            }
            GUILayout.EndVertical();
            return selected;
        }

        #endregion

        #region Localized content getters

        /// <summary>
        /// Get content by key from localization package, or the key if not found
        /// </summary>
        /// <param name="key">Key of the content</param>
        /// <returns>Returns content or the key if doesn't exist in Localization</returns>
        public GUIContent GetContent(string key)
        {
            LocalizationItem content;
            if (m_langPack.Items.TryGetValue(key, out content))
            {
                return new GUIContent(content.Val, content.Tooltip);
            }
            else
            {
                if (Dev.Present)
                {
                    Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
                    key = "*M*" + key;
                }
                return new GUIContent(key);
            }
        }

        /// <summary>
        /// Get content by key from localization package, or the key if not found and return GUIContent that has both text an image, and has tooltip.
        /// </summary>
        /// <param name="key">Key of the content</param>
        /// <param name="image">Key of the content</param>
        /// <returns>Returns content or the key if doesn't exist in Localization. The GUIContent will have both text, image, and tooltip if any.</returns>
        public GUIContent GetContent(string key, Texture image)
        {
            LocalizationItem content;
            if (m_langPack.Items.TryGetValue(key, out content))
            {
                return new GUIContent(content.Val, image, content.Tooltip);
            }
            else
            {
                if (Dev.Present)
                {
                    Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
                    key = "*M*" + key;
                }
                return new GUIContent(key, image);
            }
        }

        /// <summary>
        /// Get contents by key from localization package, or the keys if not found
        /// </summary>
        /// <param name="keys">An array of localization keys to get the contents for</param>
        /// <returns>Returns an array of contents or keys (if keys doen't exist in Localization)</returns>
        public GUIContent[] GetContent(string[] keys)
        {
            GUIContent[] contents = new GUIContent[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                LocalizationItem content;
                if (m_langPack.Items.TryGetValue(keys[i], out content))
                {
                    contents[i] = new GUIContent(content.Val, content.Tooltip);
                }
                else
                {
                    if (Dev.Present)
                    {
                        Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", keys[i], m_className, Culture.Language));
                        keys[i] = "*M*" + keys[i];
                    }
                    contents[i] = new GUIContent(keys[i]);
                }
            }

            return contents;
        }

        /// <summary>
        /// Get the text value by key from localization package, or the key if not found
        /// </summary>
        /// <param name="key">Key of the content</param>
        /// <returns>Returns the text value or the key if doesn't exist in Localization</returns>
        public string GetTextValue(string key)
        {
            LocalizationItem content;
            if (m_langPack.Items.TryGetValue(key, out content))
            {
                return content.Val;
            }
            else
            {
                if (Dev.Present)
                {
                    Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
                    key = "*M*" + key;
                }
                return key;
            }
        }

        /// <summary>
        /// Get tooltip by key from localization package, or the key if not found
        /// </summary>
        /// <param name="key">Key of the content</param>
        /// <returns>Returns the tooltip or the key if doesn't exist in Localization</returns>
        public string GetTooltip(string key)
        {
            LocalizationItem content;
            if (m_langPack.Items.TryGetValue(key, out content))
            {
                return content.Tooltip;
            }
            else
            {
                if (Dev.Present)
                {
                    Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
                    key = "*M*" + key;
                }
                return key;
            }
        }

        /// <summary>
        /// Get detailed help by key from localization package but only if the item has help
        /// </summary>
        /// <param name="key">Key of the content</param>
        public void InlineHelp(string key, bool active)
        {
            if (!active)
            {
                return;
            }

            LocalizationItem content;
            if (m_langPack.Items.TryGetValue(key, out content))
            {
                if (!string.IsNullOrEmpty(content.Help))
                {
                    //GUILayout.Label(content.Help, Styles.help);
                    GUILayout.BeginVertical(Styles.helpBox);
                    {
                        // Link regex goups
                        // 0: The whole match
                        // 1: Preceding or following text
                        // 2: >helper - ignore this<
                        // 3: Link (if there)
                        // 4: Link text (if there)
                        MatchCollection matches = Regex.Matches(content.Help, LINK_REGEX_PATTERN);
                        foreach (Match m in matches)
                        {
                            if (m.Groups.Count > 1 && m.Groups[1].Success && m.Groups[1].Length > 0)
                            {
                                GUILayout.Label(m.Groups[1].Value, Styles.help);
                            }

                            if (m.Groups.Count > 3 && m.Groups[3].Success)
                            {
                                if (m.Groups.Count > 4 && m.Groups[4].Success && m.Groups[4].Length > 0)
                                {
                                    LinkNonLocalized(m.Groups[4].Value, m.Groups[3].Value);
                                }
                                else
                                {
                                    LinkNonLocalized(m.Groups[3].Value, m.Groups[3].Value);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            else
            {
                if (Dev.Present)
                {
                    Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
                }
            }
        }

        /// <summary>
        /// Get detailed help for a group of items by key from localization package but only if the items have help
        /// </summary>
        /// <param name="keys">An array of localization keys to get the contents for</param>
        public void InlineHelp(string[] keys, bool active)
        {
            if (!active)
            {
                return;
            }

            List<string> helpTexts = new List<string>();

            foreach (string key in keys)
            {
                LocalizationItem content;
                if (m_langPack.Items.TryGetValue(key, out content))
                {
                    if (!string.IsNullOrEmpty(content.Help))
                    {
                        helpTexts.Add(content.Help);
                    }
                }
                else
                {
                    if (Dev.Present)
                    {
                        Debug.LogError(string.Format("Could not find key '{0}' in localization data for '{1}' language '{2}'", key, m_className, Culture.Language));
                    }
                }
            }

            if (helpTexts.Count > 0)
            {
                GUILayout.BeginVertical(Styles.helpBox);
                {
                    GUILayout.Label(string.Join("\n\n", helpTexts.ToArray()), Styles.help);
                }
                GUILayout.EndVertical();
            }
        }

        #endregion

        public class CommonStyles : IDisposable
        {
            public GUIStyle header;
            public GUIStyle headerText;
            public GUIStyle box;
            public GUIStyle darkBox;

            public GUIStyle tabBar;
            public GUIStyle tab;
            public GUIStyle tabSelected;
            public GUIStyle tabsFrame;
            public GUIStyle tabsFrameBorderless;
            public GUIStyle tabsPanel;

            public GUIStyle gridList;
            public GUIStyle gridBox;

            public GUIStyle panelLabel;
            public GUIStyle panel;
            public GUIStyle panelFrame;

            public GUIStyle toggleButton;
            public GUIStyle toggleButtonDown;
            public GUIStyle helpToggle;
            public GUIStyle deleteButton;
            public GUIStyle foldoutBold;

            public GUIStyle body;
            public GUIStyle centeredBody;
            public GUIStyle imgDescriptionBold;
            public GUIStyle heading;
            public GUIStyle title;
            public GUIStyle wrap;
            public GUIStyle editWrap;

            public GUIStyle helpBox;
            public GUIStyle help;

            public GUIStyle link;
            public GUIStyle clickImg;

            public Texture2D helpOff;
            public Texture2D helpOn;

            private List<Texture2D> m_texturesInMemory = new List<Texture2D>();

            // Temp
            //public GUIStyle proInspectorBG;

            public bool Inited { get { return panel.normal.background != null; } }

            public CommonStyles()
            {
                //proInspectorBG = new GUIStyle();
                //proInspectorBG.normal.background = GetBGTexture(new Color(0.282f, 0.282f, 0.282f));

                // GUI Header
                header = new GUIStyle("Box");
                header.name = "GUI Header";
                header.normal.textColor = GUI.skin.label.normal.textColor;
                header.fontStyle = FontStyle.Bold;
                header.alignment = TextAnchor.MiddleLeft;
                header.stretchWidth = true;
                header.margin = new RectOffset(0, 0, 0, 0);
                header.overflow = new RectOffset(2, 2, 2, 0);
                header.padding = new RectOffset(6, 6, 6, 4);

                headerText = new GUIStyle(GUI.skin.label);
                headerText.fontStyle = FontStyle.Bold;
                headerText.alignment = TextAnchor.MiddleLeft;
                headerText.fixedHeight = 20f;
                headerText.wordWrap = true;

                // Box
                box = new GUIStyle(GUI.skin.box);
                box.normal.textColor = GUI.skin.label.normal.textColor;
                box.fontStyle = FontStyle.Bold;
                box.alignment = TextAnchor.UpperLeft;

                darkBox = new GUIStyle(box);
                darkBox.fontStyle = FontStyle.Normal;

                // Tabs
                tabBar = new GUIStyle(GUI.skin.box);
                tabBar.margin = new RectOffset(4, 4, 0, 0);
                tabBar.padding = new RectOffset(0, 0, 0, 0);
                tabBar.overflow = new RectOffset(0, 0, 1, 0);
                tabBar.normal.background = GUI.skin.label.normal.background;

                tabSelected = new GUIStyle(GUI.skin.button)
                {
                    // Setting height here will always add some sort of margin, so setting it with GUILayoutOptions
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(30, 30, 2, 2),
                    fixedHeight = 20f,
                    stretchWidth = true,
                    wordWrap = false,
                };

                tab = new GUIStyle(tabSelected);

                // Tabs Frame
                tabsFrame = new GUIStyle(tabBar);
                tabsFrame.normal.textColor = GUI.skin.label.normal.textColor;
                tabsFrame.padding = new RectOffset(1, 1, 1, 1);
                tabsFrame.stretchHeight = true;

                tabsFrameBorderless = new GUIStyle(tabsFrame);

                // Tabs Panel
                tabsPanel = new GUIStyle(GUI.skin.box);
                tabsPanel.normal.textColor = GUI.skin.label.normal.textColor;
                tabsPanel.stretchHeight = true;
                tabsPanel.margin = new RectOffset(0, 0, 0, 0);
                tabsPanel.padding = new RectOffset(5, 5, 5, 5);
                tabsPanel.alignment = TextAnchor.UpperLeft;
                tabsPanel.normal.background = GUI.skin.label.normal.background;

                // Panel
                panelLabel = new GUIStyle(GUI.skin.label);
                panelLabel.normal.textColor = GUI.skin.label.normal.textColor;
                panelLabel.fontStyle = FontStyle.Bold;
                panelLabel.normal.background = GUI.skin.label.normal.background;

                //panelLabel.normal.textColor = new Color(0.773f, 0.773f, 0.773f);

                // Panel Frame
                panelFrame = new GUIStyle(GUI.skin.box);
                panelFrame.normal.textColor = GUI.skin.label.normal.textColor;
                panelFrame.fontStyle = FontStyle.Bold;
                panelFrame.alignment = TextAnchor.UpperLeft;

                // Panel
                panel = new GUIStyle(GUI.skin.box);
                panel.normal.textColor = GUI.skin.label.normal.textColor;
                panel.alignment = TextAnchor.UpperLeft;

                // Toggle Button
                toggleButton = new GUIStyle(GUI.skin.button);
                toggleButtonDown = new GUIStyle(toggleButton);
                toggleButtonDown.normal.background = toggleButton.active.background;

                // Help Toggle Button
                helpToggle = new GUIStyle(GUI.skin.label);
                helpToggle.margin = new RectOffset(0, 6, 3, 0);
                helpToggle.padding = new RectOffset(0, 0, 0, 0);
                helpToggle.contentOffset = new Vector2(0f, -1f);
                //helpToggle.fixedHeight = 16f;

                // Delete button
                deleteButton = new GUIStyle("button");
                deleteButton.padding = new RectOffset(0, 0, 0, 0);
                deleteButton.margin = new RectOffset(3, 3, 2, 2);
                deleteButton.fontStyle = FontStyle.Bold;
                deleteButton.fixedWidth = 17f;
                deleteButton.fixedHeight = 17f;
                deleteButton.normal.textColor = Color.red;
                deleteButton.active.textColor = new Color(1f, 0.6f, 0.1f);
                deleteButton.contentOffset = new Vector2(0f, 1f);
                deleteButton.alignment = TextAnchor.LowerRight;
                deleteButton.fontSize = 18;

                // Foldout
                foldoutBold = new GUIStyle(EditorStyles.foldout);
                foldoutBold.normal.textColor = GUI.skin.label.normal.textColor;
                foldoutBold.fontStyle = FontStyle.Bold;

                // Body
                body = new GUIStyle(GUI.skin.label);
                body.fontStyle = FontStyle.Normal;
                body.wordWrap = true;
                body.richText = true;

                // Body Centered
                centeredBody = new GUIStyle(body);
                centeredBody.alignment = TextAnchor.UpperCenter;

                // Body Centered
                imgDescriptionBold = new GUIStyle(centeredBody);
                imgDescriptionBold.fontStyle = FontStyle.Bold;

                // Heading
                heading = new GUIStyle(body);
                heading.fontStyle = FontStyle.Bold;

                // Title
                title = new GUIStyle(body);
                title.fontStyle = FontStyle.Bold;
                title.fontSize = 17;

                // Wrap
                wrap = new GUIStyle(GUI.skin.label);
                wrap.fontStyle = FontStyle.Normal;
                wrap.wordWrap = true;

                // Text Area
                editWrap = new GUIStyle(GUI.skin.textArea);
                editWrap.wordWrap = true;

                // Link
                link = new GUIStyle(GUI.skin.label);
                link.name = "link";
                link.fontStyle = FontStyle.Bold;
                link.stretchWidth = false;
                link.wordWrap = false;

                // Clickable Image
                clickImg = new GUIStyle(body);

                // Help
                helpBox = new GUIStyle(GUI.skin.box);
                helpBox.margin = new RectOffset(0, 0, 0, 0);
                helpBox.padding = new RectOffset(5, 5, 5, 5);
                helpBox.alignment = TextAnchor.UpperLeft;
                helpBox.stretchWidth = true;
                //helpBox.wordWrap = true;
                help = new GUIStyle(GUI.skin.label);
                help.alignment = TextAnchor.UpperLeft;
                help.stretchWidth = true;
                help.richText = true;
                help.wordWrap = true;

                // Gridlist
                gridList = new GUIStyle("GridList");


                // Setup colors for Unity Pro
                if (EditorGUIUtility.isProSkin)
                {
                    // Tabs
                    tabsFrame.normal.background = Resources.Load("pwtabsBorderp" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    tab.normal.background = Resources.Load("pwtabp" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    tab.normal.textColor = GetColorFromHTML("#afafafdc");
                    tabSelected.normal.background = Resources.Load("pwtabap" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    tabSelected.normal.textColor = GetColorFromHTML("#cfcfcfdc");

                    // Panel
                    panelFrame.normal.background = GetBGTexture(new Color(0.267f, 0.267f, 0.267f), new Color(0.219f, 0.219f, 0.219f));
                    panel.normal.background = GetBGTexture(new Color(0.267f, 0.267f, 0.267f));

                    // Box
                    darkBox.normal.background = Resources.Load("pwdarkBoxp" + PWConst.VERSION_IN_FILENAMES) as Texture2D;

                    // Link
                    link.normal.textColor = new Color(0.251f, 0.392f, 1f);

                    // Help
                    //m_helpBG = GetEmbossedBGTexture(new Color(0.26f, 0.26f, 0.3f), new Color(0.635f, 0.635f, 0.635f));
                    helpBox.normal.background = GetEmbossedBGTexture(GetColorFromHTML("313138ff"), GetColorFromHTML("5A5A5Aff"));
                    helpBox.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

                    // Click images
                    clickImg.normal.background = GetEmbossedBGTexture(GetColorFromHTML("383838ff"), GetColorFromHTML("5A5A5Aff"));
                    clickImg.hover.background = GetEmbossedBGTexture(GetColorFromHTML("3838aaff"), GetColorFromHTML("3838aaff"), false);

                    // Help Toggle
                    helpOff = Resources.Load("helpBtnOffp" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    helpOn = Resources.Load("helpBtnOnp" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                }
                // or Unity Personal
                else
                {
                    // Tabs
                    tabsFrame.normal.background = Resources.Load("pwtabsBorder" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    tab.normal.background = Resources.Load("pwtab" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    tab.normal.textColor = GetColorFromHTML("#ffffffdc");
                    tabSelected.normal.background = Resources.Load("pwtaba" + PWConst.VERSION_IN_FILENAMES) as Texture2D;

                    // Panel
                    panelFrame.normal.background = GetBGTexture(new Color(0.80f, 0.805f, 0.81f), new Color(0f, 0f, 0f));
                    panel.normal.background = GetBGTexture(new Color(0.80f, 0.805f, 0.81f));
                    //panelFrame.normal.background = GetBGTexture(new Color(0.73f, 0.73f, 0.73f), new Color(0f, 0f, 0f));
                    //panel.normal.background = GetBGTexture(new Color(0.73f, 0.73f, 0.73f));

                    // Box
                    darkBox.normal.background = Resources.Load("pwdarkBox" + PWConst.VERSION_IN_FILENAMES) as Texture2D;

                    // Link
                    link.normal.textColor = new Color(0.114f, 0.259f, 0.859f);

                    // Help
                    helpBox.normal.background = GetEmbossedBGTexture(new Color(0.71f, 0.71f, 0.76f), new Color(0.635f, 0.635f, 0.635f));
                    helpBox.normal.textColor = new Color(0.3f, 0.3f, 0.3f);

                    // Click images
                    clickImg.normal.background = GetEmbossedBGTexture(new Color(0.761f, 0.761f, 0.761f), new Color(0.635f, 0.635f, 0.635f));
                    clickImg.hover.background = GetEmbossedBGTexture(new Color(0.3f, 0.3f, 0.761f), new Color(0.3f, 0.3f, 0.761f), false);

                    // Help Toggle
                    helpOff = Resources.Load("helpBtnOff" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                    helpOn = Resources.Load("helpBtnOn" + PWConst.VERSION_IN_FILENAMES) as Texture2D;
                }
            }

            /// <summary>
            /// Tidy things up
            /// </summary>
            public void Dispose()
            {
                for (int i = 0; i < m_texturesInMemory.Count; i++)
                {
                    UnityEngine.Object.DestroyImmediate(m_texturesInMemory[i]);
                }
            }

            /// <summary>
            /// Get a color from a html string
            /// </summary>
            /// <param name="htmlString">Color in RRGGBB or RRGGBBBAA or #RRGGBB or #RRGGBBAA format.</param>
            /// <returns>Color or white if unable to parse it.</returns>
            public static Color GetColorFromHTML(string htmlString)
            {
                Color color = Color.white;
                if (!htmlString.StartsWith("#"))
                {
                    htmlString = "#" + htmlString;
                }
                if (!ColorUtility.TryParseHtmlString(htmlString, out color))
                {
                    color = Color.white;
                }
                return color;
            }

            protected Texture2D GetBGTexture(Color backgroundColor)
            {
                int res = 1;

                Color[] colors = new Color[res * res];

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = backgroundColor;
                }

                Texture2D tex = new Texture2D(res, res);
                tex.SetPixels(colors);
                tex.Apply(true);
                m_texturesInMemory.Add(tex);

                return tex;
            }

            protected Texture2D GetBGTexture(Color backgroundColor, Color borderColor)
            {
                int res = 6;

                Color[] colors = new Color[res * res];

                for (int x = 0; x < res; x++)
                {
                    for (int y = 0; y < res; y++)
                    {
                        int i = x * res + y;

                        if (x == 0 || x == res - 1 || y == 0 || y == res - 1)
                        {
                            // Apply the border color
                            colors[i] = borderColor;
                        }
                        else
                        {
                            // Apply the background color
                            colors[i] = backgroundColor;
                        }
                    }
                }

                Texture2D tex = new Texture2D(res, res);
                tex.SetPixels(colors);
                tex.Apply(true);
                m_texturesInMemory.Add(tex);

                return tex;
            }

            protected Texture2D GetEmbossedBGTexture(Color backgroundColor, Color borderColor, bool pressed = true)
            {
                int res = 120;
                Color bgShadow = new Color(
                    backgroundColor.r * 0.8f,
                    backgroundColor.g * 0.8f,
                    backgroundColor.b * 0.8f);
                Color bgSpecular = new Color(
                    backgroundColor.r * 1.2f,
                    backgroundColor.g * 1.2f,
                    backgroundColor.b * 1.2f);
                Color borderShadow = new Color(
                    borderColor.r * 0.6f,
                    borderColor.g * 0.6f,
                    borderColor.b * 0.6f);

                Color[] colors = new Color[res * res];

                for (int x = 2; x < res - 2; x++)
                {
                    for (int y = 2; y < res - 2; y++)
                    {
                        int i = x * res + y;

                        // Apply the background color
                        colors[i] = backgroundColor;
                    }
                }

                if (pressed)
                {
                    for (int x = 0; x < res; x++)
                    {
                        colors[x] = borderColor;
                        colors[x + res] = bgSpecular;

                        colors[(res - 2) * res + x] = bgShadow;
                    }
                }
                else
                {
                    for (int x = 0; x < res; x++)
                    {
                        colors[x + res] = bgShadow;

                        colors[(res - 1) * res + x] = borderColor;
                        colors[(res - 2) * res + x] = bgSpecular;
                    }
                }

                for (int y = 0; y < res; y++)
                {
                    colors[y * res] = borderColor;
                    colors[y * res + 1] = bgShadow;

                    colors[y * res + res - 1] = borderColor;
                    colors[y * res + res - 2] = bgShadow;
                }

                if (pressed)
                {
                    for (int x = 0; x < res; x++)
                    {
                        colors[(res - 1) * res + x] = borderShadow;
                    }
                }
                else
                {
                    for (int x = 0; x < res; x++)
                    {
                        colors[x] = borderShadow;
                    }
                }

                Texture2D tex = new Texture2D(res, res);
                tex.SetPixels(colors);
                tex.Apply(true);
                m_texturesInMemory.Add(tex);

                return tex;
            }
        }
    }
}
