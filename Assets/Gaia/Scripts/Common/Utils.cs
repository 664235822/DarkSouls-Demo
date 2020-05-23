// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace GaiaCommon1
{
    public partial class Utils : MonoBehaviour
    {
        #region File helpers

        /// <summary>
        /// Remove any characters that could cause issues with files names from the source passed in
        /// </summary>
        /// <param name="sourceFileName">The source file name </param>
        /// <returns>A destination string with rubbish removed</returns>
        public static string FixFileName(string sourceFileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in sourceFileName)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Open a file for reading
        /// </summary>
        /// <param name="path">File to open</param>
        /// <returns>Filestream of the opened file</returns>
        public static FileStream OpenRead(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Returns the content of the file
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns>Content of the file</returns>
        public static string ReadAllText(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("Argument_EmptyPath");
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8, true, 1024))
                return sr.ReadToEnd();
        }

        /// <summary>
        /// Write the content to a file
        /// </summary>
        /// <param name="path">File to write</param>
        /// <param name="contents">Content to write</param>
        public static void WriteAllText(String path, String contents)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("Argument_EmptyPath");
            if (path == null)
                throw new ArgumentNullException("contents");
            if (contents.Length == 0)
                throw new ArgumentException("Argument_EmptyContents");

            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8, 1024))
                sw.Write(contents);
        }

        /// <summary>
        /// Read all bytes of the supplied file
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns>Byte array of the files contents</returns>
        public static byte[] ReadAllBytes(string path)
        {
#if UNITY_WEBPLAYER
            var file = Resources.Load(path) as TextAsset;
            if (file != null)
            {
                return file.bytes;
            }
            return new byte[0];
#else
            using (FileStream s = OpenRead(path))
            {
                long size = s.Length;
                // limited to 2GB according to http://msdn.microsoft.com/en-us/library/system.io.file.readallbytes.aspx
                if (size > Int32.MaxValue)
                    throw new IOException("Reading more than 2GB with this call is not supported");

                int pos = 0;
                int count = (int)size;
                byte[] result = new byte[size];
                while (count > 0)
                {
                    int n = s.Read(result, pos, count);
                    if (n == 0)
                        throw new IOException("Unexpected end of stream");
                    pos += n;
                    count -= n;
                }
                return result;
            }
#endif
        }

        /// <summary>
        /// Write the byte array to the supplied file
        /// </summary>
        /// <param name="path">File to write</param>
        /// <param name="bytes">Byte array to write</param>
        public static void WriteAllBytes(string path, byte[] bytes)
        {
            using (Stream stream = File.Create(path))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Get the Editor Scripts Path of Prod or return null if unsuccesful.
        /// </summary>
        /// <param name="appConfig">Appconfig of the Prod.</param>
        public static string GetEditorScriptsPath(AppConfig appConfig)
        {
            return GetAppsSubfolder(appConfig.Folder, appConfig.EditorScriptsFolder);
        }

        /// <summary>
        /// Get a folder in a Prod/App or return null if unsuccesful.
        /// </summary>
        /// <param name="appFolder">Folder name of the App.</param>
        /// <param name="subfolderPath">Path of the subfolder inside the Prod/App.</param>
        public static string GetAppsSubfolder(string appFolder, string subfolderPath)
        {
#if PW_DEBUG
            Debug.Log("Looking for subfolder '" + subfolderPath + "' in app folder '"+ appFolder + "', root: 'Assets'");
#endif
            DirectoryInfo searchRoot = new DirectoryInfo("Assets");
            if (!searchRoot.Exists)
            {
                Debug.LogWarning("Search root does not exist: Assets");
                return null;
            }

            List<DirectoryInfo> dirsAtLevel = new List<DirectoryInfo>(searchRoot.GetDirectories());

            while (dirsAtLevel.Count > 0)
            {
                List<DirectoryInfo> nextLevel = new List<DirectoryInfo>();

                for (int i = 0; i < dirsAtLevel.Count; i++)
                {
                    if (dirsAtLevel[i].Name == appFolder)
                    {
                        string path = Path.Combine(dirsAtLevel[i].FullName, subfolderPath);
                        //string path = dirsAtLevel[i].FullName + @"\" + subfolderPath;
#if PW_DEBUG
                        Debug.Log("Found a match for project folder: '" + dirsAtLevel[i].FullName + "'; Checking existence of '" + path + "'");
#endif
                        if (Directory.Exists(path))
                        {
                            // Returning project relative path with slashes
                            path = path.Replace(searchRoot.FullName, "Assets");
                            return path.Replace(@"\", "/");
                        }
                    }
                    nextLevel.AddRange(dirsAtLevel[i].GetDirectories());
                }

                dirsAtLevel = nextLevel;
            }

            Debug.LogWarning("Unable to locate directory: '" + appFolder + "/" + subfolderPath + "'");
            return null;
        }

        #endregion

        #region Math helpers

        /// <summary>
        /// Return true if the values are approximately equal
        /// </summary>
        /// <param name="a">Parameter A</param>
        /// <param name="b">Parameter B</param>
        /// <param name="threshold">Threshold to test for</param>
        /// <returns>True if approximately equal</returns>
        public static bool Math_ApproximatelyEqual(float a, float b, float threshold)
        {

            if (a == b || Mathf.Abs(a - b) < threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if the values are approximately equal
        /// </summary>
        /// <param name="a">Parameter A</param>
        /// <param name="b">Parameter B</param>
        /// <returns>True if approximately equal</returns>
        public static bool Math_ApproximatelyEqual(float a, float b)
        {
            return Math_ApproximatelyEqual(a, b, float.Epsilon);
        }

        /// <summary>
        /// Return true if the value is a power of 2
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>True if a power of 2</returns>
        public static bool Math_IsPowerOf2(int value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Returned value clamped in range of min to max
        /// </summary>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="value">Value to check</param>
        /// <returns>Clamped value</returns>
        public static float Math_Clamp(float min, float max, float value)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Return mod of value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="mod">Mod value</param>
        /// <returns>Mode of value</returns>
        public static float Math_Modulo(float value, float mod)
        {
            return value - mod * (float)Math.Floor(value / mod);
        }

        /// <summary>
        /// Return mod of value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="mod">Mod value</param>
        /// <returns>Mode of value</returns>
        public static int Math_Modulo(int value, int mod)
        {
            return (int)(value - mod * (float)Math.Floor((float)value / mod));
        }

        /// <summary>
        /// Linear interpolation between two values
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <param name="fraction">Fraction</param>
        /// <returns></returns>
        public static float Math_InterpolateLinear(float value1, float value2, float fraction)
        {
            return value1 * (1f - fraction) + value2 * fraction;
        }

        /// <summary>
        /// Smooth interpolation between two values
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <param name="fraction">Fraction</param>
        /// <returns></returns>
        public static float Math_InterpolateSmooth(float value1, float value2, float fraction)
        {
            if (fraction < 0.5f)
            {
                fraction = 2f * fraction * fraction;
            }
            else
            {
                fraction = 1f - 2f * (fraction - 1f) * (fraction - 1f);
            }
            return value1 * (1f - fraction) + value2 * fraction;
        }

        /// <summary>
        /// Calculate the distance between two points
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <returns></returns>
        public static float Math_Distance(float x1, float y1, float x2, float y2)
        {
            return Mathf.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }

        public static float Math_InterpolateSmooth2(float v1, float v2, float fraction)
        {
            float fraction2 = fraction * fraction;
            fraction = 3 * fraction2 - 2f * fraction * fraction2;
            return v1 * (1f - fraction) + v2 * fraction;
        }

        public static float Math_InterpolateCubic(float v0, float v1, float v2, float v3, float fraction)
        {
            float p = (v3 - v2) - (v0 - v1);
            float q = (v0 - v1) - p;
            float r = v2 - v0;
            float fraction2 = fraction * fraction;
            return p * fraction * fraction2 + q * fraction2 + r * fraction + v1;
        }


        /// <summary>
        /// Rotate the point around the pivot - used to handle rotation
        /// </summary>
        /// <param name="point">Point to move</param>
        /// <param name="pivot">Pivot</param>
        /// <param name="angle">Angle to pivot</param>
        /// <returns>New location</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
        {
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angle) * dir;
            point = dir + pivot;
            return point;
        }

        #endregion

        #region Time helpers

        /// <summary>
        /// Get the seconds passed since 2018-1-1 0:00
        /// </summary>
        /// <returns>Seconds passed since 2018-1-1 0:00</returns>
        public static int GetFrapoch()
        {
            // This is sufficient till 2086
            System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(2018, 1, 1);
            return (int)t.TotalSeconds;
        }

        /// <summary>
        /// Get the seconds passed between <paramref name="time"/> and 2018-1-1 0:00
        /// </summary>
        /// <param name="time">Time to convert (must be later than 2018-1-1 0:00</param>
        /// <returns>Seconds passed between <paramref name="time"/> and 2018-1-1 0:00</returns>
        public static int TimeToFrapoch(DateTime time)
        {
            // This is sufficient till 2086
            System.TimeSpan t = time - new System.DateTime(2018, 1, 1);
            return (int)t.TotalSeconds;
        }
        
        /// <summary>
        /// Convert Frapoch seconds to local date
        /// </summary>
        /// <param name="seconds">Frapoch seconds</param>
        /// <returns></returns>
        public static DateTime FrapochToLocalDate(int seconds)
        {
            return FrapochToLocalDate((double)seconds);
        }

        /// <summary>
        /// Convert Frapoch seconds to local date
        /// </summary>
        /// <param name="seconds">Frapoch seconds</param>
        /// <returns></returns>
        public static DateTime FrapochToLocalDate(double seconds)
        {
            return new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
        }

        #endregion

        #region Adhoc helpers

        /// <summary>
        /// Check to see if a game objects layer is in the layer mask supplied
        /// </summary>
        /// <param name="obj">Game object to check</param>
        /// <param name="mask">Layer maskt to check</param>
        /// <returns>True if it is in the mask</returns>
        public static bool IsInLayerMask(GameObject obj, LayerMask mask)
        {
            return ((mask.value & (1 << obj.layer)) > 0);
        }

        /// <summary>
        /// Check to see if the textures are the same based on the instance id
        /// </summary>
        /// <param name="tex1">First texture</param>
        /// <param name="tex2">Second texture</param>
        /// <param name="checkID">If true will do an instance ID check</param>
        /// <returns>True if same instance id</returns>
        public static bool IsSameTexture(Texture2D tex1, Texture2D tex2, bool checkID = false)
        {
            if (tex1 == null || tex2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (tex1.GetInstanceID() != tex2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (tex1.name != tex2.name)
            {
                return false;
            }

            if (tex1.width != tex2.width)
            {
                return false;
            }

            if (tex1.height != tex2.height)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check to see if the game objects are the same based on the instance id
        /// </summary>
        /// <param name="go1">First game object</param>
        /// <param name="go2">Second game object</param>
        /// <param name="checkID">If true will do an instance ID check</param>
        /// <returns>True if same instance id</returns>
        public static bool IsSameGameObject(GameObject go1, GameObject go2, bool checkID = false)
        {
            if (go1 == null || go2 == null)
            {
                return false;
            }

            //Check the instance id
            if (checkID)
            {
                if (go1.GetInstanceID() != go2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            //Check the name
            if (go1.name != go2.name)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the specified type if it exists
        /// </summary>
        /// <param name="TypeName">Name of the type to load</param>
        /// <returns>Selected type or null</returns>
        public static Type GetType(string TypeName)
        {

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            var type = Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (TypeName.Contains("."))
            {
                // Get the name of the assembly (Assumption is that we are using 
                // fully-qualified type names)
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                // Attempt to load the indicated Assembly
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly == null)
                        return null;

                    // Ask that assembly to return the proper Type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
                catch (Exception)
                {
                    //Debug.Log("Unable to load assemmbly : " + ex.Message);
                }
            }

            // If we still haven't found the proper type, we can enumerate all of the 
            // loaded assemblies and see if any of them define the type
            var currentAssembly = Assembly.GetCallingAssembly();
            {
                // Load the referenced assembly
                if (currentAssembly != null)
                {
                    // See if that assembly defines the named type
                    type = currentAssembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }

            }

            //All loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int asyIdx = 0; asyIdx < assemblies.GetLength(0); asyIdx++)
            {
                type = assemblies[asyIdx].GetType(TypeName);
                if (type != null)
                {
                    return type;
                }
            }

            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                // Load the referenced assembly
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    // See if that assembly defines the named type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }

            // The type just couldn't be found...
            return null;
        }

        #endregion
    }
}