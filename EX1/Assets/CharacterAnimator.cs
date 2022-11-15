using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public TextAsset BVHFile; // The BVH file that defines the animation and skeleton
    public bool animate; // Indicates whether or not the animation should be running
    public bool interpolate; // Indicates whether or not frames should be interpolated
    [Range(0.01f, 2f)] public float animationSpeed = 1; // Controls the speed of the animation playback

    public BVHData data; // BVH data of the BVHFile will be loaded here
    public float t = 0; // Value used to interpolate the animation between frames
    public float[] currFrameData; // BVH channel data corresponding to the current keyframe
    public float[] nextFrameData; // BVH vhannel data corresponding to the next keyframe

    // Start is called before the first frame update
    void Start()
    {
        BVHParser parser = new BVHParser();
        data = parser.Parse(BVHFile);
    }

    // Returns a Matrix4x4 representing a rotation aligning the up direction of an object with the given v
    public Matrix4x4 RotateTowardsVector(Vector3 v)
    {
        // Your code here
        return Matrix4x4.zero;
    }

    // Creates a Cylinder GameObject between two given points in 3D space
    public GameObject CreateCylinderBetweenPoints(Vector3 p1, Vector3 p2, float diameter)
    {
        // Your code here
        return null;
    }

    // Creates a GameObject representing a given BVHJoint and recursively creates GameObjects for it's child joints
    public GameObject CreateJoint(BVHJoint joint, Vector3 parentPosition)
    {
        // Your code here
        return null;
    }

    // Transforms BVHJoint according to the keyframe channel data, and recursively transforms its children
    public void TransformJoint(BVHJoint joint, Matrix4x4 parentTransform)
    {
        // Your code here
    }

    // Returns the frame nunmber of the BVH animation at a given time
    public int GetFrameNumber(float time)
    {
        // Your code here
        return 0;
    }

    // Returns the proportion of time elapsed between the last frame and the next one, between 0 and 1
    public float GetFrameIntervalTime(float time)
    {
        // Your code here
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time * animationSpeed;
        if (animate)
        {
            int currFrame = GetFrameNumber(time);
            // Your code here
        }
    }
}
