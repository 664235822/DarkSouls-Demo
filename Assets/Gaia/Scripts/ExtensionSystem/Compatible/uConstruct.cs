using UnityEngine;
using System.Collections;

namespace Gaia.GXC.EEProductions
{
    /// <summary>
    /// Description for village exteriors kit
    /// </summary>
    public class uConstruct : MonoBehaviour
    {
        /// <summary>
        /// Publisher name
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "EE Productions";
        }

        /// <summary>
        /// Package name 
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "uConstruct - Runtime Building System";
        }

        /// <summary>
        /// Package image
        /// </summary>
        /// <returns>Package image</returns>
        public static string GetPackageImage()
        {
            return "uConstruct";
        }

        /// <summary>
        /// Package description
        /// </summary>
        /// <returns>Package description</returns>
        public static string GetPackageDescription()
        {
            return @"Do you want your game players to be able to construct their own buildings and structures? 

uConstruct is an easy-to-implement, run-time, socket-based building system.
uConstruct can save any of the created structures for restoration.
This capability also allows developers to use uConstruct as part of their level design/creation toolkit. 

FEATURES:

Socket based construction: Building components snap into position in snap sockets, or free placed on the socket bounds on free placed sockets, while allowing visual scaling and rotation.

Conditional placement: Ability to set the conditions for building placement within your worlds. Four very useful examples are included that you can extend for your requirements.

Draw call batching: uConstruct can reduce draw calls from thousands down to a few. The performance from batching eliminates the need for creating LOD views.

Area of interest influence: this multi-threaded subsystem of uConstruct allows you define the players influence on Unity physics, sockets and conditionals based on the player proximity to the area. Eliminating contention with the Unity main thread and optimizing your collider count.

Templates: Through the use of templates you can predefine sockets and conditions and apply them to any of the structures. With a template you can uniformly impact as many structures as you want instead of adjusting the same settings across multiple structures.

Prefab structure database: Each structure is assigned its own unique ID and enables the ability to access the structure's prefab at run-time. This is very useful for networking, pooling systems, etc.

Custom physics: uConstruct has a socket optimized physics sub-system that greatly improves performance. Utilizing this feature does not preclude you from using Unity’s default physics system.

Structure saving: Built-in structure saving system that is fully extendable for your own data needs.

Building types code generator: You can create your own building types within the editor without the requirement of having to adjust the source code.

Smart optimizations: uConstruct will detect and implement many optimizations at run-time. For example, it will disable used or overlapping sockets.

Fully Functional API: The API extends your flexibility to only be limited by your or your player’s creativity.

Complete documentation.

Video tutorials.

Full source code included.";
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <returns>Package URL</returns>
        public static string GetPackageURL()
        {
            return "https://www.assetstore.unity3d.com/en/#!/content/51881";
        }
    }
}