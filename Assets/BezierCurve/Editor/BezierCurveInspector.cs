using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve _curve;

    private int _idSelected = -1;

    Vector3[] points => _curve._points;
    bool isPointID(int id) => id >= 1000 && id < 1000 + points.Length;
    int idx2id(int idx) => 1000 + idx;
    int id2idx(int id) => isPointID(id) ? id - 1000 : -1;

    bool isNearAnyPoints => isPointID(HandleUtility.nearestControl);
    bool isSelectAnyPoints => isPointID(_idSelected);

    void OnEnable()
    {
        Tools.hidden = true;

        _curve = target as BezierCurve;
        _curve.UpdateBezierPoints();
    }

    void OnDisable()
    {
        Tools.hidden = false;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    void OnSceneGUI()
    {
        var evt = Event.current;

        for (int i = 0; i < points.Length; i++)
        {
            int id = idx2id(i);
            var size = HandleUtility.GetHandleSize(points[i]) * _curve._handleSize;

            Handles.color = id == _idSelected ? Color.green : Color.blue;
            Handles.SphereHandleCap(id, points[i], Quaternion.identity, size, evt.type);

            var guiColor = GUI.color;
            GUI.color = Color.black;
            Handles.Label(points[i], $"({points[i].x:F2}, {points[i].y:F2}, {points[i].z:F2})");
            GUI.color = guiColor;
        }

        if (isSelectAnyPoints)
        {
            var idx = id2idx(_idSelected);

            Handles.color = Color.green;
            points[idx] = Handles.PositionHandle(points[idx], Quaternion.identity);
        }

        if (evt.modifiers == EventModifiers.None && evt.type == EventType.MouseDown)
        {
            if (isNearAnyPoints)
            {
                _idSelected = HandleUtility.nearestControl;
                evt.Use();
            }
        }

        if (_curve._isShowAxis)
        {
            for (int i = 0; i <= BezierCurve.NUM_BEZIER_SEGMENTS; i++)
            {
                var t = (float)i / BezierCurve.NUM_BEZIER_SEGMENTS;

                var pos = BezierCurve.BezierPoint(t, points);
                var forward = BezierTangent(t, points);
                var right = Vector3.Cross(Vector3.up, forward).normalized;
                var up = Vector3.Cross(forward, right).normalized;

                var len = _curve._lenAxis * HandleUtility.GetHandleSize(pos);

                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(_curve._width, 2, pos, pos + forward * len);

                Handles.color = Color.red;
                Handles.DrawAAPolyLine(_curve._width, 2, pos, pos + right * len);

                Handles.color = Color.green;
                Handles.DrawAAPolyLine(_curve._width, 2, pos, pos + up * len);
            }
        }

        if (GUI.changed)
        {
            _curve.UpdateBezierPoints();
            EditorUtility.SetDirty(target);
        }
    }

    Vector3 BezierTangent(float t, params Vector3[] cp)
    {
        return new Vector3(BezierDerivative(t, cp[0].x, cp[1].x, cp[2].x, cp[3].x),
                           BezierDerivative(t, cp[0].y, cp[1].y, cp[2].y, cp[3].y),
                           BezierDerivative(t, cp[0].z, cp[1].z, cp[2].z, cp[3].z)).normalized;
    }

    float BezierDerivative(float t, params float[] w)
    {
        var mt = 1 - t;
        var mt2 = mt * mt;
        var t2 = t * t;

        var w0 = 3 * (w[1] - w[0]);
        var w1 = 3 * (w[2] - w[1]);
        var w2 = 3 * (w[3] - w[2]);

        return w0 * mt2 + w1 * 2 * mt * t + w2 * t2;
    }
}
