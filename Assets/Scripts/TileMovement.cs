using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle
{
    public class TileMovement : MonoBehaviour
    {
        public Tile Tile { get; set; }

        private Vector3 GetCorrectPosition()
        {
            return new Vector3(Tile.XIndex * Tile.TileSize, Tile.YIndex * Tile.TileSize, 0.0f);
        }

        private Vector3 m_offset = new Vector3(0.0f, 0.0f, 0.0f);
        private SpriteRenderer m_spriteRenderer;

        void Start()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Hit piece. So disable the camera panning.
            CameraMovement.CameraPanning = false;

            Tile.TilesSorting.BringToTop(m_spriteRenderer);
            m_offset = transform.position - Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        }

        void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + m_offset;
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

            // Enable back the camera panning.
            CameraMovement.CameraPanning = true;
        }
    }
}