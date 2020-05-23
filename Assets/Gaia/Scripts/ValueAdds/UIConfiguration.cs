using UnityEngine;
using UnityEngine.UI;

namespace Gaia
{
    [ExecuteInEditMode]
    public class UIConfiguration : MonoBehaviour
    {
        #region Variables
        [Header("UI Settings")]
        [Tooltip("Sets the UI text color")]
        public Color32 m_uiTextColor = new Color32(255, 255, 255, 255);
        [Tooltip("Button used to toggle the UI on and off")]
        public KeyCode m_uiToggleButton = KeyCode.U;
        private Text m_textContent;

        private Color32 storedColor;
        private bool storedUIStatus = true;
        #endregion

        #region UI Text Setup
        /// <summary>
        /// Starting function setup
        /// </summary>
        void Start()
        {
            storedColor = m_uiTextColor;

            if (m_textContent != null)
            {
                m_textContent.color = storedColor;
            }
        }

        void OnEnable()
        {
            if (m_textContent == null)
            {
                GameObject controlTextGO = GameObject.Find("Control Text");
                if (controlTextGO != null)
                {
                    m_textContent = controlTextGO.GetComponent<Text>();
                }
            }
        }

        void LateUpdate()
        {
            storedColor = m_uiTextColor;

            if (storedUIStatus && Input.GetKeyDown(m_uiToggleButton))
            {
                m_textContent.enabled = false;
                storedUIStatus = false;
            }

            else if (!storedUIStatus && Input.GetKeyDown(m_uiToggleButton))
            {
                m_textContent.enabled = true;
                storedUIStatus = true;
            }

            if (m_textContent.color != storedColor)
            {
                if (m_textContent != null)
                {
                    m_textContent.color = storedColor;
                }
            }
        }
        #endregion
    }
}