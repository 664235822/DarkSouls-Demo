using UnityEngine;

namespace GaiaCommon1
{
    [System.Serializable]
    public class UBrush
    {
        private float[] m_Strength;
        private Texture2D m_Brush;
        private const int MIN_BRUSH_SIZE = 3;
        
        public int Size { get; private set; }

        /// <summary>
        /// Load texture at the given size. Returns false if the texture is null.
        /// </summary>
        /// <returns>Returns false if the texture is null.</returns>
        public bool Load(Texture2D brushTex, int size)
        {
            if ((Object)m_Brush == (Object)brushTex && size == Size && m_Strength != null)
            {
                return true;
            }

            if ((Object)brushTex != (Object)null)
            {
                float sizeF = (float)size;
                Size = size;
                m_Strength = new float[Size * Size];
                if (Size > 3)
                {
                    for (int index1 = 0; index1 < Size; ++index1)
                    {
                        for (int index2 = 0; index2 < Size; ++index2)
                            m_Strength[index1 * Size + index2] = brushTex.GetPixelBilinear(((float)index2 + 0.5f) / sizeF, (float)index1 / sizeF).a;
                    }
                }
                else
                {
                    for (int index = 0; index < m_Strength.Length; ++index)
                        m_Strength[index] = 1f;
                }
                m_Brush = brushTex;
                return true;
            }
            m_Strength = new float[1];
            m_Strength[0] = 1f;
            Size = 1;
            return false;
        }

        /// <summary>
        /// Get brush strength at the given coords.
        /// </summary>
        public float GetStrengthAtCoords(int ix, int iy)
        {
            if (ix < 0 || Size <= ix || iy < 0 || Size <= iy)
            {
                return 0f;
            }
            //ix = Mathf.Clamp(ix, 0, Size - 1);
            //iy = Mathf.Clamp(iy, 0, Size - 1);
            return m_Strength[iy * Size + ix];
        }
    }
}
