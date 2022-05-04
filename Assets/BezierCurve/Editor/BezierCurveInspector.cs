using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve _curve;

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

    private Vector3[] points => _curve._points;

    private const int NUM_BEZIER_SEGMENTS = 30;
    private Vector3[] _bezierPoints = new Vector3[NUM_BEZIER_SEGMENTS + 1];
    private int _idSelected = -1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    bool isPointID(int id) => id >= 1000 && id < 1000 + points.Length;
    int idx2id(int idx) => 1000 + idx;
    int id2idx(int id) => isPointID(id) ? id - 1000 : -1;

    bool isNearAnyPoints => isPointID(HandleUtility.nearestControl);
    bool isSelectAnyPoints => isPointID(_idSelected);

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

        if (GUI.changed)
        {
            _curve.UpdateBezierPoints();
            EditorUtility.SetDirty(target);
        }
    }
}
