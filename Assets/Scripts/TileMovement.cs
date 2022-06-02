using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle
{
    public class TileMovement : MonoBehaviour
    {
        public Tile tile { get; set; }

        private Vector3 GetCorrectPosition()
        {
            return new Vector3(tile.XIndex * Tile.TileSize, tile.YIndex * Tile.TileSize, 0.0f);
        }

        private Vector3 mOffset = new Vector3(0.0f, 0.0f, 0.0f);
        private SpriteRenderer m_spriteRenderer;

        // Start is called before the first frame update
        void Start()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Tile.TilesSorting.BringToTop(m_spriteRenderer);
            mOffset = transform.position - Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        }

        void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;
            transform.position = curPosition;
        }

        void OnMouseUp()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            float dist = (transform.position - GetCorrectPosition()).magnitude;
            if (dist < 20.0f)
            {
                transform.position = GetCorrectPosition();
            }
        }
    }
}