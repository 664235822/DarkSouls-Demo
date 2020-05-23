using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// This class is used to get information about a location on the terrain. Used heavily by the spawning system.
    /// </summary>
    public class SpawnInfo
    {
        public Spawner m_spawner;           //The spawner that populated this
        public bool m_outOfBounds = true;   //Was this out of bounds or not
        public bool m_wasVirginTerrain;     //If virgin terrain was hit at this location - hitting a terrain tree is counted as false
        public float m_spawnRotationY;      //The rotation chosen for the spawn
        public float m_hitDistanceWU;       //The distance from the centre of the spawner to this location in unity world units
        public Vector3 m_hitLocationWU;     //The location of the hit in unity world units
        public Vector3 m_hitLocationNU;     //The location on the hit in normalised units, with respect to the terrain it was located in
        public Vector3 m_hitNormal;         //The normal of the hit
        public Transform m_hitObject;       //The object that was hit
        public Terrain m_hitTerrain;        //The terrain that this happened on
        public float m_terrainHeightWU;     //The terrain height at this location in world units
        public float m_terrainSlopeWU;      //The terrian slope at this location in 0 (flat) .. 90 (vertical) degrees
        public Vector3 m_terrainNormalWU;   //The terrain normal at the hit location
        public float m_fitness;             //The fitness that was calculated for this location
        public float[] m_textureStrengths;  //The texture strenghts in the terrain at this location
        public Vector3[] m_areaHitsWU;      //Will be filled in when doing area based hits
        public float m_areaHitSlopeWU;      //Will be calculated when doing area based hits
        public float m_areaMinSlopeWU;      //Minimum slope detected, calculates when doing area based hits
        public float m_areaAvgSlopeWU;      //Average of slopes detected, calculates when doing area based hits
        public float m_areaMaxSlopeWU;      //Maximum slope detected, calculates when doing area based hits
    }
}