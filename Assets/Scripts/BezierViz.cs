using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierViz : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pointPrefab;
    [SerializeField]
    private Color m_bezierCurveColor;
    [SerializeField]
    private Color m_lineColor;
    [SerializeField]
    private float m_lineWidth = 0.05f;

    private List<Vector2> m_controlPoints;
    private List<GameObject> m_points;
    private LineRenderer m_lineRenderer;
    private LineRenderer m_curveRenderer;

    void Start()
    {
        // Create the two LineRenderers.
        m_lineRenderer = CreateLine("Line Renderer", m_lineColor);
        m_curveRenderer = CreateLine("Curve Line Renderer", m_bezierCurveColor);

        // Create the instances of PointPrefab
        // to show the control points.
        for (int i = 0; i < m_controlPoints.Count; ++i)
        {
            GameObject obj = Instantiate(m_pointPrefab,
              m_controlPoints[i],
              Quaternion.identity);
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
}
