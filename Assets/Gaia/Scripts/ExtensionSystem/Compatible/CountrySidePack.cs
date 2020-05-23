using UnityEngine;
using System.Collections;

namespace Gaia.GXC.PolyPixel
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class CountrySidePack : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "PolyPixel";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "CountrySide Pack";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "CountrySidePack";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"CountrySide V2.1 is a starter kit to create your own beautiful & lush nature environment.

Assets include: pre­built cabins, trees (chestnut, maple, beech, etc.), ground plants (flowers, weeds, grass), boulders, terrain textures, and modular fences. 

All trees feature a high­fidelity and lower poly version to cater towards your platform.";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/34483";
        }
    }
}