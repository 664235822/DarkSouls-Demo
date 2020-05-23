using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gaia
{
    public class Utils : MonoBehaviour
    {
        #region Asset directory helpers
        /// <summary>
        /// Get raw gaia asset directory
        /// </summary>
        /// <returns>Base gaia directory</returns>
        public static string GetGaiaAssetDirectory()
        {
            string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Get the asset directory for a particular featiure type
        /// </summary>
        /// <param name="featureType"></param>
        /// <returns>Path of feature type</returns>
        public static string GetGaiaAssetDirectory(Gaia.GaiaConstants.FeatureType featureType)
        {
            string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
            path = Path.Combine(path, featureType.ToString());
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Get a list of the Gaia stamps for the feature type provided
        /// </summary>
        /// <param name="featureType"></param>
        /// <returns></returns>
        public static List<string> GetGaiaStampsList(Gaia.GaiaConstants.FeatureType featureType)
        {
            return new List<string>(System.IO.Directory.GetFiles(GetGaiaAssetDirectory(featureType), "*.jpg"));
        }

        /// <summary>
        /// Get the full asset path for a specific asset type and name
        /// </summary>
        /// <param name="featureType">The type of feature this asset is</param>
        /// <param name="assetName">The file name of the asset</param>
        /// <returns>Fully qualified path of the asset</returns>
        public static string GetGaiaAssetPath(Gaia.GaiaConstants.FeatureType featureType, string assetName)
        {
            string path = GetGaiaAssetDirectory(featureType);
            path = Path.Combine(GetGaiaAssetDirectory(featureType), assetName);
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Get the full asset path for a specific asset type and name
        /// </summary>
        /// <param name="featureType">The type of feature this asset is</param>
        /// <param name="assetName">The file name of the asset</param>
        /// <returns>Fully qualified path of the asset</returns>
        public static string GetGaiaStampAssetPath(Gaia.GaiaConstants.FeatureType featureType, string assetName)
        {
            string path = GetGaiaAssetDirectory(featureType);
            path = Path.Combine(GetGaiaAssetDirectory(featureType), "Data");
            path = Path.Combine(path, assetName);
            return path.Replace('\\', '/');
        }


        /// <summary>
        /// Parse a stamp preview texture to work out where the stamp lives
        /// </summary>
        /// <param name="source">Source texture</param>
        /// <returns></returns>
        public static string GetGaiaStampPath(Texture2D source)
        {
            string path = "";
#if UNITY_EDITOR
            path = UnityEditor.AssetDatabase.GetAssetPath(source);
#endif

            string fileName = Path.GetFileName(path);
            path = Path.Combine(Path.GetDirectoryName(path), "Data");
            path = Path.Combine(path, fileName);
            path = Path.ChangeExtension(path, ".bytes");
            path = path.Replace('\\', '/');
            return path;
        }

        /// <summary>
        /// Check to see if this actually a valid stamp - needs a .jpg and a .bytes file
        /// </summary>
        /// <param name="source">Source texture</param>
        /// <returns></returns>
        public static bool CheckValidGaiaStampPath(Texture2D source)
        {
            string path = "";
#if UNITY_EDITOR
            path = UnityEditor.AssetDatabase.GetAssetPath(source);
#endif

            //path = GetGaiaAssetDirectory() + path.Replace(Gaia.GaiaConstants.AssetDirFromAssetDB, "");

            // Check to see if we have a jpg file
            if (Path.GetExtension(path).ToLower() != ".jpg")
            {
                return false;
            }

            //Check to see if we have asset file
            string fileName = Path.GetFileName(path);
            path = Path.Combine(Path.GetDirectoryName(path), "Data");
            path = Path.Combine(path, fileName);
            path = Path.ChangeExtension(path, ".bytes");
            path = path.Replace('\\', '/');

            if (System.IO.File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create all the Gaia asset directories for scans to go into
        /// </summary>
        public static void CreateGaiaAssetDirectories()
        {
#if UNITY_EDITOR
            string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
            try
            {
                bool addedDir = false;
                foreach (Gaia.GaiaConstants.FeatureType feature in Enum.GetValues(typeof(Gaia.GaiaConstants.FeatureType)))
                {
                    path = GetGaiaAssetDirectory(feature);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        path = Path.Combine(path, "Data");
                        Directory.CreateDirectory(path);
                        addedDir = true;
                    }
                }

                if (addedDir)
                {
                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to create directory {0} : {1}", path, e.Message));
            }
#endif
        }

        /// <summary>
        /// Get all objects of the given type at the location in the path. Only works in the editor.
        /// </summary>
        /// <typeparam name="T">Type of object to load</typeparam>
        /// <param name="path">The path to look in</param>
        /// <returns>List of those objects</returns>
        public static T[] GetAtPath<T>(string path)
        {

            ArrayList al = new ArrayList();

#if UNITY_EDITOR

            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
            foreach (string fileName in fileEntries)
            {
                int index = fileName.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += fileName.Substring(index);

                UnityEngine.Object t = UnityEditor.AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

#endif

            T[] result = new T[al.Count];
            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }

        #endregion

        #region Image helpers

        /// <summary>
        /// Make the texture supplied into a normal map
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureNormal(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && tImporter.textureType != TextureImporterType.NormalMap)
            {
                tImporter.textureType = TextureImporterType.NormalMap;
                tImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Make the texture supplied readable
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureReadable(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && tImporter.isReadable != true)
            {
                tImporter.isReadable = true;
                tImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Make the texture supplied uncompressed
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureUncompressed(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && tImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                tImporter.textureCompression = TextureImporterCompression.Uncompressed;
                tImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Compress / encode a single layer map file to an image
        /// </summary>
        /// <param name="input">Single layer map in format x,y</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToSingleChannelFileImage(float[,] input, string imageName, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool exportPNG = true, bool exportJPG = true)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);

            Texture2D exportTexture = new Texture2D(width, height, imageStorageFormat, false);
            Color pixelColor = new Color();
            pixelColor.a = 1f;
            pixelColor.r = pixelColor.g = pixelColor.b = 0f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixelColor.r = pixelColor.b = pixelColor.g = input[x, y];
                    exportTexture.SetPixel(x, y, pixelColor);
                }
            }

            exportTexture.Apply();

            // Write JPG
            if (exportJPG)
            {
                ExportJPG(imageName, exportTexture);
            }

            // Write PNG
            if (exportPNG)
            {
                ExportPNG(imageName, exportTexture);
            }

            //Lose the texture
            DestroyImmediate(exportTexture);
        }

        /// <summary>
        /// Compress / encode a multi layer map file to an image
        /// </summary>
        /// <param name="input">Multi layer map in format x,y,layer</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToMultiChannelFileImage(float[,,] input, string imageName, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool exportPNG = true, bool exportJPG = true)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            int layers = input.GetLength(2);
            int images = (layers + 3) / 4;

            for (int image = 0; image < images; image++)
            {
                Texture2D exportTexture = new Texture2D(width, width, imageStorageFormat, false);
                Color pixelColor = new Color();
                int layer = image * 4;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        pixelColor.r = layer < layers ? input[x, y, layer] : 0f;
                        pixelColor.g = (layer + 1) < layers ? input[x, y, (layer + 1)] : 0f;
                        pixelColor.b = (layer + 2) < layers ? input[x, y, (layer + 2)] : 0f;
                        pixelColor.a = (layer + 3) < layers ? input[x, y, (layer + 3)] : 0f;
                        exportTexture.SetPixel(x, y, pixelColor);
                    }
                }
                exportTexture.Apply();

                // Write JPG
                if (exportJPG)
                {
                    byte[] jpgBytes = exportTexture.EncodeToJPG();
                    Gaia.Utils.WriteAllBytes(imageName + image + ".jpg", jpgBytes);
                }

                // Write PNG
                if (exportPNG)
                {
                    byte[] pngBytes = exportTexture.EncodeToPNG();
                    Gaia.Utils.WriteAllBytes(imageName + image + ".png", pngBytes);
                }

                //Lose the texture
                DestroyImmediate(exportTexture);
            }
        }

        /// <summary>
        /// Compress / encode a multi layer map file to an image
        /// </summary>
        /// <param name="input">Multi layer map in format x,y,layer</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToMultiChannelFileImage(string imageName, HeightMap r, HeightMap g, HeightMap b, HeightMap a, TextureFormat imageStorageFormat, GaiaConstants.ImageFileType imageFileType)
        {
            int width = 0;
            int height = 0;

            if (r != null)
            {
                width = r.Width();
                height = r.Depth();
            }
            else if (g != null)
            {
                width = g.Width();
                height = g.Depth();
            }
            else if (b != null)
            {
                width = b.Width();
                height = b.Depth();
            }
            else if (a != null)
            {
                width = a.Width();
                height = a.Depth();
            }

            if (string.IsNullOrEmpty(imageName))
            {
                Debug.LogError("Cannot write image - no name supplied!");
                return;
            }

            if (width == 0 || height == 0)
            {
                Debug.LogError("Cannot write image - invalid dimensions : " + width + ", " + height);
                return;
            }

            Texture2D exportTexture = new Texture2D(width, height, imageStorageFormat, true, false);
            Color pixelColor = new Color();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixelColor.r = r != null ? r[x, y] : 0f;
                    pixelColor.g = g != null ? g[x, y] : 0f;
                    pixelColor.b = b != null ? b[x, y] : 0f;
                    pixelColor.a = a != null ? a[x, y] : 1f;
                    exportTexture.SetPixel(x, y, pixelColor);
                }
            }
            exportTexture.Apply();

#if UNITY_2017_1_OR_NEWER
            switch (imageFileType)
            {
                case GaiaConstants.ImageFileType.Jpg:
                    byte[] jpgBytes = ImageConversion.EncodeToJPG(exportTexture, 100);
                    Gaia.Utils.WriteAllBytes(imageName + ".jpg", jpgBytes);
                    break;
                case GaiaConstants.ImageFileType.Png:
                    byte[] pngBytes = ImageConversion.EncodeToPNG(exportTexture);
                    Gaia.Utils.WriteAllBytes(imageName + ".png", pngBytes);
                    break;
                case GaiaConstants.ImageFileType.Exr:
                    byte[] exrBytes = ImageConversion.EncodeToEXR(exportTexture, Texture2D.EXRFlags.CompressZIP);
                    Gaia.Utils.WriteAllBytes(imageName + ".exr", exrBytes);
                    break;
            }
#else
            switch (imageFileType)
            {
                case GaiaConstants.ImageFileType.Jpg:
                    byte[] jpgBytes = exportTexture.EncodeToJPG();
                    Gaia.Utils.WriteAllBytes(imageName + ".jpg", jpgBytes);
                    break;
                case GaiaConstants.ImageFileType.Png:
                    byte[] pngBytes = exportTexture.EncodeToPNG();
                    Gaia.Utils.WriteAllBytes(imageName + ".png", pngBytes);
                    break;
                case GaiaConstants.ImageFileType.Exr:
                    byte[] exrBytes = exportTexture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
                    Gaia.Utils.WriteAllBytes(imageName + ".exr", exrBytes);
                    break;
            }
#endif

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            //Lose the texture
            DestroyImmediate(exportTexture);
        }


        /// <summary>
        /// Convert the supplied texture to an array based on grayscale value
        /// </summary>
        /// <param name="texture">Input texture - must be read enabled</param>
        /// <returns>Texture as grayscale array</returns>
        public static float[,] ConvertTextureToArray(Texture2D texture)
        {
            float[,] array = new float[texture.width, texture.height];
            for (int x = 0; x < texture.width; x++)
            {
                for (int z = 0; z < texture.height; z++)
                {
                    array[x, z] = texture.GetPixel(x, z).grayscale;
                }
            }
            return array;
        }


        /// <summary>
        /// Decompress a single channel from the provided file into a float array.
        /// </summary>
        /// <param name="fileName">File to process</param>
        /// <param name="channelR">Take data from R channel</param>
        /// <param name="channelG">Take data from G channel</param>
        /// <param name="channelB">Take data from B channel</param>
        /// <param name="channelA">Take data from A channel</param>
        /// <returns>Array of float values from the selected channel</returns>
        public static float[,] DecompressFromSingleChannelFileImage(string fileName, int width, int height, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool channelR = true, bool channelG = false, bool channelB = false, bool channelA = false)
        {
            float[,] retArray = null;

            if (System.IO.File.Exists(fileName))
            {
                byte[] bytes = Gaia.Utils.ReadAllBytes(fileName);
                Texture2D importTexture = new Texture2D(width, height, imageStorageFormat, false);
                importTexture.LoadImage(bytes);
                retArray = new float[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).r;
                    }
                }
                //Lose the texture
                DestroyImmediate(importTexture);
            }
            else
            {
                Debug.LogError("Unable to find " + fileName);
            }
            return retArray;
        }

        /// <summary>
        /// Decompress a single channel from the provided file into a float array.
        /// </summary>
        /// <param name="fileName">File to process</param>
        /// <param name="channelR">Take data from R channel</param>
        /// <param name="channelG">Take data from G channel</param>
        /// <param name="channelB">Take data from B channel</param>
        /// <param name="channelA">Take data from A channel</param>
        /// <returns>Array of float values from the selected channel</returns>
        public static float[,] DecompressFromSingleChannelTexture(Texture2D importTexture, bool channelR = true, bool channelG = false, bool channelB = false, bool channelA = false)
        {
            if ((importTexture == null) || importTexture.width <= 0 || importTexture.height <= 0)
            {
                Debug.LogError("Unable to import from texture");
                return null;
            }

            float[,] retArray = new float[importTexture.width, importTexture.height];

            if (channelR)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).r;
                    }
                }
            }
            else if (channelG)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).g;
                    }
                }
            }
            else if (channelB)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).b;
                    }
                }
            }
            if (channelA)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).a;
                    }
                }
            }
            return retArray;
        }

        /// <summary>
        /// Export a texture to jpg
        /// </summary>
        /// <param name="fileName">File name to us - will have .jpg appended</param>
        /// <param name="texture">Texture source</param>
        public static void ExportJPG(string fileName, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToJPG();
            Gaia.Utils.WriteAllBytes(fileName + ".jpg", bytes);
        }

        /// <summary>
        /// Export a texture to png
        /// </summary>
        /// <param name="fileName">File name to us - will have .png appended</param>
        /// <param name="texture">Texture source</param>
        public static void ExportPNG(string fileName, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            Gaia.Utils.WriteAllBytes(fileName + ".png", bytes);
        }

        /// <summary>
        /// Will import the raw file provided - it assumes that it is in a square 16 bit PC format
        /// </summary>
        /// <param name="fileName">Fully qualified file name</param>
        /// <returns>File contents or null</returns>
        public static float[,] LoadRawFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate heightmap file : " + fileName);
                return null;
            }

            float[,] heights = null;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    int mapSize = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length / 2));
                    heights = new float[mapSize, mapSize];
                    for (int x = 0; x < mapSize; x++)
                    {
                        for (int y = 0; y < mapSize; y++)
                        {
                            heights[x, y] = (float)(br.ReadUInt16() / 65535.0f);
                        }
                    }
                }
                fileStream.Close();
            }

            return heights;
        }


        #endregion

        #region Mesh helpers

        /// <summary>
        /// Create a mesh for the heightmap
        /// </summary>
        /// <param name="heightmap"></param>
        /// <param name="targetSize"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static Mesh CreateMesh(float[,] heightmap, Vector3 targetSize)
        {
            //Need to sample these to not blow unity mesh sizes
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            int targetRes = 1;

            Vector3 targetOffset = Vector3.zero - (targetSize / 2f);
            Vector2 uvScale = new Vector2(1.0f / (width - 1), 1.0f / (height - 1));

            //Choose best possible target res
            for (targetRes = 1; targetRes < 100; targetRes++)
            {
                if (((width / targetRes) * (height / targetRes)) < 65000)
                {
                    break;
                }
            }

            targetSize = new Vector3(targetSize.x / (width - 1) * targetRes, targetSize.y, targetSize.z / (height - 1) * targetRes);
            width = (width - 1) / targetRes + 1;
            height = (height - 1) / targetRes + 1;

            Vector3[] vertices = new Vector3[width * height];
            Vector2[] uvs = new Vector2[width * height];
            Vector3[] normals = new Vector3[width * height];
            Color[] colors = new Color[width * height];
            int[] triangles = new int[(width - 1) * (height - 1) * 6];

            // Build vertices and UVs
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[y * width + x] = Color.black;
                    normals[y * width + x] = Vector3.up;
                    //vertices[y * w + x] = Vector3.Scale(targetSize, new Vector3(-y, heightmap[x * tRes, y * tRes], x)) + targetOffset;
                    vertices[y * width + x] = Vector3.Scale(targetSize, new Vector3(x, heightmap[x * targetRes, y * targetRes], y)) + targetOffset;
                    uvs[y * width + x] = Vector2.Scale(new Vector2(x * targetRes, y * targetRes), uvScale);
                }
            }

            // Build triangle indices: 3 indices into vertex array for each triangle
            int index = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    triangles[index++] = (y * width) + x;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = (y * width) + x + 1;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = ((y + 1) * width) + x + 1;
                    triangles[index++] = (y * width) + x + 1;
                }
            }

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        /// <summary>
        /// Return the bounds of both the object and any colliders it has
        /// </summary>
        /// <param name="go">Game object to check</param>
        public static Bounds GetBounds(GameObject go)
        {
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                bounds.Encapsulate(c.bounds);
            }
            return bounds;
        }

        #endregion

        #region Direction helpers

        /// <summary>
        /// Rotate a direction vector left 90% around X axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftXAxis(Vector3 input)
        {
            return new Vector3(input.x, -input.z, input.y);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around X axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightXAxis(Vector3 input)
        {
            return new Vector3(input.x, input.z, -input.y);
        }

        /// <summary>
        /// Rotate a direction vector left 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftYAxis(Vector3 input)
        {
            return new Vector3(-input.z, input.y, input.x);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightYAxis(Vector3 input)
        {
            return new Vector3(input.z, input.y, -input.x);
        }

        /// <summary>
        /// Rotate a direction vector left 90% around Z axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftZAxis(Vector3 input)
        {
            return new Vector3(input.y, -input.x, input.z);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightZAxis(Vector3 input)
        {
            return new Vector3(-input.y, input.x, input.z);
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
        /// Write a scriptable object out into a new asset that can be shared
        /// </summary>
        /// <typeparam name="T">The scriptable object to be saved as an asset</typeparam>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
#if UNITY_EDITOR
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
#endif
        }

        /// <summary>
        /// Get the path of the unity object supplied
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        public static string GetAssetPath(UnityEngine.Object uo)
        {
            string path = "";
#if UNITY_EDITOR
            path = Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(uo));
            path = path.Replace("/Assets", "");
            path = path.Replace("\\", "/");
#endif
            return path;
        }

        /// <summary>
        /// Wrap the scriptable object up so that it can be transferred without causing unity errors
        /// </summary>
        /// <param name="so"></param>
        public static string WrapScriptableObject(ScriptableObject so)
        {
            string newpath = "";
#if UNITY_EDITOR
            string path = GetAssetPath(so);
            if (File.Exists(path))
            {
                newpath = Path.ChangeExtension(path, "bytes");
                UnityEditor.FileUtil.CopyFileOrDirectory(path, newpath);
            }
            else
            {
                Debug.LogError("There is no file at the path supplied: " + path);
            }
#endif
            return newpath;
        }


        public static void UnwrapScriptableObject(string path, string newpath)
        {
#if UNITY_EDITOR
            if (File.Exists(path))
            {
                if (!File.Exists(newpath))
                {
                    UnityEditor.FileUtil.CopyFileOrDirectory(path, newpath);
                }
                else
                {
                    Debug.LogError("There is already a file with this name at the path supplied: " + newpath);
                }
            }
            else
            {
                Debug.LogError("There is no file at the path supplied: " + path);
            }
#endif
        }

        public static string WrapGameObjectAsPrefab(GameObject go)
        {
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            string name = go.name;
            UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(new GameObject(), "Assets/" + name + ".prefab");
            PrefabUtility.SavePrefabAsset(go);
            AssetDatabase.Refresh();
            return AssetDatabase.GetAssetPath(prefab);
#else
            string name = go.name;
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
            PrefabUtility.ReplacePrefab(go, prefab);
            AssetDatabase.Refresh();
            return AssetDatabase.GetAssetPath(prefab);
#endif
#else
            return "";
#endif
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
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string fileName)
        {
#if UNITY_EDITOR
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string[] assets = AssetDatabase.FindAssets(fName, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == fileName)
                {
                    return path;
                }
            }
#endif
            return "";
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <param name="name">Type to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string name, string type)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            string[] file;
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                //Make sure its an exact match
                file = Path.GetFileName(path).Split('.');
                if (file.GetLength(0) != 2)
                {
                    continue;
                }
                if (file[0] != name)
                {
                    continue;
                }
                if (file[1] != type)
                {
                    continue;
                }
                return path;
            }
#endif
            return "";
        }

        /// <summary>
        /// Return GaiaSettings or null;
        /// </summary>
        /// <returns>Gaia settings or null if not found</returns>
        public static GaiaSettings GetGaiaSettings()
        {
            return GetAsset("GaiaSettings.asset", typeof(Gaia.GaiaSettings)) as Gaia.GaiaSettings;
        }

        /// <summary>
        /// Returns the first asset that matches the file path and name passed. Will try
        /// full path first, then will try just the file name.
        /// </summary>
        /// <param name="fileNameOrPath">File name as standalone or fully pathed</param>
        /// <returns>Object or null if it was not found</returns>
        public static UnityEngine.Object GetAsset(string fileNameOrPath, Type assetType)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(fileNameOrPath))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(fileNameOrPath, assetType);
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    string path = Utils.GetAssetPath(Path.GetFileName(fileNameOrPath));
                    if (!string.IsNullOrEmpty(path))
                    {
                        return AssetDatabase.LoadAssetAtPath(path, assetType);
                    }
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first prefab that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static GameObject GetAssetPrefab(string name)
        {
#if UNITY_EDITOR
            string path = Utils.GetAssetPath(name, "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first scriptable that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static ScriptableObject GetAssetScriptableObject(string name)
        {
#if UNITY_EDITOR
            string path = Utils.GetAssetPath(name, "asset");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first texture that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the texture or null</returns>
        public static Texture2D GetAssetTexture2D(string name)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (path.Contains(".jpg") || path.Contains(".psd") || path.Contains(".png"))
                {
                    //Make sure its an exact match
                    string filename = Path.GetFileNameWithoutExtension(path);
                    if (filename == name)
                    {
                        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    }
                }
            }
#endif
            return null;
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