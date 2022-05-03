using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public float _handleSize = 1;
    public float _lineSize = 5;
    public Vector3[] _points = new Vector3[]
    {
        Vector3.zero,
        new Vector3(0, 1, 0),
        new Vector3(2, -1, 0),
        new Vector3(2, 0, 0),
    };

    private const int NUM_BEZIER_SEGMENTS = 30;
    private Vector3[] _bezierPoints = new Vector3[NUM_BEZIER_SEGMENTS + 1];

    void Start()
    {
        UpdateBezierPoints();
    }

    void OnDrawGizmos()
    {
        DrawBezier(Color.green);
    }

    public void UpdateBezierPoints()
    {
        for (int i = 0; i <= NUM_BEZIER_SEGMENTS; i++)
        {
            _bezierPoints[i] = BezierPoint((float)i / NUM_BEZIER_SEGMENTS, _points);
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

    Vector3 BezierPoint(float t, params Vector3[] cp)
    {
        return new Vector3(Bezier(t, cp[0].x, cp[1].x, cp[2].x, cp[3].x),
                           Bezier(t, cp[0].y, cp[1].y, cp[2].y, cp[3].y),
                           Bezier(t, cp[0].z, cp[1].z, cp[2].z, cp[3].z));
    }

    float Bezier(float t, params float[] w)
    {
        var mt = 1 - t;
        var mt2 = mt * mt;
        var mt3 = mt * mt * mt;
        var t2 = t * t;
        var t3 = t * t * t;

        return w[0] * mt3 + w[1] * 3 * mt2 * t + w[2] * 3 * mt * t2 + w[3] * t3;
    }
}
