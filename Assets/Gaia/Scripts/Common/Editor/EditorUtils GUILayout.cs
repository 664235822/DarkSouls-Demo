// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using GaiaCommon1.Localization;

namespace GaiaCommon1
{
    /// <summary>
    /// Handy editor utils - GUILayout Compatibility Layer
    /// </summary>
    public partial class EditorUtils
    {
        /// <summary>
        ///   <para>Make an auto-layout label.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(Texture image, params GUILayoutOption[] options)
        {
            Label(new GUIContent(image), options);
        }

        /// <summary>
        ///   <para>Make an auto-layout label with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            Label(GetContent(key), options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make an auto-layout label.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(string key, params GUILayoutOption[] options)
        {
            Label(GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make an auto-layout label.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout label.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            Label(new GUIContent(image), style, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout label with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(string key, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            Label(GetContent(key), style, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make an auto-layout label.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            Label(GetContent(key), style, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout label.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Label(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, style, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout box.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(Texture image, params GUILayoutOption[] options)
        {
            Box(new GUIContent(image), options);
        }

        /// <summary>
        ///   <para>Make an auto-layout box with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            Box(GetContent(key), options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make an auto-layout box.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(string key, params GUILayoutOption[] options)
        {
            Box(GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make an auto-layout box.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Box(content, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout box.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            Box(new GUIContent(image), style, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout box with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(string key, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            Box(GetContent(key), style, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make an auto-layout box.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            Box(GetContent(key), style, options);
        }

        /// <summary>
        ///   <para>Make an auto-layout box.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Box(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Box(content, style, options);
        }

        /// <summary>
        ///   <para>Make a single press button.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(Texture image, params GUILayoutOption[] options)
        {
            return Button(new GUIContent(image), options);
        }

        /// <summary>
        ///   <para>Make a single press button with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            bool pressed = Button(GetContent(key), options);
            InlineHelp(key, helpSwitch);
            return pressed;
        }

        /// <summary>
        ///   <para>Make a single press button.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(string key, params GUILayoutOption[] options)
        {
            return Button(GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make a single press button.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Button(content, options);
        }

        /// <summary>
        ///   <para>Make a single press button.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            return Button(new GUIContent(image), style, options);
        }

        /// <summary>
        ///   <para>Make a single press button with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(string key, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            bool pressed = Button(GetContent(key), style, options);
            InlineHelp(key, helpSwitch);
            return pressed;
        }

        /// <summary>
        ///   <para>Make a single press button.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            return Button(GetContent(key), style, options);
        }

        /// <summary>
        ///   <para>Make a single press button.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the users clicks the button.</para>
        /// </returns>
        public bool Button(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Button(content, style, options);
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(Texture image, params GUILayoutOption[] options)
        {
            return RepeatButton(new GUIContent(image), options);
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            bool pressed = RepeatButton(GetContent(key), options);
            InlineHelp(key, helpSwitch);
            return pressed;
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(string key, params GUILayoutOption[] options)
        {
            return RepeatButton(GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.RepeatButton(content, options);
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            return RepeatButton(new GUIContent(image), style, options);
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(string key, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            bool pressed = RepeatButton(GetContent(key), style, options);
            InlineHelp(key, helpSwitch);
            return pressed;
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            return RepeatButton(GetContent(key), style, options);
        }

        /// <summary>
        ///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>true when the holds down the mouse.</para>
        /// </returns>
        public bool RepeatButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.RepeatButton(content, style, options);
        }

        /// <summary>
        ///   <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, Texture image, params GUILayoutOption[] options)
        {
            return Toggle(value, new GUIContent(image), options);
        }

        /// <summary>
        ///   <para>Make an on/off toggle button with Automatic Help handling.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            bool ticked = Toggle(value, GetContent(key), options);
            InlineHelp(key, helpSwitch);
            return ticked;
        }

        /// <summary>
        ///   <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, string key, params GUILayoutOption[] options)
        {
            return Toggle(value, GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, options);
        }

        /// <summary>
        ///   <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            return Toggle(value, new GUIContent(image), style, options);
        }

        /// <summary>
        ///   <para>Make an on/off toggle button with Automatic Help handling.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, string key, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            bool ticked = Toggle(value, GetContent(key), style, options);
            InlineHelp(key, helpSwitch);
            return ticked;
        }

        /// <summary>
        ///   <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, string key, GUIStyle style, params GUILayoutOption[] options)
        {
            return Toggle(value, GetContent(key), style, options);
        }

        /// <summary>
        ///   <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="value">Is the button on or off?</param>
        /// <param name="key">Localization key of the content to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The new value of the button.</para>
        /// </returns>
        public bool Toggle(bool value, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, style, options);
        }

        /// <summary>
        ///   <para>Make a toolbar with Automatic Help handling.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int Toolbar(int selected, string[] keys, bool helpSwitch, params GUILayoutOption[] options)
        {
            int sel = Toolbar(selected, GetContent(keys), options);
            InlineHelp(keys, helpSwitch);
            return sel;
        }

        /// <summary>
        ///   <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int Toolbar(int selected, string[] keys, params GUILayoutOption[] options)
        {
            return Toolbar(selected, GetContent(keys), options);
        }

        /// <summary>
        ///   <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int Toolbar(int selected, GUIContent[] content, params GUILayoutOption[] options)
        {
            return GUILayout.Toolbar(selected, content, options);
        }

        /// <summary>
        ///   <para>Make a toolbar with Automatic Help handling.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int Toolbar(int selected, string[] keys, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            int sel = Toolbar(selected, GetContent(keys), style, options);
            InlineHelp(keys, helpSwitch);
            return sel;
        }

        /// <summary>
        ///   <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int Toolbar(int selected, string[] keys, GUIStyle style, params GUILayoutOption[] options)
        {
            return Toolbar(selected, GetContent(keys), style, options);
        }

        /// <summary>
        ///   <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int Toolbar(int selected, GUIContent[] contents, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Toolbar(selected, contents, style, options);
        }

        /// <summary>
        ///   <para>Make a Selection Grid with Automatic Help handling.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int SelectionGrid(int selected, string[] keys, int xCount, bool helpSwitch, params GUILayoutOption[] options)
        {
            int sel = SelectionGrid(selected, GetContent(keys), xCount, options);
            InlineHelp(keys, helpSwitch);
            return sel;
        }

        /// <summary>
        ///   <para>Make a Selection Grid.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int SelectionGrid(int selected, string[] keys, int xCount, params GUILayoutOption[] options)
        {
            return SelectionGrid(selected, GetContent(keys), xCount, options);
        }

        /// <summary>
        ///   <para>Make a Selection Grid.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int SelectionGrid(int selected, GUIContent[] content, int xCount, params GUILayoutOption[] options)
        {
            return GUILayout.SelectionGrid(selected, content, xCount, options);
        }

        /// <summary>
        ///   <para>Make a Selection Grid with Automatic Help handling.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int SelectionGrid(int selected, string[] keys, int xCount, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            int sel = SelectionGrid(selected, GetContent(keys), xCount, style, options);
            InlineHelp(keys, helpSwitch);
            return sel;
        }

        /// <summary>
        ///   <para>Make a Selection Grid.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int SelectionGrid(int selected, string[] keys, int xCount, GUIStyle style, params GUILayoutOption[] options)
        {
            return SelectionGrid(selected, GetContent(keys), xCount, style, options);
        }

        /// <summary>
        ///   <para>Make a Selection Grid.</para>
        /// </summary>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="keys">An array of localization keys for the content to show on the buttons.</param>
        /// <param name="images">An array of textures on the buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="content"></param>
        /// <returns>
        ///   <para>The index of the selected button.</para>
        /// </returns>
        public int SelectionGrid(int selected, GUIContent[] contents, int xCount, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.SelectionGrid(selected, contents, xCount, style, options);
        }
    }
}
