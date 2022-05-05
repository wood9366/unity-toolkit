using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public bool _isShowAxis = false;
    public float _lenAxis = 0.1f;
    public float _width = 5;
    public float _handleSize = 1;
    public Vector3[] _points = new Vector3[]
    {
        Vector3.zero,
        new Vector3(0, 1, 0),
        new Vector3(2, -1, 0),
        new Vector3(2, 0, 0),
    };

    public const int NUM_BEZIER_SEGMENTS = 30;
    private Vector3[] _bezierPoints = new Vector3[NUM_BEZIER_SEGMENTS + 1];

    void Start()
    {
        UpdateBezierPoints();
    }

    void OnDrawGizmos()
    {
        DrawBezier(Color.white);
    }

    public void UpdateBezierPoints()
    {
        for (int i = 0; i <= NUM_BEZIER_SEGMENTS; i++)
        {
            var t = (float)i / NUM_BEZIER_SEGMENTS;

            _bezierPoints[i] = BezierUtil.BezierPoint(t, _points);
        }
    }

    void DrawBezier(Color color)
    {
        Gizmos.color = color;

        for (int i = 0; i < NUM_BEZIER_SEGMENTS; i++)
        {
            Gizmos.DrawLine(_bezierPoints[i], _bezierPoints[i + 1]);
        }
    }
}
