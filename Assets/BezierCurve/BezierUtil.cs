using UnityEngine;

public class BezierUtil
{
    static public Vector3 BezierCurvature(float t, params Vector3[] cp)
    {
        return new Vector3(BezierDerivative2(t, cp[0].x, cp[1].x, cp[2].x, cp[3].x),
                           BezierDerivative2(t, cp[0].y, cp[1].y, cp[2].y, cp[3].y),
                           BezierDerivative2(t, cp[0].z, cp[1].z, cp[2].z, cp[3].z)).normalized;
    }

    static public Vector3 BezierTangent(float t, params Vector3[] cp)
    {
        return new Vector3(BezierDerivative(t, cp[0].x, cp[1].x, cp[2].x, cp[3].x),
                           BezierDerivative(t, cp[0].y, cp[1].y, cp[2].y, cp[3].y),
                           BezierDerivative(t, cp[0].z, cp[1].z, cp[2].z, cp[3].z)).normalized;
    }

    static public Vector3 BezierPoint(float t, params Vector3[] cp)
    {
        return new Vector3(Bezier(t, cp[0].x, cp[1].x, cp[2].x, cp[3].x),
                           Bezier(t, cp[0].y, cp[1].y, cp[2].y, cp[3].y),
                           Bezier(t, cp[0].z, cp[1].z, cp[2].z, cp[3].z));
    }

    static public float BezierDerivative2(float t, params float[] w)
    {
        var mt = 1 - t;

        var w0 = 3 * (w[1] - w[0]);
        var w1 = 3 * (w[2] - w[1]);
        var w2 = 3 * (w[3] - w[2]);

        var w20 = 2 * (w1 - w0);
        var w21 = 2 * (w2 - w1);

        return w20 * mt + w21 * t;
    }

    static public float BezierDerivative(float t, params float[] w)
    {
        var mt = 1 - t;
        var mt2 = mt * mt;
        var t2 = t * t;

        var w0 = 3 * (w[1] - w[0]);
        var w1 = 3 * (w[2] - w[1]);
        var w2 = 3 * (w[3] - w[2]);

        return w0 * mt2 + w1 * 2 * mt * t + w2 * t2;
    }

    static public float Bezier(float t, params float[] w)
    {
        var mt = 1 - t;
        var mt2 = mt * mt;
        var mt3 = mt * mt * mt;
        var t2 = t * t;
        var t3 = t * t * t;

        return w[0] * mt3 + w[1] * 3 * mt2 * t + w[2] * 3 * mt * t2 + w[3] * t3;
    }
}
