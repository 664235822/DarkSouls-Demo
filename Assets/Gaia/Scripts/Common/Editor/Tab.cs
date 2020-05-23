// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System;

namespace GaiaCommon1
{
    /// <summary>
    /// A simple tab Object
    /// </summary>
    public struct Tab
    {
        /// <summary>
        /// The localization key for the label of the tab
        /// </summary>
        public string Key;

        /// <summary>
        /// The label of the tab. This can be text or image
        /// </summary>
        public GUIContent Label;

        /// <summary>
        /// The icon of the tab. An image
        /// </summary>
        public Texture2D Icon;

        /// <summary>
        /// The method that implements the Tabs content
        /// </summary>
        public Action TabMethod;

        /// <summary>
        /// Does this tab have a localized label
        /// </summary>
        public bool HasLocalizedLabel { get { return !string.IsNullOrEmpty(Key); } }

        /// <summary>
        /// Does this tab have a localized label
        /// </summary>
        public bool HasIcon { get { return Icon != null; } }

        /// <summary>
        /// Scroll position of the tab
        /// </summary>
        public Vector2 ScrollPosition;

        /// <summary>
        /// Create a Non Localized tab Object
        /// </summary>
        /// <param name="label">The label of the tab. This can be text or image</param>
        /// <param name="tabMethod">The method that implements the Tabs content</param>
        public Tab(GUIContent label, Action tabMethod)
        {
            Key = null;
            Icon = null;
            Label = label;
            TabMethod = tabMethod;
            ScrollPosition = Vector2.zero;
        }

        /// <summary>
        /// Create a Localized tab Object
        /// </summary>
        /// <param name="key">The localization key for the label of the tab</param>
        /// <param name="tabMethod">The method that implements the Tabs content</param>
        public Tab(string key, Action tabMethod)
        {
            Key = key;
            Icon = null;
            Label = null;
            TabMethod = tabMethod;
            ScrollPosition = Vector2.zero;
        }

        /// <summary>
        /// Create a Localized tab Object
        /// </summary>
        /// <param name="key">The localization key for the label of the tab</param>
        /// <param name="ico">Icon for the tab</param>
        /// <param name="tabMethod">The method that implements the Tabs content</param>
        public Tab(string key, Texture2D icon, Action tabMethod)
        {
            Key = key;
            Icon = icon;
            Label = null;
            TabMethod = tabMethod;
            ScrollPosition = Vector2.zero;
        }
    }
}
