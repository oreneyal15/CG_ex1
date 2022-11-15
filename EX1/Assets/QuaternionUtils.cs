using System;
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
        return Vector4.zero;
    }

    // Returns the Hamilton product of given quaternions q and v
    public static Vector4 HamiltonProduct(Vector4 q, Vector4 v)
    {
        return Vector4.zero;
    }

    // Returns a quaternion representing a rotation of theta degrees around the given axis
    public static Vector4 AxisAngle(Vector3 axis, float theta)
    {
        return Vector4.zero;
    }

    // Returns a quaternion representing the given Euler angles applied in the given rotation order
    public static Vector4 FromEuler(Vector3 euler, Vector3Int rotationOrder)
    {
        return Vector4.zero;
    }

    // Returns a spherically interpolated quaternion between q1 and q2 at time t in [0,1]
    public static Vector4 Slerp(Vector4 q1, Vector4 q2, float t)
    {
        return Vector4.zero;
    }
}