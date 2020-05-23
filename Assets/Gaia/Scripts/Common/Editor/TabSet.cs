// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System.Collections.Generic;
using System;

namespace GaiaCommon1
{
    /// <summary>
    /// A collection of tabs to be used as a <seealso cref="TabBar"/>
    /// </summary>
    public class TabSet
    {
        /// <summary>
        /// Index of the default tab
        /// </summary>
        public int DefaultTabIndex { get; set; }
        /// <summary>
        /// Index of the currently active tab
        /// </summary>
        public int ActiveTabIndex { get; set; }
        /// <summary>
        /// The number of tabs in this TabSet
        /// </summary>
        public int Length { get { return m_tabs.Length; } }
        /// <summary>
        /// Index of the last tab in the TabsSet
        /// </summary>
        public int LastTabIndex { get { return m_tabs.Length - 1; } }

        /// <summary>
        /// The active tab
        /// </summary>
        public Tab ActiveTab { get { return m_tabs[ActiveTabIndex]; } }

        /// <summary>
        /// Scroll position of the active tab
        /// </summary>
        public Vector2 ActiveTabsScroll { get { return m_tabs[ActiveTabIndex].ScrollPosition; } set { m_tabs[ActiveTabIndex].ScrollPosition = value; } }

        /// <summary>
        /// The labels of the tabs in this set as an <see langword="Array"/>
        /// </summary>
        public GUIContent[] Labels { get; private set; }

        private Tab[] m_tabs;
        private EditorUtils m_parentsEditorUtils;

        /// <summary>
        /// Creates a collection of tabs to be used as a <seealso cref="TabBar"/>
        /// </summary>
        /// <param name="parentsEditorUtils">The <see cref="EditorUtils"/> object of the parent editor this TabsSet belongs to</param>
        /// <param name="tabs">A number of tabs that belongs to this set</param>
        public TabSet(EditorUtils parentsEditorUtils, Tab[] tabs) : this(parentsEditorUtils, tabs, 0) { }

        /// <summary>
        /// Creates a collection of tabs to be used as a <seealso cref="TabBar"/>
        /// </summary>
        /// <param name="parentsEditorUtils">The <see cref="EditorUtils"/> object of the parent editor this TabsSet belongs to</param>
        /// <param name="tabs">A number of tabs that belongs to this set</param>
        /// <param name="defaultTab">The default tab to be selected</param>
        public TabSet(EditorUtils parentsEditorUtils, Tab[] tabs, Tab defaultTab) : this(parentsEditorUtils, tabs, (new List<Tab> (tabs).IndexOf(defaultTab))) { }

        /// <summary>
        /// Creates a collection of tabs to be used as a <seealso cref="TabBar"/>
        /// </summary>
        /// <param name="parentsEditorUtils">The <see cref="EditorUtils"/> object of the parent editor this TabsSet belongs to</param>
        /// <param name="tabs">A number of tabs that belongs to this set</param>
        /// <param name="defaultTabIndex">Index of the default tab to be selected</param>
        public TabSet(EditorUtils parentsEditorUtils, Tab[] tabs, int defaultTabIndex)
        {
            if (defaultTabIndex < 0 || defaultTabIndex >= tabs.Length)
            {
                throw new IndexOutOfRangeException("Default tab index out of range: " + defaultTabIndex + ". Tab count: " + tabs.Length);
            }
            if (parentsEditorUtils == null)
            {
                throw new NullReferenceException("EditorUtils is null for Tabs");
            }

            m_parentsEditorUtils = parentsEditorUtils;
            m_tabs = tabs;

            DefaultTabIndex = ActiveTabIndex = defaultTabIndex;

            m_parentsEditorUtils.OnLocalizationUpdate -= UpdateLocalizedLabels;
            m_parentsEditorUtils.OnLocalizationUpdate += UpdateLocalizedLabels;

            UpdateLabels();
        }

        /// <summary>
        /// Update a tab's label with a Localized value
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        /// <param name="key">Localization key</param>
        public void UpdateTab(int index, string key)
        {
            m_tabs[index].Key = key;
            m_tabs[index].Label = null;

            UpdateLabelAt(index);
        }

        /// <summary>
        /// Update a tab's implementation method and its label with a Localized value
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        /// <param name="key">Localization key</param>
        /// <param name="tabMethod">The method that implements the Tabs content</param>
        public void UpdateTab(int index, string key, Action tabMethod)
        {
            m_tabs[index].Key = key;
            m_tabs[index].Label = null;
            m_tabs[index].TabMethod = tabMethod;

            UpdateLabelAt(index);
        }

        /// <summary>
        /// Update a tab's label with a Non Localized value
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        /// <param name="label">GUIContent to use as the tab's label</param>
        public void UpdateTab(int index, GUIContent label)
        {
            m_tabs[index].Key = null;
            m_tabs[index].Label = label;

            UpdateLabelAt(index);
        }

        /// <summary>
        /// Update a tab's implementation method and its label with a Non Localized value
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        /// <param name="label">GUIContent to use as the tab's label</param>
        /// <param name="tabMethod">The method that implements the Tabs content</param>
        public void UpdateTab(int index, GUIContent label, Action tabMethod)
        {
            m_tabs[index].Key = null;
            m_tabs[index].Label = label;
            m_tabs[index].TabMethod = tabMethod;

            UpdateLabelAt(index);
        }

        /// <summary>
        /// Swap a tab out in the set
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        /// <param name="tab">Replacement tab object</param>
        public void SwapTab(int index, Tab tab)
        {
            m_tabs[index] = tab;
            UpdateLabelAt(index);
        }

        /// <summary>
        /// Add a tab to the end of the set
        /// </summary>
        /// <param name="tab">Tab object</param>
        public void AddTab(Tab tab)
        {
            AddTab(m_tabs.Length, tab);
        }

        /// <summary>
        /// Insert a tab into the set - Tip: Use without index if you want to add to the end of the set
        /// </summary>
        /// <param name="index">Index where the tab will be inserted</param>
        /// <param name="tab">Tab object</param>
        public void AddTab(int index, Tab tab)
        {
            if (index < 0 || index > m_tabs.Length)
            {
                throw new IndexOutOfRangeException("Tabs index out of range for adding: " + index + ". Tab count: " + m_tabs.Length);
            }

            List<Tab> newTabs = null;

            // Adding to the end
            if (index == m_tabs.Length)
            {
                newTabs = new List<Tab>(m_tabs);
                newTabs.Add(tab);
            }
            // Or injecting in the middle
            else
            {
                newTabs = new List<Tab>();

                for (int i = 0; i < m_tabs.Length; i++)
                {
                    if (i == index)
                    {
                        newTabs.Add(tab);
                    }

                    newTabs.Add(m_tabs[i]);
                }
            }

            m_tabs = newTabs.ToArray();
            UpdateLabels();
        }

        /// <summary>
        /// Remove a tab at index
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        public void RemoveTab(int index)
        {
            if (index < 0 || index >= m_tabs.Length)
            {
                throw new IndexOutOfRangeException("Tabs index out of range for removing: " + index + ". Tab count: " + m_tabs.Length);
            }

            List<Tab> newTabs = null;

            newTabs = new List<Tab>();

            for (int i = 0; i < m_tabs.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                newTabs.Add(m_tabs[i]);
            }

            m_tabs = newTabs.ToArray();
            UpdateLabels();

        }

        /// <summary>
        /// Update the labels of the tabs. This is typically used after the tabs in the set changed
        /// </summary>
        private void UpdateLabels()
        {
            Labels = new GUIContent[m_tabs.Length];
            for (int i = 0; i < m_tabs.Length; i++)
            {
                if (m_tabs[i].HasLocalizedLabel)
                {
                    if (m_tabs[i].HasIcon)
                    {
                        Labels[i] = new GUIContent(m_parentsEditorUtils.GetContent(m_tabs[i].Key, m_tabs[i].Icon));
                    }
                    else
                    {
                        Labels[i] = m_parentsEditorUtils.GetContent(m_tabs[i].Key);
                    }
                }
                else
                {
                    Labels[i] = m_tabs[i].Label;
                }
            }
        }

        /// <summary>
        /// Updates the labels which are localized. This is automatically called when the localization data changes.
        /// </summary>
        private void UpdateLocalizedLabels()
        {
            for (int i = 0; i < m_tabs.Length; i++)
            {
                if (m_tabs[i].HasLocalizedLabel)
                {
                    if (m_tabs[i].HasIcon)
                    {
                        Labels[i] = new GUIContent(m_parentsEditorUtils.GetContent(m_tabs[i].Key, m_tabs[i].Icon));
                    }
                    else
                    {
                        Labels[i] = m_parentsEditorUtils.GetContent(m_tabs[i].Key);
                    }
                }
            }
        }

        /// <summary>
        /// Update the label of the tab at <paramref name="index"/>
        /// </summary>
        /// <param name="index">Index of the tab in the set</param>
        private void UpdateLabelAt(int index)
        {
            if (m_tabs[index].HasLocalizedLabel)
            {
                Labels[index] = m_parentsEditorUtils.GetContent(m_tabs[index].Key);
            }
            else
            {
                Labels[index] = m_tabs[index].Label;
            }
        }
    }
}
