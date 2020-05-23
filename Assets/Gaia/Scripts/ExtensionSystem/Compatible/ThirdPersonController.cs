using UnityEngine;

namespace Gaia.GXC.Opsive
{
    /// <summary>
    /// Description for Third Person Controller
    /// </summary>
    public class ThirdPersonController : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Opsive";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Third Person Controller";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "ThirdPersonController";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"This is so much more than just a smooth and flexible character controller. The Third Person Controller is your ultimate framework for creating ANY 3rd person game.

Featuring a character and camera controller, combat system, inventory management, and much more! Thousands of hours have gone into developing this framework so you can focus on the unique aspects of your game.

Use the editor scripts to create your character and items in seconds. Easily add new animations with the unique ability system.

Designed to scale, the Third Person Controller comes with mocap animations, mobile and Unity 5 multiplayer support. Any model (humanoid or generic) can be used. It is also integrated with many assets, including Behavior Designer to add life to your AI characters. ";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/27438";
        }
    }
}