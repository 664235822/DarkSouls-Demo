using UnityEngine;
using System.Collections;

namespace Gaia
{
    public class SpawnRuleExtension : MonoBehaviour
    {
        /// <summary>
        /// Initialise the extension
        /// </summary>
        public virtual void Initialise()
        {
        }


        /// <summary>
        /// Whether or not this extension impacts textures
        /// </summary>
        /// <returns></returns>
        public virtual bool AffectsTextures()
        {
            return false;
        }

        /// <summary>
        /// Whether or not this extension impacts details
        /// </summary>
        /// <returns></returns>
        public virtual bool AffectsDetails()
        {
            return false;
        }


        /// <summary>
        /// Call this to update fitness of the location spawnInfo passed in
        /// </summary>
        /// <param name="spawnInfo">Use this to get details about the location and update the fitness</param>
        /// <returns>Return the fitness of the location</returns>
        public virtual float GetFitness(float fitness, ref SpawnInfo spawnInfo)
        {
            return fitness;
        }

        /// <summary>
        /// Whether or not this extension should override the spawn itself
        /// </summary>
        /// <param name="spawnInfo"></param>
        /// <returns></returns>
        public virtual bool OverridesSpawn(SpawnRule spawnRule, ref SpawnInfo spawnInfo)
        {
            return false;
        }


        /// <summary>
        /// Spawn an instance at the location provided. 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual void Spawn(SpawnRule spawnRule, ref SpawnInfo spawnInfo)
        {

        }

        /// <summary>
        /// Call this to do any post spawn goodness
        /// </summary>
        public virtual void PostSpawn(SpawnRule spawnRule, ref SpawnInfo spawnInfo)
        {

        }
    }
}
