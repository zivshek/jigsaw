using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    [field:SerializeField]
    public float CameraSizeMin { get; private set; } = 150.0f;

    public static bool CameraPanning { get; set; } = true;

    private Vector3 m_dragPos;
    private Vector3 m_originalPosition;

    private float m_cameraSizeMax;
    private float m_zoomFactor = 0.0f;
    private Camera m_camera;

    void Start()
    {
        m_camera = Camera.main; // cache the camera
        m_cameraSizeMax = m_camera.orthographicSize; // set the max size
        m_originalPosition = m_camera.transform.position; // copy the original position.
    }

    public void RePositionCamera(int numTilesX, int numTilesY)
    {
        // We set the size of the camera. 
        // You can implement your own way of doing this.
        m_camera.orthographicSize = numTilesX < numTilesY ?
            numTilesX * 100 : numTilesY * 100;

        // Set the position of the camera to be at the
        // centre of the board.
        m_camera.transform.position = new Vector3(
            (numTilesX * 100 + 40) / 2,
            (numTilesY * 100 + 40) / 2,
            -1000.0f);

        m_cameraSizeMax = m_camera.orthographicSize;
        m_originalPosition = m_camera.transform.position;
    }

    public void Zoom(float value)
    {
        m_zoomFactor = value;
        m_zoomFactor = Mathf.Clamp01(m_zoomFactor);
        //mSliderZoom.value = mZoomFactor;

        m_camera.orthographicSize = m_cameraSizeMax - m_zoomFactor * (m_cameraSizeMax - CameraSizeMin);
    }

    public void ZoomIn()
    {
        Zoom(m_zoomFactor + 0.01f);
    }

    public void ZoomOut()
    {
        Zoom(m_zoomFactor - 0.01f);
    }

    public void ResetCameraView()
    {
        m_camera.transform.position = m_originalPosition;
        m_camera.orthographicSize = m_cameraSizeMax;
        m_zoomFactor = 0.0f;
        //mSliderZoom.value = 0.0f;
    }

    void Update()
    {
        // Camera panning is disabled when a tile is selected.
        if (!CameraPanning) return;

        // We also check if the pointer is not on UI item
        // or is disabled.
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }

        // Save the position in worldspace.
        if (Input.GetMouseButtonDown(0))
        {
            m_dragPos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 diff = m_dragPos - m_camera.ScreenToWorldPoint(Input.mousePosition);
            diff.z = 0.0f;
            m_camera.transform.position += diff;
        }
    }
}
