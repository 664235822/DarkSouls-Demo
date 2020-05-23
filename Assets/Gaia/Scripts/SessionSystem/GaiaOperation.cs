using UnityEngine;
using System.Collections;
using System;

namespace Gaia
{
    /// <summary>
    /// A gaia operation - serialises and deserialises and executes a gaia operation
    /// </summary>
    [System.Serializable]
    public class GaiaOperation
    {
        /// <summary>
        /// An optional description
        /// </summary>
        public string m_description;

        /// <summary>
        /// The types of operations we can record
        /// </summary>
        public enum OperationType { CreateTerrain, FlattenTerrain, SmoothTerrain, ClearDetails, ClearTrees, Stamp, StampUndo, StampRedo, Spawn, SpawnReset }

        /// <summary>
        /// The operation type
        /// </summary>
        public OperationType m_operationType;

        /// <summary>
        /// Whether or not the operation is active
        /// </summary>
        public bool m_isActive = true;

        /// <summary>
        /// The name of the object that generated this operation
        /// </summary>
        public string m_generatedByName;

        /// <summary>
        /// The ID of the onject that generated this operation
        /// </summary>
        public string m_generatedByID;

        /// <summary>
        /// The type of object that generated this operation
        /// </summary>
        public string m_generatedByType;

        /// <summary>
        /// When the operation was recorded
        /// </summary>
        public string m_operationDateTime = DateTime.Now.ToString();

        /// <summary>
        /// Data associated with this operation as a series of json strings
        /// </summary>
        [HideInInspector]
        public string [] m_operationDataJson = new string[0];

        /// <summary>
        /// Whether or not we are folded out in the editor
        /// </summary>
        public bool m_isFoldedOut = false;
    }
}