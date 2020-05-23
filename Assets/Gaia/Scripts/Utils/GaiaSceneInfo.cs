using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// Class that provides information about the current scene
    /// </summary>
    public class GaiaSceneInfo
    {
        public Bounds   m_sceneBounds = new Bounds();
        public Vector3  m_centrePointOnTerrain = Vector3.zero;
        public float    m_seaLevel = 0f;

        /// <summary>
        /// Get current basic scene information
        /// </summary>
        /// <returns></returns>
        public static GaiaSceneInfo GetSceneInfo()
        {
            GaiaSceneInfo sceneInfo = new GaiaSceneInfo();
            Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();

            if (terrain == null)
            {
                Debug.LogWarning("You must have a valid terrain for sceneinfo to work correctly.");
            }
            else
            {
                //Get or create a session in order to get a sea level
                GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();

                //Get the terrain bounds
                Gaia.TerrainHelper.GetTerrainBounds(terrain, ref sceneInfo.m_sceneBounds);

                //Get the sea level
                sceneInfo.m_seaLevel = sessionMgr.GetSeaLevel();

                //Grab the central point on the terrain - handy for placing player etc
                sceneInfo.m_centrePointOnTerrain = new Vector3(sceneInfo.m_sceneBounds.center.x, terrain.SampleHeight(sceneInfo.m_sceneBounds.center), sceneInfo.m_sceneBounds.center.z);
            }

            return sceneInfo;
        }
    }
}

