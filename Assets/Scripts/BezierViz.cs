using System.Collections.Generic;
using UnityEngine;

public class BezierViz : MonoBehaviour
{
    public List<Vector2> ControlPoints = new List<Vector2>()
    {
        new Vector2(0, 0),
        new Vector2(35, 15),
        new Vector2(47, 13),
        new Vector2(45, 5),
        new Vector2(48, 0),
        new Vector2(25, -5),
        new Vector2(15, -18),
        new Vector2(36, -20),
        new Vector2(64, -20),
        new Vector2(85, -18),
        new Vector2(75, -5),
        new Vector2(52, 0),
        new Vector2(55, 5),
        new Vector2(53, 13),
        new Vector2(65, 15),
        new Vector2(100, 0)
    };

    [SerializeField]
    private GameObject m_pointPrefab;
    [SerializeField]
    private Color m_bezierCurveColor;
    [SerializeField]
    private Color m_lineColor;
    [SerializeField]
    private float m_lineWidth = 0.05f;

    private List<GameObject> m_points = new List<GameObject>();
    private LineRenderer m_lineRenderer;
    private LineRenderer m_curveRenderer;

    void Start()
    {
        // Create the two LineRenderers.
        m_lineRenderer = CreateLine("Line Renderer", m_lineColor);
        m_curveRenderer = CreateLine("Curve Line Renderer", m_bezierCurveColor);

        // Create the instances of PointPrefab
        // to show the control points.
        for (int i = 0; i < ControlPoints.Count; ++i)
        {
            GameObject obj = Instantiate(m_pointPrefab, ControlPoints[i], Quaternion.identity);
            obj.name = "ControlPoint_" + i.ToString();
            m_points.Add(obj);
        }
    }

    void Update()
    {
        List<Vector2> pts = new List<Vector2>();

        for (int k = 0; k < m_points.Count; ++k)
        {
            pts.Add(m_points[k].transform.position);
        }

        // create a line renderer for showing the straight
        // lines between control points.
        m_lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < pts.Count; ++i)
        {
            m_lineRenderer.SetPosition(i, pts[i]);
        }

        // we take the control points from the list of points in the scene.
        // recalculate points every frame.
        List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
        m_curveRenderer.positionCount = curve.Count;
        for (int i = 0; i < curve.Count; ++i)
        {
            m_curveRenderer.SetPosition(i, curve[i]);
        }
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            if (e.clickCount == 2 && e.button == 0)
            {
                Vector2 rayPos = new Vector2(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                InsertNewControlPoint(rayPos);
            }
        }
    }

    private LineRenderer CreateLine(string name, Color color)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = m_lineWidth;
        lr.endWidth = m_lineWidth;
        return lr;
    }

    private void InsertNewControlPoint(Vector2 p)
    {
        if (m_points.Count >= 16)
        {
            Debug.Log("Cannot create any new control points. Max number is 16");
            return;
        }
        GameObject obj = Instantiate(m_pointPrefab, p, Quaternion.identity);
        obj.name = "ControlPoint_" + m_points.Count.ToString();
        m_points.Add(obj);
    }
}
