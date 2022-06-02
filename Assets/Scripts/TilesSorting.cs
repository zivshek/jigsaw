using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class TilesSorting
    {
        private List<SpriteRenderer> m_sortIndices = new List<SpriteRenderer>();

        public TilesSorting()
        {

        }

        public void Clear()
        {
            m_sortIndices.Clear();
        }

        public void Add(SpriteRenderer ren)
        {
            m_sortIndices.Add(ren);
            SetRenderOrder(ren, m_sortIndices.Count);
        }

        public void Remove(SpriteRenderer ren)
        {
            m_sortIndices.Remove(ren);
            for (int i = 0; i < m_sortIndices.Count; ++i)
            {
                SetRenderOrder(m_sortIndices[i], i + 1);
            }
        }
        public void BringToTop(SpriteRenderer ren)
        {
            Remove(ren);
            Add(ren);
        }

        private void SetRenderOrder(SpriteRenderer ren, int order)
        {
            // First we set the render order of sorting.
            ren.sortingOrder = order;

            // Then we set the z value so that selection/raycast 
            // selects the top sprite.
            Vector3 p = ren.transform.position;
            p.z = -order / 10.0f;
            ren.transform.position = p;
        }
    }
}