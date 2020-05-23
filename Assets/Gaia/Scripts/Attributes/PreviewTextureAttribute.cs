//using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using System.IO;

//namespace Gaia
//{
//    /// <summary>
//    /// Rather brilliant little propert drawer for previewing textures.
//    /// Sourced from : https://github.com/anchan828/property-drawer-collection
//    /// </summary>
//    public class PreviewTextureAttribute : PropertyAttribute
//    {
//        public Rect m_lastPosition = new Rect(0, 0, 0, 0);
//        public long m_expire = 6000000000; // 10min
//        public WWW m_www;
//        public Texture2D m_cached;
//        public float m_width = 1f;
//        public float m_offset = 0f;

//        public PreviewTextureAttribute()
//        {
//        }

//        public PreviewTextureAttribute(int expire)
//        {
//            this.m_expire = expire * 1000 * 10000;
//        }

//        /// <summary>
//        /// Texture preview
//        /// </summary>
//        /// <param name="offset">Offset from LHS as pct of total image offset - range 0f..1f</param>
//        /// <param name="width">Width of image as a pct of total available width rang 0f..1f</param>
//        public PreviewTextureAttribute(float offset, float width)
//        {
//            this.m_offset = offset;
//            this.m_width = width;
//        }
//    }

//#if UNITY_EDITOR

//    [CustomPropertyDrawer(typeof(PreviewTextureAttribute))]
//    public class PreviewTextureDrawer : PropertyDrawer
//    {

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            position.height = 16;
//            if (property.propertyType == SerializedPropertyType.String)
//            {
//                DrawStringValue(position, property, label);
//            }
//            else if (property.propertyType == SerializedPropertyType.ObjectReference)
//            {
//                DrawTextureValue(position, property, label);
//            }
//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return base.GetPropertyHeight(property, label) + previewTextureAttribute.m_lastPosition.height;
//        }

//        PreviewTextureAttribute previewTextureAttribute
//        {
//            get { return (PreviewTextureAttribute)attribute; }
//        }

//        void DrawTextureValue(Rect position, SerializedProperty property, GUIContent label)
//        {
//            property.objectReferenceValue = (Texture2D)EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Texture2D), false);


//            if (property.objectReferenceValue != null)
//                DrawTexture(position, (Texture2D)property.objectReferenceValue);
//        }

//        void DrawStringValue(Rect position, SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginChangeCheck();
//            property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
//            if (EditorGUI.EndChangeCheck())
//            {
//                previewTextureAttribute.m_www = null;
//                previewTextureAttribute.m_cached = null;
//            }
//            string path = GetCachedTexturePath(property.stringValue);

//            if (!string.IsNullOrEmpty(path))
//            {
//                if (IsExpired(path))
//                {
//                    Delete(path);
//                }
//                else if (previewTextureAttribute.m_cached == null)
//                    previewTextureAttribute.m_cached = GetTextureFromCached(path);
//            }
//            else
//                previewTextureAttribute.m_cached = null;

//            if (previewTextureAttribute.m_cached == null)
//            {
//                previewTextureAttribute.m_cached = GetTextureFromWWW(position, property);
//            }
//            else
//                DrawTexture(position, previewTextureAttribute.m_cached);
//        }

//        bool IsExpired(string path)
//        {
//            string fileName = Path.GetFileNameWithoutExtension(path);
//            string[] split = fileName.Split('_');
//            return System.DateTime.Now.Ticks >= long.Parse(split[1]);
//        }

//        string GetCachedTexturePath(string stringValue)
//        {
//            int hash = stringValue.GetHashCode();
//            foreach (string path in Directory.GetFiles("Temp"))
//            {
//                if (Path.GetFileNameWithoutExtension(path).StartsWith(hash.ToString()))
//                {
//                    return path;
//                }
//            }
//            return string.Empty;
//        }

//        Texture2D GetTextureFromWWW(Rect position, SerializedProperty property)
//        {
//            if (previewTextureAttribute.m_www == null)
//            {
//                previewTextureAttribute.m_www = new WWW(property.stringValue);
//            }
//            else if (!previewTextureAttribute.m_www.isDone)
//            {
//                previewTextureAttribute.m_lastPosition = new Rect(position.x, position.y + 16, position.width, 16);
//                EditorGUI.ProgressBar(previewTextureAttribute.m_lastPosition, previewTextureAttribute.m_www.progress, "Downloading... " + (previewTextureAttribute.m_www.progress * 100) + "%");
//            }
//            else if (previewTextureAttribute.m_www.isDone)
//            {

//                if (previewTextureAttribute.m_www.error != null)
//                    return null;

//                int hash = property.stringValue.GetHashCode();
//                long expire = (System.DateTime.Now.Ticks + previewTextureAttribute.m_expire);
//                GaiaCommon1.Utils.WriteAllBytes(string.Format("Temp/{0}_{1}_{2}_{3}", hash, expire, previewTextureAttribute.m_www.texture.width, previewTextureAttribute.m_www.texture.height), previewTextureAttribute.m_www.bytes);
//                return previewTextureAttribute.m_www.texture;
//            }
//            return null;
//        }

//        Texture2D GetTextureFromCached(string path)
//        {
//            string[] split = Path.GetFileNameWithoutExtension(path).Split('_');
//            int width = int.Parse(split[2]);
//            int height = int.Parse(split[3]);
//            Texture2D t = new Texture2D(width, height);

//            return t.LoadImage(GaiaCommon1.Utils.ReadAllBytes(path)) ? t : null;
//        }

//        private GUIStyle style;

//        void DrawTexture(Rect position, Texture2D texture)
//        {
//            //float width = Mathf.Clamp(texture.width, position.width * 0.7f, position.width * 0.7f);

//            float offset = 0.05f + previewTextureAttribute.m_offset;
//            float width = Mathf.Clamp(texture.width, position.width * previewTextureAttribute.m_width, position.width * previewTextureAttribute.m_width);
//            previewTextureAttribute.m_lastPosition = new Rect(position.width * offset, position.y + 16, width, texture.height * (width / texture.width));

//            if (style == null)
//            {
//                style = new GUIStyle();
//                style.imagePosition = ImagePosition.ImageOnly;
//            }
//            style.normal.background = texture;
//            GUI.Label(previewTextureAttribute.m_lastPosition, "", style);
//        }

//        void Delete(string path)
//        {
//            File.Delete(path);
//            previewTextureAttribute.m_cached = null;
//        }
//    }

//#endif
//}