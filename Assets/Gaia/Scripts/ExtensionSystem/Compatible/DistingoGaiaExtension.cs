using UnityEngine;
using System.Collections;

namespace Gaia.GXC.RandomchaosLtd
{

    public class DistingoGaiaExtension : MonoBehaviour
    {

        #region Gaia Bits
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Randomchaos Ltd";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Distingo - Terrain in Detail";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "DistingoGaiaImage";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"Distingo – Bringing ever increasing detail to your teerrain.

Alter terrain splatting distance.
Regulate texture tialing based on distance from the camera.
Alter individual textures:-
    Near and Far UV Multipliers
    Normal map power
    Smoothness
    Metallic
";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/#!/content/54737";
        }
        #endregion
    }
}