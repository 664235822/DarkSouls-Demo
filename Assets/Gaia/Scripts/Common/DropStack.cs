using UnityEngine;
using System;
using System.Collections.Generic;

namespace GaiaCommon1
{
    /// <summary>
    /// A limited size stack where the oldest items start "falling out at the bottom"
    /// when more items are added after the <see cref="Capacity"/> was reached.
    /// DO make sure that the content is serializable by unity if you want the stack to be serialized.
    /// </summary>
    [Serializable]
    public class DropStack<T> : System.Object
    {
        [SerializeField] private T[] m_items;
        [SerializeField] private int m_topIndex = 0;
        [SerializeField] private int m_count = 0;

        /// <summary>
        /// The max capacity of the stack.
        /// </summary>
        public int Capacity { get { return m_items.Length; } }

        /// <summary>
        /// The count of the items currently stored in the stack.
        /// </summary>
        public int Count { get { return m_count; } }

        /// <summary>
        /// Create a new drop stack.
        /// DO make sure that the content is serializable by unity if you want the stack to be serialized.
        /// </summary>
        /// <param name="capacity">Max capacity of the stack.</param>
        public DropStack(int capacity)
        {
            m_items = new T[capacity];
            m_count = 0;
        }

        /// <summary>
        /// Push an <paramref name="item"/> to the stack.
        /// DO make sure that the <paramref name="item"/> is serializable by unity if you want the stack to be serialized.
        /// </summary>
        public void Push(T item)
        {
            m_items[m_topIndex] = item;
            m_topIndex = (m_topIndex + 1) % m_items.Length;
            m_count = Count >= m_items.Length ? m_items.Length : Count + 1;
        }

        /// <summary>
        /// Push a bunch of <paramref name="items"/> to the stack.
        /// They will be pushed in a forward order, same as stepping through an array.
        /// DO make sure that the <paramref name="items"/> are serializable by unity if you want the stack to be serialized.
        /// </summary>
        public void Push(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Push(item);
            }
        }

        /// <summary>
        /// Pops an item from the stack and returns it.
        /// </summary>
        public T Pop()
        {
            m_topIndex = (m_items.Length + m_topIndex - 1) % m_items.Length;
            m_count = Count < 2 ? 0 : Count - 1;
            return m_items[m_topIndex];
        }

        /// <summary>
        /// Peeks the top item without popping it from the stack and returns it.
        /// </summary>
        public T Peek()
        {
            return m_items[(m_items.Length + m_topIndex - 1) % m_items.Length];
        }

    }
}