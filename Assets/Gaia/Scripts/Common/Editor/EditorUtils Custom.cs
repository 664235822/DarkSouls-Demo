// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using GaiaCommon1.Localization;

namespace GaiaCommon1 {
    /// <summary>
    /// Handy editor utils - Custom controls
    /// </summary>
    public partial class EditorUtils {


        #region Sliders

        ///// <summary>
        /////   <para>Make a slider with a delayed text field to enter values manually. The user can drag to change a value between a min and a max with Automatic Help handling.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the slider.</param>
        ///// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        ///// <param name="leftValue">The value at the left end of the slider.</param>
        ///// <param name="rightValue">The value at the right end of the slider.</param>
        ///// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <returns>
        /////   <para>The value that has been set by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        ///// </returns>
        //public float DelayedSlider(string key, float value, float leftValue, float rightValue, bool helpSwitch, params GUILayoutOption[] options)
        //{
        //    value = DelayedSlider(key, value, leftValue, rightValue, options);
        //    InlineHelp(key, helpSwitch);
        //    return value;
        //}

        ///// <summary>
        /////   <para>Make a slider with a delayed text field to enter values manually. The user can drag to change a value between a min and a max.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the slider.</param>
        ///// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        ///// <param name="leftValue">The value at the left end of the slider.</param>
        ///// <param name="rightValue">The value at the right end of the slider.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <returns>
        /////   <para>The value that has been set by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        ///// </returns>
        //public float DelayedSlider(string key, float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        //{
        //    GUILayout.BeginHorizontal();
        //    {
        //        PrefixLabel(key);
        //        value = GUILayout.HorizontalSlider(value, leftValue, rightValue, options);
        //        value = EditorGUILayout.DelayedFloatField(value, Styles.sliderTextField, GUILayout.Width(65f));
        //    }
        //    GUILayout.EndHorizontal();
        //    return value;
        //}

        ///// <summary>
        /////   <para>Make a slider with a delayed text field to enter values manually. The user can drag to change an integer value between a min and a max with Automatic Help handling.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the slider.</param>
        ///// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        ///// <param name="leftValue">The value at the left end of the slider.</param>
        ///// <param name="rightValue">The value at the right end of the slider.</param>
        ///// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <returns>
        /////   <para>The value that has been set by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        ///// </returns>
        //public int DelayedIntSlider(string key, int value, int leftValue, int rightValue, bool helpSwitch, params GUILayoutOption[] options)
        //{
        //    value = DelayedIntSlider(key, value, leftValue, rightValue, options);
        //    InlineHelp(key, helpSwitch);
        //    return value;
        //}

        ///// <summary>
        /////   <para>Make a slider with a delayed text field to enter values manually. The user can drag to change an integer value between a min and a max.</para>
        ///// </summary>
        ///// <param name="key">Localization key of the label in front of the slider.</param>
        ///// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        ///// <param name="leftValue">The value at the left end of the slider.</param>
        ///// <param name="rightValue">The value at the right end of the slider.</param>
        ///// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        ///// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        ///// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        ///// <returns>
        /////   <para>The value that has been set by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        ///// </returns>
        //public int DelayedIntSlider(string key, int value, int leftValue, int rightValue, params GUILayoutOption[] options)
        //{
        //    GUILayout.BeginHorizontal();
        //    {
        //        PrefixLabel(key);
        //        Rect r = EditorGUILayout.GetControlRect(false, options);
        //        value = Mathf.RoundToInt(GUILayout.HorizontalSlider(value, leftValue, rightValue, options));
        //        value = EditorGUILayout.DelayedIntField(value, Styles.sliderTextField, GUILayout.Width(65f));
        //    }
        //    GUILayout.EndHorizontal();
        //    return value;
        //}

        #endregion

        #region Popups

        /// <summary>
        /// Make an Enum popup selection field with localized value names
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="valueKeyPrefix">Localization key prefix of the option names. (option name will be appended)</param>
        /// <param name="property">SerializedProperty to edit</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void EnumPopupLocalized(string key, string valueKeyPrefix, SerializedProperty property, params GUILayoutOption[] options)
        {
            string[] EnumNames = property.enumDisplayNames;
            GUIContent[] displayNames = new GUIContent[EnumNames.Length];
            int[] displayValues = new int[EnumNames.Length];
            for (int x = 0; x < EnumNames.Length; ++x)
            {
                displayValues[x] = x;
                displayNames[x] = GetContent(valueKeyPrefix + EnumNames[x]);
            }
            int newValue = property.hasMultipleDifferentValues ? -1 : property.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUILayout.IntPopup(GetContent(key), newValue, displayNames, displayValues, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = newValue;
            }
        }

        /// <summary>
        /// Make an Enum popup selection field with localized value names
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="valueKeyPrefix">Localization key prefix of the option names. (option name will be appended)</param>
        /// <param name="property">SerializedProperty to edit</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void EnumPopupLocalized(string key, string valueKeyPrefix, SerializedProperty property, bool helpSwitch, params GUILayoutOption[] options)
        {
            string[] EnumNames = property.enumDisplayNames;
            GUIContent[] displayNames = new GUIContent[EnumNames.Length];
            int[] displayValues = new int[EnumNames.Length];
            for (int x = 0; x < EnumNames.Length; ++x)
            {
                displayValues[x] = x;
                displayNames[x] = GetContent(valueKeyPrefix + EnumNames[x]);
            }
            int newValue = property.hasMultipleDifferentValues ? -1 : property.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUILayout.IntPopup(GetContent(key), newValue, displayNames, displayValues, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = newValue;
            }
            InlineHelp(key, helpSwitch);
        }

        /// <summary>
        /// Make an Enum popup selection field with localized value names
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="valueKeyPrefix">Localization key prefix of the option names. (option name will be appended)</param>
        /// <param name="property">SerializedProperty to edit</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void EnumPopupLocalized(string key, string valueKeyPrefix, SerializedProperty property, GUIStyle style, params GUILayoutOption[] options)
        {
            string[] EnumNames = property.enumDisplayNames;
            GUIContent[] displayNames = new GUIContent[EnumNames.Length];
            int[] displayValues = new int[EnumNames.Length];
            for (int x = 0; x < EnumNames.Length; ++x)
            {
                displayValues[x] = x;
                displayNames[x] = GetContent(valueKeyPrefix + EnumNames[x]);
            }
            int newValue = property.hasMultipleDifferentValues ? -1 : property.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUILayout.IntPopup(GetContent(key), newValue, displayNames, displayValues, style, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = newValue;
            }
        }

        /// <summary>
        /// Make an Enum popup selection field with localized value names
        /// </summary>
        /// <param name="key">Localization key of the label in front of the field.</param>
        /// <param name="valueKeyPrefix">Localization key prefix of the option names. (option name will be appended)</param>
        /// <param name="property">SerializedProperty to edit</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="helpSwitch">The <see langword="bool"/> that the user interacts with to switch help On/Off.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public void EnumPopupLocalized(string key, string valueKeyPrefix, SerializedProperty property, GUIStyle style, bool helpSwitch, params GUILayoutOption[] options)
        {
            string[] EnumNames = property.enumDisplayNames;
            GUIContent[] displayNames = new GUIContent[EnumNames.Length];
            int[] displayValues = new int[EnumNames.Length];
            for (int x = 0; x < EnumNames.Length; ++x)
            {
                displayValues[x] = x;
                displayNames[x] = GetContent(valueKeyPrefix + EnumNames[x]);
            }
            int newValue = property.hasMultipleDifferentValues ? -1 : property.enumValueIndex;
            EditorGUI.BeginChangeCheck();
            newValue = EditorGUILayout.IntPopup(GetContent(key), newValue, displayNames, displayValues, style, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = newValue;
            }
            InlineHelp(key, helpSwitch);
        }

        #endregion
    }
}
