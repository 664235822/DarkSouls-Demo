using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// Base growth class - can be assigned to a game object to cause it to grow over a given range.
    /// Implemeneted as virtuals so that you can derive more sophisticated behavior from it.
    /// </summary>
    public class Growth : MonoBehaviour
    {

        [Range(0.1f, 2f), Tooltip("The start size in the game.")]
        public float m_startScale = 0.15f;
        [Range(0.1f, 2f), Tooltip("The end size in the game.")]
        public float m_endScale = 1.0f;
        [Range(0f, 2f), Tooltip("Scale variance. Final scale is equal to end scale plus a a random value between 0 and this.")]
        public float m_scaleVariance = 0.25f;
        [Range(0.5f, 60f), Tooltip("The time it takes to grow in seconds.")]
        public float m_growthTime = 5.0f;

        /// <summary>
        /// The actual end scale being used
        /// </summary>
        private float m_actualEndScale = 0f;

        void Start()
        {
            Initialise();
        }

        /// <summary>
        /// Initialise this agent.
        /// </summary>
        public virtual void Initialise()
        {
            //Randomly choose an end scale
            m_actualEndScale = m_endScale + Random.Range(0f, m_scaleVariance);

            //Update the scale of the agent
            StartCoroutine(Grow());
        }

        /// <summary>
        /// Grow this agent in a co-routine. Its a one shot thing.
        /// </summary>
        IEnumerator Grow()
        {
            //Set growth params
            float scale;
            float startTime = Time.realtimeSinceStartup;
            float currentTime = startTime;
            float deltaScale = m_actualEndScale - m_startScale;
            float finishTime = startTime + m_growthTime;
            while (currentTime < finishTime)
            {
                //Update scale
                scale = 1f - ((finishTime - currentTime) / m_growthTime);
                scale = m_startScale + (scale * deltaScale);

                //Apply it to the game object
                gameObject.transform.localScale = Vector3.one * scale;
                yield return null;

                //yield return new WaitForSeconds(0.1f); // Can use this to lessen impact on fps

                //Update time
                currentTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// Kill this instance.
        /// </summary>
        public virtual void Die()
        {
            //Destroy ourselves
            Destroy(gameObject, 5f);
        }
    }
}