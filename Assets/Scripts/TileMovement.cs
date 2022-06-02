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
            return new Vector3(tile.XIndex * 100.0f, tile.YIndex * 100.0f, 0.0f);
        }

        private Vector3 mOffset = new Vector3(0.0f, 0.0f, 0.0f);

        // Start is called before the first frame update
        void Start()
        {
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