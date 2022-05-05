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
            Handles.Label(points[i], $"{i} ({points[i].x:F2}, {points[i].y:F2}, {points[i].z:F2})");
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
            Vector3 ppos, pforward, pright, pup;

            ppos = pforward = pright = pup = Vector3.zero;

            for (int i = 0; i <= BezierCurve.NUM_BEZIER_SEGMENTS; i++)
            {
                var t = (float)i / BezierCurve.NUM_BEZIER_SEGMENTS;

                var pos = BezierUtil.BezierPoint(t, points);
                var forward = BezierUtil.BezierTangent(t, points);
                Vector3 right = Vector3.one;
                Vector3 up = Vector3.one;

                if (i == 0)
                {
                    var nforward = forward + BezierUtil.BezierCurvature(0, points);

                    right = Vector3.Cross(forward, nforward);
                    up = Vector3.Cross(forward, right);
                }
                else
                {
                    var v1 = pos - ppos;
                    var pforward2 = reflect(pforward, v1);
                    var pright2 = reflect(pright, v1);

                    var v2 = forward - pforward2;

                    right = reflect(pright2, v2);
                    up = Vector3.Cross(forward, right);
                }

                var len = _curve._lenAxis * HandleUtility.GetHandleSize(pos);

                DrawAxis(pos, up, right, forward, len, _curve._width);

                ppos = pos;
                pforward = forward;
                pright = right;
                pup = up;
            }
        }

        if (GUI.changed)
        {
            _curve.UpdateBezierPoints();
            EditorUtility.SetDirty(target);
        }
    }

    Vector3 reflect(Vector3 dir, Vector3 normal)
    {
        var c = Vector3.Dot(normal, normal);

        return dir - normal * 2 / c * Vector3.Dot(normal, dir);
    }

    void DrawAxis(Vector3 pos, Vector3 up, Vector3 right, Vector3 forward, float len, float width)
    {
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(width, 2, pos, pos + forward * len);

        Handles.color = Color.red;
        Handles.DrawAAPolyLine(width, 2, pos, pos + right * len);

        Handles.color = Color.green;
        Handles.DrawAAPolyLine(width, 2, pos, pos + up * len);
    }
}
