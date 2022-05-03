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

        if (evt.modifiers == EventModifiers.None && evt.type == EventType.MouseUp)
        {
            if (isNearAnyPoints)
            {
                _idSelected = HandleUtility.nearestControl;
                evt.Use();
            }
        }

        for (int i = 0; i < points.Length; i++)
        {
            int id = idx2id(i);
            bool isSelected = id == _idSelected;

            Handles.color = isSelected ? Color.green : Color.blue;

            var size = HandleUtility.GetHandleSize(points[i]) * _curve._handleSize;

            Handles.SphereHandleCap(id, points[i], Quaternion.identity, size, evt.type);
        }

        if (isSelectAnyPoints)
        {
            var idx = id2idx(_idSelected);

            Handles.color = Color.green;
            points[idx] = Handles.PositionHandle(points[idx], Quaternion.identity);
        }

        if (GUI.changed)
        {
            _curve.UpdateBezierPoints();
            EditorUtility.SetDirty(target);
        }
    }
}
