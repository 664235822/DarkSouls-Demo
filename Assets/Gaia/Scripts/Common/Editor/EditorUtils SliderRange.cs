using UnityEngine;
using UnityEditor;

namespace GaiaCommon1 {
    /// <summary>
    /// Handy editor utils - SliderRange custom control
    /// </summary>
    public partial class EditorUtils {

        /// <summary> Internal variable used by SliderRange to track the start of a drag </summary>
        static float SliderRangeDragPos = 0f;
        /// <summary> Internal variable used by SliderRange to track the starting start/end values of a drag </summary>
        static Vector2 SliderRangeDragStartEnd = Vector2.zero;

        //Base versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, ref float start, ref float end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            bool changed = false;
            GUISkin editorSkin = null;
            if (Event.current.type == EventType.Repaint)
                editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            float totalHeight = position.height;
            position = EditorGUI.PrefixLabel(position, label);
            float minMaxWidth = Mathf.Min(80f, (position.width * 0.3f + 10f) * 0.5f);
            Rect minRect = new Rect(position.x, position.y, minMaxWidth, totalHeight);
            Rect maxRect = new Rect(position.xMax - minMaxWidth, position.y, minMaxWidth, totalHeight);
            EditorGUI.BeginChangeCheck();
            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            start = Mathf.Clamp(EditorGUI.FloatField(minRect, start), minValue, end);
            end = Mathf.Clamp(EditorGUI.FloatField(maxRect, end), start, maxValue);
            EditorGUI.indentLevel = oldIndentLevel;
            if (EditorGUI.EndChangeCheck())
                changed = true;
            #region Slider
            Rect barRect = new Rect(minRect.xMax + 5f, position.y, maxRect.xMin - minRect.xMax - 10f, totalHeight);
            int mainControlID = GUIUtility.GetControlID(867530901, FocusType.Passive, barRect); //control ID for dragging main bar
            int minControlID = GUIUtility.GetControlID(867530902, FocusType.Passive, barRect); //control ID for dragging min
            int maxControlID = GUIUtility.GetControlID(867530903, FocusType.Passive, barRect); //control ID for dragging max

            float totalWidth = barRect.width - 11f;
            float startPos = (start - minValue) / (maxValue - minValue);
            float endPos = (end - minValue) / (maxValue - minValue);
            Rect drawRect = new Rect(barRect.x + totalWidth * startPos, barRect.y + totalHeight * 0.165f, totalWidth * (endPos - startPos) + 10f, totalHeight * 0.67f);
            //Check for Events
            Rect sliderPanRect = new Rect(drawRect.xMin + 5f, drawRect.y, drawRect.width - 10f, totalHeight * 0.67f);
            Rect sliderEndRect = new Rect(drawRect.xMax - 5f, drawRect.y, 5f, totalHeight * 0.67f);
            Rect sliderBeginRect = new Rect(drawRect.xMin, drawRect.y, 5f, totalHeight * 0.67f);
            #region Mouse Events
            EditorGUIUtility.AddCursorRect(sliderEndRect, MouseCursor.SplitResizeLeftRight);
            EditorGUIUtility.AddCursorRect(sliderBeginRect, MouseCursor.SplitResizeLeftRight);
            //check for drag of either ends or slider
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                if (sliderEndRect.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = maxControlID;
                    SliderRangeDragStartEnd.y = end;
                    SliderRangeDragPos = Event.current.mousePosition.x;
                    Event.current.Use();
                } else if (sliderBeginRect.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = minControlID;
                    SliderRangeDragStartEnd.x = start;
                    SliderRangeDragPos = Event.current.mousePosition.x;
                    Event.current.Use();
                } else if (sliderPanRect.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = mainControlID;
                    SliderRangeDragStartEnd.x = start;
                    SliderRangeDragStartEnd.y = end;
                    SliderRangeDragPos = Event.current.mousePosition.x;
                    Event.current.Use();
                }
            } else if (Event.current.type == EventType.MouseDrag) {
                if (GUIUtility.hotControl == maxControlID) {
                    float DragDelta = (Event.current.mousePosition.x - SliderRangeDragPos) / barRect.width;
                    end = Mathf.Clamp(SliderRangeDragStartEnd.y + DragDelta * (maxValue - minValue), start, maxValue);
                    Event.current.Use();
                    changed = true;
                } else if (GUIUtility.hotControl == minControlID) {
                    float DragDelta = (Event.current.mousePosition.x - SliderRangeDragPos) / barRect.width;
                    start = Mathf.Clamp(SliderRangeDragStartEnd.x + DragDelta * (maxValue - minValue), minValue, end);
                    Event.current.Use();
                    changed = true;
                } else if (GUIUtility.hotControl == mainControlID) {
                    float DragDelta = (Event.current.mousePosition.x - SliderRangeDragPos) / barRect.width;
                    float barSize = SliderRangeDragStartEnd.y - SliderRangeDragStartEnd.x;
                    start = Mathf.Clamp(SliderRangeDragStartEnd.x + DragDelta * (maxValue - minValue), minValue, maxValue - barSize);
                    end = Mathf.Clamp(SliderRangeDragStartEnd.y + DragDelta * (maxValue - minValue), minValue + barSize, maxValue);
                    Event.current.Use();
                    changed = true;
                }
            } else if (Event.current.type == EventType.MouseUp) {
                if (GUIUtility.hotControl == maxControlID || GUIUtility.hotControl == minControlID || GUIUtility.hotControl == mainControlID)
                    GUIUtility.hotControl = -1;
                SliderRangeDragPos = 0f;
                SliderRangeDragStartEnd = Vector2.zero;
            }
            #endregion
            //Draw slider (Repaint event only)
            if (Event.current.type == EventType.Repaint) {
                editorSkin.horizontalSlider.Draw(barRect, false, false, false, false);
                if (!invert) { //normal display
                    editorSkin.button.Draw(drawRect, false, false, false, false);
                } else { //invert display
                    GUIStyle leftButton = editorSkin.GetStyle("ButtonLeft") ?? editorSkin.button;
                    GUIStyle rightButton = editorSkin.GetStyle("ButtonRight") ?? editorSkin.button;
                    Rect drawRect2 = drawRect;
                    drawRect2.xMin = drawRect.xMax - 5f;
                    drawRect2.xMax = barRect.xMax + 4f;
                    drawRect.xMax = drawRect.xMin + 5f;
                    drawRect.xMin = barRect.xMin - 5f;
                    if (startPos > 0f || endPos >= 1f)
                        rightButton.Draw(drawRect, false, false, false, false);
                    if (endPos < 1f || startPos <= 0f)
                        leftButton.Draw(drawRect2, false, false, false, false);
                }
            }
            return changed;
            #endregion
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, ref float start, ref float end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), label, ref start, ref end, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, ref Vector2 startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(position, label, ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, ref Vector2 startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), label, ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, SerializedProperty start, SerializedProperty end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, label, ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, SerializedProperty startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, label, ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, SerializedProperty start, SerializedProperty end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), label, ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, SerializedProperty startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), label, ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        //Vector2 min/max versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, ref float start, ref float end, Vector2 minMaxValue, bool invert = false) {
            return SliderRange(position, label, ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, ref float start, ref float end, Vector2 minMaxValue, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), label, ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, SerializedProperty start, SerializedProperty end, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, label, ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="label">label to display</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, GUIContent label, SerializedProperty startEnd, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, label, ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, SerializedProperty start, SerializedProperty end, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), label, ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="label">label to display</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(GUIContent label, SerializedProperty startEnd, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), label, ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        //Localized key versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, ref float start, ref float end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(position, GetContent(key), ref start, ref end, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, ref float start, ref float end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref start, ref end, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, ref Vector2 startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(position, GetContent(key), ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, ref Vector2 startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty start, SerializedProperty end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty start, SerializedProperty end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        //Localized key with Vector2 min/max versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, ref float start, ref float end, Vector2 minMaxValue, bool invert = false) {
            return SliderRange(position, GetContent(key), ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, ref float start, ref float end, Vector2 minMaxValue, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty start, SerializedProperty end, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty startEnd, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (EditorGUI.EndChangeCheck()) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty start, SerializedProperty end, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty startEnd, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        //Inline-Help versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited with integrated inlineHelp </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, ref float start, ref float end, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            bool changed = SliderRange(position, GetContent(key), ref start, ref end, minValue, maxValue, invert);
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited with integrated inlineHelp </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, ref float start, ref float end, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref start, ref end, minValue, maxValue, invert);
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, ref Vector2 startEnd, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            bool changed = SliderRange(position, GetContent(key), ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, ref Vector2 startEnd, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty start, SerializedProperty end, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty startEnd, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty start, SerializedProperty end, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty startEnd, bool inlineHelp, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        //Inline-Help with Vector2 min/max versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, ref float start, ref float end, bool inlineHelp, Vector2 minMaxValue, bool invert = false) {
            bool changed = SliderRange(position, GetContent(key), ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, ref float start, ref float end, bool inlineHelp, Vector2 minMaxValue, bool invert = false) {
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty start, SerializedProperty end, bool inlineHelp, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, string key, SerializedProperty startEnd, bool inlineHelp, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (EditorGUI.EndChangeCheck()) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty start, SerializedProperty end, bool inlineHelp, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="key">Key of label for localization</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="inlineHelp">Should inlineHelp be displayed?</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(string key, SerializedProperty startEnd, bool inlineHelp, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), GetContent(key), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            InlineHelp(key, inlineHelp);
            return changed;
        }

        //No-Label versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, ref float start, ref float end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            bool changed = false;
            GUISkin editorSkin = null;
            if (Event.current.type == EventType.Repaint)
                editorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            float totalHeight = position.height;
            float minMaxWidth = Mathf.Min(80f, (position.width * 0.3f + 10f) * 0.5f);
            Rect minRect = new Rect(position.x, position.y, minMaxWidth, totalHeight);
            Rect maxRect = new Rect(position.xMax - minMaxWidth, position.y, minMaxWidth, totalHeight);
            EditorGUI.BeginChangeCheck();
            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            start = Mathf.Clamp(EditorGUI.FloatField(minRect, start), minValue, end);
            end = Mathf.Clamp(EditorGUI.FloatField(maxRect, end), start, maxValue);
            EditorGUI.indentLevel = oldIndentLevel;
            if (EditorGUI.EndChangeCheck())
                changed = true;
            #region Slider
            Rect barRect = new Rect(minRect.xMax + 5f, position.y, maxRect.xMin - minRect.xMax - 10f, totalHeight);
            int mainControlID = GUIUtility.GetControlID(867530901, FocusType.Passive, barRect); //control ID for dragging main bar
            int minControlID = GUIUtility.GetControlID(867530902, FocusType.Passive, barRect); //control ID for dragging min
            int maxControlID = GUIUtility.GetControlID(867530903, FocusType.Passive, barRect); //control ID for dragging max

            float totalWidth = barRect.width - 11f;
            float startPos = (start - minValue) / (maxValue - minValue);
            float endPos = (end - minValue) / (maxValue - minValue);
            Rect drawRect = new Rect(barRect.x + totalWidth * startPos, barRect.y + totalHeight * 0.165f, totalWidth * (endPos - startPos) + 10f, totalHeight * 0.67f);
            //Check for Events
            Rect sliderPanRect = new Rect(drawRect.xMin + 5f, drawRect.y, drawRect.width - 10f, totalHeight * 0.67f);
            Rect sliderEndRect = new Rect(drawRect.xMax - 5f, drawRect.y, 5f, totalHeight * 0.67f);
            Rect sliderBeginRect = new Rect(drawRect.xMin, drawRect.y, 5f, totalHeight * 0.67f);
            #region Mouse Events
            EditorGUIUtility.AddCursorRect(sliderEndRect, MouseCursor.SplitResizeLeftRight);
            EditorGUIUtility.AddCursorRect(sliderBeginRect, MouseCursor.SplitResizeLeftRight);
            //check for drag of either ends or slider
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                if (sliderEndRect.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = maxControlID;
                    SliderRangeDragStartEnd.y = end;
                    SliderRangeDragPos = Event.current.mousePosition.x;
                    Event.current.Use();
                } else if (sliderBeginRect.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = minControlID;
                    SliderRangeDragStartEnd.x = start;
                    SliderRangeDragPos = Event.current.mousePosition.x;
                    Event.current.Use();
                } else if (sliderPanRect.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = mainControlID;
                    SliderRangeDragStartEnd.x = start;
                    SliderRangeDragStartEnd.y = end;
                    SliderRangeDragPos = Event.current.mousePosition.x;
                    Event.current.Use();
                }
            } else if (Event.current.type == EventType.MouseDrag) {
                if (GUIUtility.hotControl == maxControlID) {
                    float DragDelta = (Event.current.mousePosition.x - SliderRangeDragPos) / barRect.width;
                    end = Mathf.Clamp(SliderRangeDragStartEnd.y + DragDelta * (maxValue - minValue), start, maxValue);
                    Event.current.Use();
                    changed = true;
                } else if (GUIUtility.hotControl == minControlID) {
                    float DragDelta = (Event.current.mousePosition.x - SliderRangeDragPos) / barRect.width;
                    start = Mathf.Clamp(SliderRangeDragStartEnd.x + DragDelta * (maxValue - minValue), minValue, end);
                    Event.current.Use();
                    changed = true;
                } else if (GUIUtility.hotControl == mainControlID) {
                    float DragDelta = (Event.current.mousePosition.x - SliderRangeDragPos) / barRect.width;
                    float barSize = SliderRangeDragStartEnd.y - SliderRangeDragStartEnd.x;
                    start = Mathf.Clamp(SliderRangeDragStartEnd.x + DragDelta * (maxValue - minValue), minValue, maxValue - barSize);
                    end = Mathf.Clamp(SliderRangeDragStartEnd.y + DragDelta * (maxValue - minValue), minValue + barSize, maxValue);
                    Event.current.Use();
                    changed = true;
                }
            } else if (Event.current.type == EventType.MouseUp) {
                if (GUIUtility.hotControl == maxControlID || GUIUtility.hotControl == minControlID || GUIUtility.hotControl == mainControlID)
                    GUIUtility.hotControl = -1;
                SliderRangeDragPos = 0f;
                SliderRangeDragStartEnd = Vector2.zero;
            }
            #endregion
            //Draw slider (Repaint event only)
            if (Event.current.type == EventType.Repaint) {
                editorSkin.horizontalSlider.Draw(barRect, false, false, false, false);
                if (!invert) { //normal display
                    editorSkin.button.Draw(drawRect, false, false, false, false);
                } else { //invert display
                    GUIStyle leftButton = editorSkin.GetStyle("ButtonLeft") ?? editorSkin.button;
                    GUIStyle rightButton = editorSkin.GetStyle("ButtonRight") ?? editorSkin.button;
                    Rect drawRect2 = drawRect;
                    drawRect2.xMin = drawRect.xMax - 5f;
                    drawRect2.xMax = barRect.xMax + 4f;
                    drawRect.xMax = drawRect.xMin + 5f;
                    drawRect.xMin = barRect.xMin - 5f;
                    if (startPos > 0f || endPos >= 1f)
                        rightButton.Draw(drawRect, false, false, false, false);
                    if (endPos < 1f || startPos <= 0f)
                        leftButton.Draw(drawRect2, false, false, false, false);
                }
            }
            return changed;
            #endregion
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(ref float start, ref float end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), ref start, ref end, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, ref Vector2 startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(position, ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="startEnd">Min/Max Vector2 value to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(ref Vector2 startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), ref startEnd.x, ref startEnd.y, minValue, maxValue, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, SerializedProperty start, SerializedProperty end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, SerializedProperty startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(SerializedProperty start, SerializedProperty end, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minValue">Minimum value allowed</param>
        /// <param name="maxValue">Maximum value allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(SerializedProperty startEnd, float minValue = 0f, float maxValue = 1f, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), ref min, ref max, minValue, maxValue, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        //No-Label Vector2 min/max versions
        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, ref float start, ref float end, Vector2 minMaxValue, bool invert = false) {
            return SliderRange(position, ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end values being edited </summary>
        /// <param name="start">Start value to edit</param>
        /// <param name="end">End value to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(ref float start, ref float end, Vector2 minMaxValue, bool invert = false) {
            return SliderRange(EditorGUILayout.GetControlRect(), ref start, ref end, minMaxValue.x, minMaxValue.y, invert);
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, SerializedProperty start, SerializedProperty end, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="position">Rect to draw control in</param>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(Rect position, SerializedProperty startEnd, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(position, ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperties (Float or Integer) being edited </summary>
        /// <param name="start">Start Property to edit</param>
        /// <param name="end">End Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(SerializedProperty start, SerializedProperty end, Vector2 minMaxValue, bool invert = false) {
            float min;
            switch (start.propertyType) {
                case SerializedPropertyType.Float:
                    min = start.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    min = start.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    min = 0f;
                    break;
            }
            float max;
            switch (end.propertyType) {
                case SerializedPropertyType.Float:
                    max = end.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    max = end.intValue;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + start.propertyType + "' Expecting 'Float' or 'Integer'");
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (start.propertyType) {
                    case SerializedPropertyType.Float:
                        start.floatValue = min;
                        break;
                    case SerializedPropertyType.Integer:
                        start.intValue = (int)min;
                        break;
                    default:
                        break;
                }
                switch (end.propertyType) {
                    case SerializedPropertyType.Float:
                        end.floatValue = max;
                        break;
                    case SerializedPropertyType.Integer:
                        end.intValue = (int)max;
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }

        /// <summary> Custom control to edit a range between minValue and maxValue with a start and end SerializedProperty (Vector2) being edited </summary>
        /// <param name="startEnd">Min/Max Vector2 Property to edit</param>
        /// <param name="minMaxValue">Minimum/Maximum values allowed</param>
        /// <param name="invert">Should the bar be drawn inverted (everything but the area between start/end is filled)</param>
        /// <returns>True if a value was changed, otherwise false</returns>
        public bool SliderRange(SerializedProperty startEnd, Vector2 minMaxValue, bool invert = false) {
            float min, max;
            switch (startEnd.propertyType) {
                case SerializedPropertyType.Vector2:
                    min = startEnd.vector2Value.x;
                    max = startEnd.vector2Value.y;
                    break;
                default:
                    Debug.LogWarning("Start property is type '" + startEnd.propertyType + "' Expecting 'Vector2'");
                    min = 0f;
                    max = 1f;
                    break;
            }
            bool changed = SliderRange(EditorGUILayout.GetControlRect(), ref min, ref max, minMaxValue.x, minMaxValue.y, invert);
            if (changed) {
                switch (startEnd.propertyType) {
                    case SerializedPropertyType.Vector2:
                        startEnd.vector2Value = new Vector2(min, max);
                        break;
                    default:
                        break;
                }
            }
            return changed;
        }
    }
}
