using System;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionUtils
{
    // The default rotation order of Unity. May be used for testing
    public static readonly Vector3Int UNITY_ROTATION_ORDER = new Vector3Int(1,2,0);

    // Returns the product of 2 given quaternions
    public static Vector4 Multiply(Vector4 q1, Vector4 q2)
    {
        return new Vector4(
            q1.w*q2.x + q1.x*q2.w + q1.y*q2.z - q1.z*q2.y,
            q1.w*q2.y + q1.y*q2.w + q1.z*q2.x - q1.x*q2.z,
            q1.w*q2.z + q1.z*q2.w + q1.x*q2.y - q1.y*q2.x,
            q1.w*q2.w - q1.x*q2.x - q1.y*q2.y - q1.z*q2.z
        );
    }

    // Returns the conjugate of the given quaternion q
    public static Vector4 Conjugate(Vector4 q)
    {
        return new Vector4(-q.x, -q.y, -q.z, q.w);
    }

    // Returns the Hamilton product of given quaternions q and v
    public static Vector4 HamiltonProduct(Vector4 q, Vector4 v)
    {
        return Multiply(q, Multiply(v, Conjugate(q)));
    }

    // Returns a quaternion representing a rotation of theta degrees around the given axis
    public static Vector4 AxisAngle(Vector3 axis, float theta)
    {
        axis = axis.normalized;
        theta = theta * Mathf.Deg2Rad;
        var sinTheta = Mathf.Sin(theta / 2) * Mathf.Rad2Deg;
        Vector4 q = new Vector4(axis.x * sinTheta, axis.y * sinTheta, axis.z * sinTheta,
            Mathf.Cos(theta / 2) * Mathf.Rad2Deg);
        return q.normalized;
    }

    // Returns a quaternion representing the given Euler angles applied in the given rotation order
    public static Vector4 FromEuler(Vector3 euler, Vector3Int rotationOrder)
    {
        Dictionary<int, Vector4> rotation_matricies = new Dictionary<int, Vector4>();
        rotation_matricies.Add(rotationOrder.x, AxisAngle(Vector3.right, euler.x));
        rotation_matricies.Add(rotationOrder.y, AxisAngle(Vector3.up, euler.y));
        rotation_matricies.Add(rotationOrder.z, AxisAngle(Vector3.forward, euler.z));
        Vector4 q = Multiply(rotation_matricies[0],
            Multiply(rotation_matricies[1], rotation_matricies[2]));
        return q.normalized;
    }

    // Returns a spherically interpolated quaternion between q1 and q2 at time t in [0,1]
    public static Vector4 Slerp(Vector4 q1, Vector4 q2, float t)
    {
        q1 = q1.normalized;
        q2 = q2.normalized;
        var w = Multiply(q1, Conjugate(q2)).w;
        w = Mathf.Clamp(w, -1f, 1f);
        var theta = 2 * Mathf.Acos(w);
        if (Mathf.Sin(theta) == 0)
        {
            return q1;
        }
        var coefQ1 = Mathf.Sin((1 - t) * theta) / Mathf.Sin(theta);
        var coefQ2 = Mathf.Sin(t * theta) / Mathf.Sin(theta);
        var q = coefQ1 * q1 + coefQ2 * q2;
        return q.normalized;
    }
}