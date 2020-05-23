using UnityEngine;
using System.Collections;

namespace Gaia.GXC.GameTexturesCom
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class GameTexturesCom : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "GameTextures.com";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Game Textures";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "GameTexturesCom";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"GameTextures.com is the most beautiful library of game-ready PBR Materials on the Internet.";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "http://www.gametextures.com/";
        }
    }
}