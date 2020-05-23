// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using GaiaCommon1.Localization;

namespace GaiaCommon1
{
    /// <summary>
    /// Handy editor utils - EditorGUILayout Compatibility Layer
    /// </summary>
    public partial class EditorUtils
    {
        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>    
        public bool Foldout(bool foldout, string key, bool helpSwitch)
        {
            var val = EditorGUILayout.Foldout(foldout, GetContent(key), true);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>    
        public bool Foldout(bool foldout, string key)
        {
            return EditorGUILayout.Foldout(foldout, GetContent(key), true);
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, string key, GUIStyle style, bool helpSwitch)
        {
            var val = EditorGUILayout.Foldout(foldout, GetContent(key), true, style);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, string key, GUIStyle style)
        {
            return EditorGUILayout.Foldout(foldout, GetContent(key), true, style);
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, string key, bool toggleOnLabelClick, bool helpSwitch)
        {
            var val = EditorGUILayout.Foldout(foldout, GetContent(key), toggleOnLabelClick);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(string key, bool foldout, bool toggleOnLabelClick)
        {
            return EditorGUILayout.Foldout(foldout, GetContent(key), toggleOnLabelClick);
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(string key, bool foldout, bool toggleOnLabelClick, GUIStyle style, bool helpSwitch)
        {
            var val = EditorGUILayout.Foldout(foldout, GetContent(key), toggleOnLabelClick, style);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="key">Localization key of the content to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, string key, bool toggleOnLabelClick, GUIStyle style)
        {
            return EditorGUILayout.Foldout(foldout, GetContent(key), toggleOnLabelClick, style);
        }

        /// <summary>
        ///   <para>Make a NON LOCALIZED label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, GUIContent content)
        {
            return EditorGUILayout.Foldout(foldout, content, true);
        }

        /// <summary>
        ///   <para>Make a NON LOCALIZED label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, GUIContent content, GUIStyle style)
        {
            return EditorGUILayout.Foldout(foldout, content, true, style);
        }

        /// <summary>
        ///   <para>Make a NON LOCALIZED label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick)
        {
            return EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick);
        }

        /// <summary>
        ///   <para>Make a NON LOCALIZED label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        ///   <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
        {
            return EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick, style);
        }

        /// <summary>
        ///   <para>Make a label in front of some control with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        public void PrefixLabel(string key, bool helpSwitch)
        {
            EditorGUILayout.PrefixLabel(GetContent(key));
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public void PrefixLabel(string key)
        {
            EditorGUILayout.PrefixLabel(GetContent(key));
        }

        /// <summary>
        ///   <para>Make a label in front of some control with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        public void PrefixLabel(string key, GUIStyle followingStyle, bool helpSwitch)
        {
            EditorGUILayout.PrefixLabel(GetContent(key), followingStyle);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public void PrefixLabel(string key, GUIStyle followingStyle)
        {
            EditorGUILayout.PrefixLabel(GetContent(key), followingStyle);
        }

        /// <summary>
        ///   <para>Make a label in front of some control with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        public void PrefixLabel(string key, GUIStyle followingStyle, GUIStyle labelStyle, bool helpSwitch)
        {
            EditorGUILayout.PrefixLabel(GetContent(key), followingStyle, labelStyle);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public void PrefixLabel(string key, GUIStyle followingStyle, GUIStyle labelStyle)
        {
            EditorGUILayout.PrefixLabel(GetContent(key), followingStyle, labelStyle);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info. with Automatic Help handling)</para>
        /// </summary>
        /// <param name="key">Localization key of the content in front of the label field.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="key">Localization key of the content in front of the label field.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info. with Automatic Help handling)</para>
        /// </summary>
        /// <param name="key">Localization key of the content in front of the label field.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), style, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="key">Localization key of the content in front of the label field.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), style, options);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info. with Automatic Help handling)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="key2">Localization key of the label to show to the right.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, string key2, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), GetContent(key2), options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="key2">Localization key of the label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, string key2, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), GetContent(key2), options);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info. with Automatic Help handling)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="key2">Localization key of the label to show to the right.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, string key2, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), GetContent(key2), style, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="key2">Localization key of the label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, string key2, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), GetContent(key2), style, options);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info. with Automatic Help handling)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="content">The content (NON LOCALIZED) of the label to show to the right.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, GUIContent content, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), content, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="content">The content (NON LOCALIZED) of the label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, GUIContent content, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), content, options);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info. with Automatic Help handling)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="content">The content (NON LOCALIZED) of the label to show to the right.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, GUIContent content, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), content, style, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the label field.</param>
        /// <param name="content">The content (NON LOCALIZED) of the label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public void LabelField(string key, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(GetContent(key), content, style, options);
        }

        /// <summary>
        ///   <para>Make a toggle with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        ///         properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The selected state of the toggle.</para>
        /// </returns>
        public bool Toggle(string key, bool value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Toggle(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a toggle.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        ///         properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The selected state of the toggle.</para>
        /// </returns>
        public bool Toggle(string key, bool value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Toggle(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a toggle with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        ///         properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The selected state of the toggle.</para>
        /// </returns>
        public bool Toggle(string key, bool value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Toggle(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a toggle.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        ///         properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The selected state of the toggle.</para>
        /// </returns>
        public bool Toggle(string key, bool value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Toggle(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public bool ToggleLeft(string key, bool value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = ToggleLeft(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public bool ToggleLeft(string key, bool value, params GUILayoutOption[] options)
        {
            return ToggleLeft(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public bool ToggleLeft(string key, bool value, GUIStyle labelStyle, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = ToggleLeft(GetContent(key), value, labelStyle, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public bool ToggleLeft(string key, bool value, GUIStyle labelStyle, params GUILayoutOption[] options)
        {
            return ToggleLeft(GetContent(key), value, labelStyle, options);
        }

        /// <summary>
        ///   <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it with Automatic Help handling.</para>
        /// </summary>
        /// <param name="label">Label for the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public bool ToggleLeft(GUIContent label, bool value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.ToggleLeft(label, value, options);
        }

        /// <summary>
        ///   <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="label">Label for the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public bool ToggleLeft(GUIContent label, bool value, GUIStyle labelStyle, params GUILayoutOption[] options)
        {
            return EditorGUILayout.ToggleLeft(label, value, labelStyle, options);
        }

        /// <summary>
        ///   <para>Make a text field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The text entered by the user.</para>
        /// </returns>
        public string TextField(string key, string text, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.TextField(GetContent(key), text, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The text entered by the user.</para>
        /// </returns>
        public string TextField(string key, string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(GetContent(key), text, options);
        }

        /// <summary>
        ///   <para>Make a text field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The text entered by the user.</para>
        /// </returns>
        public string TextField(string key, string text, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.TextField(GetContent(key), text, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The text entered by the user.</para>
        /// </returns>
        public string TextField(string key, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(GetContent(key), text, style, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public string DelayedTextField(string key, string text, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedTextField(GetContent(key), text, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public string DelayedTextField(string key, string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedTextField(GetContent(key), text, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public string DelayedTextField(string key, string text, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedTextField(GetContent(key), text, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public string DelayedTextField(string key, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedTextField(GetContent(key), text, style, options);
        }

        /// <summary>
        ///   <para>Make a text field where the user can enter a password with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The password entered by the user.</para>
        /// </returns>
        public string PasswordField(string key, string password, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.PasswordField(GetContent(key), password, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The password entered by the user.</para>
        /// </returns>
        public string PasswordField(string key, string password, params GUILayoutOption[] options)
        {
            return EditorGUILayout.PasswordField(GetContent(key), password, options);
        }

        /// <summary>
        ///   <para>Make a text field where the user can enter a password with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The password entered by the user.</para>
        /// </returns>
        public string PasswordField(string key, string password, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.PasswordField(GetContent(key), password, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The password entered by the user.</para>
        /// </returns>
        public string PasswordField(string key, string password, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.PasswordField(GetContent(key), password, style, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering float values with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public float FloatField(string key, float value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.FloatField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public float FloatField(string key, float value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.FloatField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering float values with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public float FloatField(string key, float value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.FloatField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public float FloatField(string key, float value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.FloatField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering floats with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public float DelayedFloatField(string key, float value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedFloatField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public float DelayedFloatField(string key, float value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedFloatField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering floats with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public float DelayedFloatField(string key, float value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedFloatField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public float DelayedFloatField(string key, float value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedFloatField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering double values with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public double DoubleField(string key, double value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DoubleField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public double DoubleField(string key, double value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DoubleField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering double values with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public double DoubleField(string key, double value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DoubleField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public double DoubleField(string key, double value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DoubleField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering doubles with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">
        ///   An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///   See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///   GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public double DelayedDoubleField(string key, double value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedDoubleField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        ///   An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///   See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///   GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public double DelayedDoubleField(string key, double value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedDoubleField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering doubles with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">
        ///   An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///   See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///   GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public double DelayedDoubleField(string key, double value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedDoubleField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        ///   An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///   See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///   GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public double DelayedDoubleField(string key, double value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedDoubleField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering integers with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public int IntField(string key, int value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.IntField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public int IntField(string key, int value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering integers with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public int IntField(string key, int value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.IntField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public int IntField(string key, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering integers with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public int DelayedIntField(string key, int value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedIntField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public int DelayedIntField(string key, int value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedIntField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering integers with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public int DelayedIntField(string key, int value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.DelayedIntField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public int DelayedIntField(string key, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedIntField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering long integers with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public long LongField(string key, long value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.LongField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public long LongField(string key, long value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.LongField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a text field for entering long integers with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public long LongField(string key, long value, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.LongField(GetContent(key), value, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public long LongField(string key, long value, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.LongField(GetContent(key), value, style, options);
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change a value between a min and a max with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value that has been set by the user.</para>
        /// </returns>
        public float Slider(string key, float value, float leftValue, float rightValue, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Slider(GetContent(key), value, leftValue, rightValue, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value that has been set by the user.</para>
        /// </returns>
        public float Slider(string key, float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Slider(GetContent(key), value, leftValue, rightValue, options);
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change a value between a min and a max with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Slider(SerializedProperty property, float leftValue, float rightValue, string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.Slider(property, leftValue, rightValue, GetContent(key), options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void Slider(SerializedProperty property, float leftValue, float rightValue, string key, params GUILayoutOption[] options)
        {
            EditorGUILayout.Slider(property, leftValue, rightValue, GetContent(key), options);
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change an integer value between a min and a max with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value that has been set by the user.</para>
        /// </returns>
        public int IntSlider(string key, int value, int leftValue, int rightValue, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.IntSlider(GetContent(key), value, leftValue, rightValue, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value that has been set by the user.</para>
        /// </returns>
        public int IntSlider(string key, int value, int leftValue, int rightValue, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntSlider(GetContent(key), value, leftValue, rightValue, options);
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change an integer value between a min and a max with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void IntSlider(SerializedProperty property, int leftValue, int rightValue, string key, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.IntSlider(property, leftValue, rightValue, GetContent(key), options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        ///   <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void IntSlider(SerializedProperty property, int leftValue, int rightValue, string key, params GUILayoutOption[] options)
        {
            EditorGUILayout.IntSlider(property, leftValue, rightValue, GetContent(key), options);
        }

        /// <summary>
        /// Make a slider the user can drag to change a range (min/max value) between a min and a max.
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="minValue">The left/minimum value the user can set.</param>
        /// <param name="maxValue">The right/maximum value the user can set.</param>
        /// <param name="minLimit">The value at the left end of the slider.</param>
        /// <param name="maxLimit">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void MinMaxSlider(string key, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool helpSwitch, params GUILayoutOption[] options)
        {
            EditorGUILayout.MinMaxSlider(GetContent(key), ref minValue, ref maxValue, minLimit, maxLimit, options);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        /// Make a slider the user can drag to change a range (min/max value) between a min and a max.
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="minValue">The left/minimum value the user can set.</param>
        /// <param name="maxValue">The right/maximum value the user can set.</param>
        /// <param name="minLimit">The value at the left end of the slider.</param>
        /// <param name="maxLimit">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void MinMaxSlider(string key, ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            EditorGUILayout.MinMaxSlider(GetContent(key), ref minValue, ref maxValue, minLimit, maxLimit, options);
        }

        /// <summary>
        /// Make a slider the user can drag or type values in fields to change a range (min/max value) between a min and a max.
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="minValue">The left/minimum value the user can set.</param>
        /// <param name="maxValue">The right/maximum value the user can set.</param>
        /// <param name="minLimit">The value at the left end of the slider.</param>
        /// <param name="maxLimit">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        public void MinMaxSliderWithFields(string key, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool helpSwitch)
        {
            MinMaxSliderWithFields(key, ref minValue, ref maxValue, minLimit, maxLimit);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        /// Make a slider the user can drag or type values in fields to change a range (min/max value) between a min and a max.
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="minValue">The left/minimum value the user can set.</param>
        /// <param name="maxValue">The right/maximum value the user can set.</param>
        /// <param name="minLimit">The value at the left end of the slider.</param>
        /// <param name="maxLimit">The value at the right end of the slider.</param>
        public void MinMaxSliderWithFields(string key, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, GetContent(key));

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float min = minValue;
            float max = maxValue;

            min = Mathf.Clamp(EditorGUI.DelayedFloatField(new Rect(rect.x, rect.y, EditorGUIUtility.fieldWidth, rect.height), min), minLimit, maxLimit);
            if (min != minValue && max < min)
            {
                max = min;
            }

            EditorGUI.MinMaxSlider(new Rect(rect.x + EditorGUIUtility.fieldWidth + 5f, rect.y, rect.width - 10f - 2 * EditorGUIUtility.fieldWidth, rect.height), ref min, ref max, minLimit, maxLimit);

            max = Mathf.Clamp(EditorGUI.DelayedFloatField(new Rect(rect.xMax - EditorGUIUtility.fieldWidth, rect.y, EditorGUIUtility.fieldWidth, rect.height), max), minLimit, maxLimit);
            if (max != maxValue && max < min)
            {
                min = max;
            }

            minValue = min;
            maxValue = max;

            EditorGUI.indentLevel = indent;
        }

        /// <summary>
        /// Make a slider the user can drag or type values in fields to change a range (min/max value) between a min and a max.
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="minValue">The left/minimum value the user can set.</param>
        /// <param name="maxValue">The right/maximum value the user can set.</param>
        /// <param name="minLimit">The value at the left end of the slider.</param>
        /// <param name="maxLimit">The value at the right end of the slider.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        public void MinMaxSliderWithFields(string key, ref int minValue, ref int maxValue, int minLimit, int maxLimit, bool helpSwitch)
        {
            MinMaxSliderWithFields(key, ref minValue, ref maxValue, minLimit, maxLimit);
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        /// Make a slider the user can drag or type values in fields to change a range (min/max value) between a min and a max.
        /// </summary>
        /// <param name="key">Localization key of the label in front of the slider.</param>
        /// <param name="minValue">The left/minimum value the user can set.</param>
        /// <param name="maxValue">The right/maximum value the user can set.</param>
        /// <param name="minLimit">The value at the left end of the slider.</param>
        /// <param name="maxLimit">The value at the right end of the slider.</param>
        public void MinMaxSliderWithFields(string key, ref int minValue, ref int maxValue, int minLimit, int maxLimit)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, GetContent(key));

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int min = minValue;
            int max = maxValue;

            min = Mathf.Clamp(EditorGUI.DelayedIntField(new Rect(rect.x, rect.y, EditorGUIUtility.fieldWidth, rect.height), minValue), minLimit, maxLimit);
            if (min != minValue && max < min)
            {
                max = min;
            }

            float minValF = min;
            float maxValF = max;
            EditorGUI.MinMaxSlider(new Rect(rect.x + EditorGUIUtility.fieldWidth + 5f, rect.y, rect.width - 10f - 2 * EditorGUIUtility.fieldWidth, rect.height), ref minValF, ref maxValF, minLimit, maxLimit);
            minValue = min = Mathf.RoundToInt(minValF);
            maxValue = max = Mathf.RoundToInt(maxValF);

            max = Mathf.Clamp(EditorGUI.DelayedIntField(new Rect(rect.xMax - EditorGUIUtility.fieldWidth, rect.y, EditorGUIUtility.fieldWidth, rect.height), maxValue), minLimit, maxLimit);
            if (max != maxValue && max < min)
            {
                min = max;
            }

            minValue = min;
            maxValue = max;

            EditorGUI.indentLevel = indent;
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, string[] optionsKeys, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Popup(GetContent(key), selectedIndex, GetContent(optionsKeys), options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, string[] optionsKeys, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(GetContent(key), selectedIndex, GetContent(optionsKeys), options);
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, string[] optionsKeys, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Popup(GetContent(key), selectedIndex, GetContent(optionsKeys), style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, string[] optionsKeys, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(GetContent(key), selectedIndex, GetContent(optionsKeys), style, options);
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="options">An array of GUIContent: the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, GUIContent[] optionsKeys, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Popup(GetContent(key), selectedIndex, optionsKeys, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="options">An array of GUIContent: the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, GUIContent[] optionsKeys, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(GetContent(key), selectedIndex, optionsKeys, options);
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="options">An array of GUIContent: the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, GUIContent[] optionsKeys, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Popup(GetContent(key), selectedIndex, optionsKeys, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a generic popup selection field with localized label and options.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="options">An array of GUIContent: the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public int Popup(string key, int selectedIndex, GUIContent[] optionsKeys, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(GetContent(key), selectedIndex, optionsKeys, style, options);
        }

        /// <summary>
        ///   <para>Make an enum popup selection field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public Enum EnumPopup(string key, Enum selected, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.EnumPopup(GetContent(key), selected, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public Enum EnumPopup(string key, Enum selected, params GUILayoutOption[] options)
        {
            return EditorGUILayout.EnumPopup(GetContent(key), selected, options);
        }

        /// <summary>
        ///   <para>Make an enum popup selection field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public Enum EnumPopup(string key, Enum selected, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.EnumPopup(GetContent(key), selected, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public Enum EnumPopup(string key, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.EnumPopup(GetContent(key), selected, style, options);
        }

        ///// <summary>
        /////   <para>Make an enum popup selection field for a bitmask with Automatic Help handling.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the field.</param>
        ///// <param name="selected">The enum options the field shows.</param>
        ///// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        ///// <param name="options">Optional layout options.</param>
        ///// <param name="style">Optional GUIStyle.</param>
        ///// <returns>
        /////   <para>The enum options that has been selected by the user.</para>
        ///// </returns>
        //public Enum EnumMaskPopup(string key, Enum selected, bool helpSwitch, params GUILayoutOption[] options)
        //{
        //    var val = EditorGUILayout.EnumMaskPopup(GetContent(key), selected, options);
        //    InlineHelp(key, helpSwitch);
        //    return val;
        //}

        ///// <summary>
        /////   <para>Make an enum popup selection field for a bitmask.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the field.</param>
        ///// <param name="selected">The enum options the field shows.</param>
        ///// <param name="options">Optional layout options.</param>
        ///// <param name="style">Optional GUIStyle.</param>
        ///// <returns>
        /////   <para>The enum options that has been selected by the user.</para>
        ///// </returns>
        //public Enum EnumMaskPopup(string key, Enum selected, params GUILayoutOption[] options)
        //{
        //    return EditorGUILayout.EnumMaskPopup(GetContent(key), selected, options);
        //}

        ///// <summary>
        /////   <para>Make an enum popup selection field for a bitmask with Automatic Help handling.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the field.</param>
        ///// <param name="selected">The enum options the field shows.</param>
        ///// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        ///// <param name="options">Optional layout options.</param>
        ///// <param name="style">Optional GUIStyle.</param>
        ///// <returns>
        /////   <para>The enum options that has been selected by the user.</para>
        ///// </returns>
        //public Enum EnumMaskPopup(string key, Enum selected, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        //{
        //    var val = EditorGUILayout.EnumMaskPopup(GetContent(key), selected, style, options);
        //    InlineHelp(key, helpSwitch);
        //    return val;
        //}

        ///// <summary>
        /////   <para>Make an enum popup selection field for a bitmask.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the field.</param>
        ///// <param name="selected">The enum options the field shows.</param>
        ///// <param name="options">Optional layout options.</param>
        ///// <param name="style">Optional GUIStyle.</param>
        ///// <returns>
        /////   <para>The enum options that has been selected by the user.</para>
        ///// </returns>
        //public Enum EnumMaskPopup(string key, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        //{
        //    return EditorGUILayout.EnumMaskPopup(GetContent(key), selected, style, options);
        //}

        /// <summary>
        ///   <para>Make an integer popup selection field with localized label and options with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public int IntPopup(string key, int selectedValue, string[] optionsKeys, int[] optionValues, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.IntPopup(GetContent(key), selectedValue, GetContent(optionsKeys), optionValues, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an integer popup selection field with localized label and options.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public int IntPopup(string key, int selectedValue, string[] optionsKeys, int[] optionValues, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntPopup(GetContent(key), selectedValue, GetContent(optionsKeys), optionValues, options);
        }

        /// <summary>
        ///   <para>Make an integer popup selection field with localized label and options with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public int IntPopup(string key, int selectedValue, string[] optionsKeys, int[] optionValues, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.IntPopup(GetContent(key), selectedValue, GetContent(optionsKeys), optionValues, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an integer popup selection field with localized label and options.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="optionsKeys">An array of Localization key of the options shown in the popup.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public int IntPopup(string key, int selectedValue, string[] optionsKeys, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntPopup(GetContent(key), selectedValue, GetContent(optionsKeys), optionValues, style, options);
        }

        /// <summary>
        ///   <para>Make a tag selection field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The tag selected by the user.</para>
        /// </returns>
        public string TagField(string key, string tag, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.TagField(GetContent(key), tag, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The tag selected by the user.</para>
        /// </returns>
        public string TagField(string key, string tag, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TagField(GetContent(key), tag, options);
        }

        /// <summary>
        ///   <para>Make a tag selection field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The tag selected by the user.</para>
        /// </returns>
        public string TagField(string key, string tag, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.TagField(GetContent(key), tag, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The tag selected by the user.</para>
        /// </returns>
        public string TagField(string key, string tag, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TagField(GetContent(key), tag, style, options);
        }

        /// <summary>
        ///   <para>Make a layer selection field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The layer selected by the user.</para>
        /// </returns>
        public int LayerField(string key, int layer, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.LayerField(GetContent(key), layer, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The layer selected by the user.</para>
        /// </returns>
        public int LayerField(string key, int layer, params GUILayoutOption[] options)
        {
            return EditorGUILayout.LayerField(GetContent(key), layer, options);
        }

        /// <summary>
        ///   <para>Make a layer selection field with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The layer selected by the user.</para>
        /// </returns>
        public int LayerField(string key, int layer, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.LayerField(GetContent(key), layer, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The layer selected by the user.</para>
        /// </returns>
        public int LayerField(string key, int layer, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.LayerField(GetContent(key), layer, style, options);
        }

        /// <summary>
        ///   <para>Make a field for masks with Automatic Help handling.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        ///   <para>The value modified by the user.</para>
        /// </returns>
        public int MaskField(string key, int mask, string[] displayedOptions, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.MaskField(GetContent(key), mask, displayedOptions, style, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        ///   <para>The value modified by the user.</para>
        /// </returns>
        public int MaskField(string key, int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            return EditorGUILayout.MaskField(GetContent(key), mask, displayedOptions, style, options);
        }

        /// <summary>
        ///   <para>Make a field for masks with Automatic Help handling.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        ///   <para>The value modified by the user.</para>
        /// </returns>
        public int MaskField(string key, int mask, string[] displayedOptions, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.MaskField(GetContent(key), mask, displayedOptions, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        ///   <para>The value modified by the user.</para>
        /// </returns>
        public int MaskField(string key, int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            return EditorGUILayout.MaskField(GetContent(key), mask, displayedOptions, options);
        }

        ///// <summary>
        /////   <para>Make a field for enum based masks with Automatic Help handling.</para>
        ///// </summary>
        ///// <param name="label">Prefix label for this field.</param>
        ///// <param name="enumValue">Enum to use for the flags.</param>
        ///// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <param name="style"></param>
        ///// <returns>
        /////   <para>The value modified by the user.</para>
        ///// </returns>
        //public Enum EnumMaskField(string key, Enum enumValue, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        //{
        //    var val = EditorGUILayout.EnumMaskField(GetContent(key), enumValue, style, options);
        //    InlineHelp(key, helpSwitch);
        //    return val;
        //}

        ///// <summary>
        /////   <para>Make a field for enum based masks.</para>
        ///// </summary>
        ///// <param name="label">Prefix label for this field.</param>
        ///// <param name="enumValue">Enum to use for the flags.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <param name="style"></param>
        ///// <returns>
        /////   <para>The value modified by the user.</para>
        ///// </returns>
        //public Enum EnumMaskField(string key, Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
        //{
        //    return EditorGUILayout.EnumMaskField(GetContent(key), enumValue, style, options);
        //}

        ///// <summary>
        /////   <para>Make a field for enum based masks with Automatic Help handling.</para>
        ///// </summary>
        ///// <param name="label">Prefix label for this field.</param>
        ///// <param name="enumValue">Enum to use for the flags.</param>
        ///// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <param name="style"></param>
        ///// <returns>
        /////   <para>The value modified by the user.</para>
        ///// </returns>
        //public Enum EnumMaskField(string key, Enum enumValue, bool helpSwitch, params GUILayoutOption[] options)
        //{
        //    var val = EditorGUILayout.EnumMaskField(GetContent(key), enumValue, options);
        //    InlineHelp(key, helpSwitch);
        //    return val;
        //}

        ///// <summary>
        /////   <para>Make a field for enum based masks.</para>
        ///// </summary>
        ///// <param name="label">Prefix label for this field.</param>
        ///// <param name="enumValue">Enum to use for the flags.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <param name="style"></param>
        ///// <returns>
        /////   <para>The value modified by the user.</para>
        ///// </returns>
        //public Enum EnumMaskField(string key, Enum enumValue, params GUILayoutOption[] options)
        //{
        //    return EditorGUILayout.EnumMaskField(GetContent(key), enumValue, options);
        //}

        /// <summary>
        ///   <para>Make a field to receive any object type with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="allowSceneObjects">Allow assigning scene objects. See Description for more info.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The object that has been set by the user.</para>
        /// </returns>
        public UnityEngine.Object ObjectField(string key, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.ObjectField(GetContent(key), obj, objType, allowSceneObjects, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="allowSceneObjects">Allow assigning scene objects. See Description for more info.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The object that has been set by the user.</para>
        /// </returns>
        public UnityEngine.Object ObjectField(string key, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
        {
            return EditorGUILayout.ObjectField(GetContent(key), obj, objType, allowSceneObjects, options);
        }

        /// <summary>
        ///   <para>Make an X &amp; Y field for entering a Vector2 with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// </param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Vector2 Vector2Field(string key, Vector2 value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Vector2Field(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an X &amp; Y field for entering a Vector2.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// </param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Vector2 Vector2Field(string key, Vector2 value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Vector2Field(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make an X, Y &amp; Z field for entering a Vector3 with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        ///         properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Vector3 Vector3Field(string key, Vector3 value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Vector3Field(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an X, Y &amp; Z field for entering a Vector3.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        ///         properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Vector3 Vector3Field(string key, Vector3 value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Vector3Field(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make an X, Y, Z &amp; W field for entering a Vector4 with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Vector4 Vector4Field(string key, Vector4 value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.Vector4Field(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an X, Y, Z &amp; W field for entering a Vector4.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Vector4 Vector4Field(string key, Vector4 value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Vector4Field(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make an X, Y, W &amp; H field for entering a Rect with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Rect RectField(string key, Rect value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.RectField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make an X, Y, W &amp; H field for entering a Rect.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Rect RectField(string key, Rect value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.RectField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make Center &amp; Extents field for entering a Bounds with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Bounds BoundsField(string key, Bounds value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.BoundsField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make Center &amp; Extents field for entering a Bounds.</para>
        /// </summary>
        /// <param name="key">Localization key of the content to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The value entered by the user.</para>
        /// </returns>
        public Bounds BoundsField(string key, Bounds value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.BoundsField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a field for selecting a Color with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The color selected by the user.</para>
        /// </returns>
        public Color ColorField(string key, Color value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.ColorField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a field for selecting a Color.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The color selected by the user.</para>
        /// </returns>
        public Color ColorField(string key, Color value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.ColorField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a field for editing an AnimationCurve with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The curve edited by the user.</para>
        /// </returns>
        public AnimationCurve CurveField(string key, AnimationCurve value, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.CurveField(GetContent(key), value, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The curve edited by the user.</para>
        /// </returns>
        public AnimationCurve CurveField(string key, AnimationCurve value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.CurveField(GetContent(key), value, options);
        }

        /// <summary>
        ///   <para>Make a field for editing an AnimationCurve with Automatic Help handling.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The curve edited by the user.</para>
        /// </returns>
        public AnimationCurve CurveField(string key, AnimationCurve value, Color color, Rect ranges, bool helpSwitch, params GUILayoutOption[] options)
        {
            var val = EditorGUILayout.CurveField(GetContent(key), value, color, ranges, options);
            InlineHelp(key, helpSwitch);
            return val;
        }

        /// <summary>
        ///   <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        ///   <para>The curve edited by the user.</para>
        /// </returns>
        public AnimationCurve CurveField(string key, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
        {
            return EditorGUILayout.CurveField(GetContent(key), value, color, ranges, options);
        }

        /// <summary>
        /// Make a field for editing a SerializedProperty
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="property">The SerializedProperty to edit</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void PropertyField(string key, SerializedProperty property, params GUILayoutOption[] options) {
            EditorGUILayout.PropertyField(property, GetContent(key), options);
        }
        /// <summary>
        /// Make a field for editing a SerializedProperty
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="property">The SerializedProperty to edit</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void PropertyField(string key, SerializedProperty property, bool helpSwitch, params GUILayoutOption[] options) {
            EditorGUILayout.PropertyField(property, GetContent(key), options);
            InlineHelp(key, helpSwitch);
        }
        /// <summary>
        /// Make a field for editing a SerializedProperty with Automatic Help handling
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="includeChildren">If true the property's children are drawn; otherwise only the control itself (such as only a foldout but nothing below it) </param>
        /// <param name="property">The SerializedProperty to edit</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void PropertyField(string key, bool includeChildren, SerializedProperty property, params GUILayoutOption[] options) {
            EditorGUILayout.PropertyField(property, GetContent(key), includeChildren, options);
        }
        /// <summary>
        /// Make a field for editing a SerializedProperty with Automatic Help handling
        /// </summary>
        /// <param name="key">Localization key of the label to display in front of the field.</param>
        /// <param name="includeChildren">If true the property's children are drawn; otherwise only the control itself (such as only a foldout but nothing below it) </param>
        /// <param name="property">The SerializedProperty to edit</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void PropertyField(string key, bool includeChildren, SerializedProperty property, bool helpSwitch, params GUILayoutOption[] options) {
            EditorGUILayout.PropertyField(property, GetContent(key), includeChildren, options);
            InlineHelp(key, helpSwitch);
        }
    }
}
