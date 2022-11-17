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

    private const String HEAD_NAME = "Head";
    private const int HEAD_SIZE = 8;
    private const int JOINT_SIZE = 2;
    private const float BONE_DIAMETER = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        BVHParser parser = new BVHParser();
        data = parser.Parse(BVHFile);
        CreateJoint(data.rootJoint, Vector3.zero);
    }

    // Returns a Matrix4x4 representing a rotation aligning the up direction of an object with the given v
    public Matrix4x4 RotateTowardsVector(Vector3 v)
    {
        // Your code here
        v = v.normalized;
        float teta_x = 90 - Mathf.Atan2(v.y, v.z) * Mathf.Rad2Deg;
        Matrix4x4 R_x = MatrixUtils.RotateX(-teta_x);
        float teta_z = 90 - Mathf.Atan2(Mathf.Sqrt(v.y* v.y + v.z * v.z), v.x) * Mathf.Rad2Deg;
        Matrix4x4 R_z = MatrixUtils.RotateZ(teta_z);
        return R_x.inverse * R_z.inverse;
    }

    // Creates a Cylinder GameObject between two given points in 3D space
    public GameObject CreateCylinderBetweenPoints(Vector3 p1, Vector3 p2, float diameter)
    {
        // Your code here
        GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Matrix4x4 t = MatrixUtils.Translate((p1 + p2) / 2);
        Matrix4x4 r = RotateTowardsVector(p1 - p2);
        Matrix4x4 s =  MatrixUtils.Scale(new Vector3(diameter, Vector3.Distance(p1, p2) / 2, diameter));
        MatrixUtils.ApplyTransform(bone, t * r * s);
        return bone;
    }

    // Creates a GameObject representing a given BVHJoint and recursively creates GameObjects for it's child joints
    public GameObject CreateJoint(BVHJoint joint, Vector3 parentPosition)
    {
        // Your code here
        joint.gameObject = new GameObject(joint.name);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = joint.gameObject.transform;
        int scaleSphere = JOINT_SIZE;
        if (joint.name == HEAD_NAME)
        {
            scaleSphere = HEAD_SIZE;
        }
        MatrixUtils.ApplyTransform(sphere, MatrixUtils.Scale(Vector3.one * scaleSphere));
        Vector3 jointPosition = parentPosition + joint.offset;
        MatrixUtils.ApplyTransform(joint.gameObject, MatrixUtils.Translate(jointPosition));
        if (!joint.isEndSite)
        {
            foreach (BVHJoint child in joint.children)
            {
                Vector3 childPosition = jointPosition + child.offset;
                
                GameObject bone = CreateCylinderBetweenPoints(jointPosition, childPosition,
                    BONE_DIAMETER);
                CreateJoint(child, jointPosition);
                bone.transform.parent = joint.gameObject.transform;
            }
        }
        return joint.gameObject;
    }

    // Transforms BVHJoint according to the keyframe channel data, and recursively transforms its children
    public void TransformJoint(BVHJoint joint, Matrix4x4 parentTransform)
    {
        // Your code here
        Matrix4x4 S = Matrix4x4.identity;
        Matrix4x4 R = Matrix4x4.identity;
        if (!joint.isEndSite)
        {
            Vector3 curAngel = new Vector3(currFrameData[joint.rotationChannels.x],
                currFrameData[joint.rotationChannels.y],
                currFrameData[joint.rotationChannels.z]);
            Vector3 nextAngel = new Vector3(nextFrameData[joint.rotationChannels.x],
                nextFrameData[joint.rotationChannels.y],
                nextFrameData[joint.rotationChannels.z]);
            var q1 = QuaternionUtils.FromEuler(curAngel, joint.rotationOrder);
            var q2 = QuaternionUtils.FromEuler(nextAngel, joint.rotationOrder);
            var q = interpolate ? QuaternionUtils.Slerp(q1, q2, t) : q1;
            R = MatrixUtils.RotateFromQuaternion(q);
        }
        //Dictionary<int, Matrix4x4> rotation_matricies = new Dictionary<int, Matrix4x4>();
        //rotation_matricies.Add(0, MatrixUtils.RotateX(currFrameData[joint.rotationChannels.x]));
        //rotation_matricies.Add(1, MatrixUtils.RotateY(currFrameData[joint.rotationChannels.y]));
        //rotation_matricies.Add(2, MatrixUtils.RotateZ(currFrameData[joint.rotationChannels.z]));
        //Matrix4x4 R = rotation_matricies[joint.rotationOrder.x] * rotation_matricies[joint.rotationOrder.y] * rotation_matricies[joint.rotationOrder.z];
        Matrix4x4 T = MatrixUtils.Translate(joint.offset);
        Matrix4x4 m_tag = parentTransform * T * R * S;
        MatrixUtils.ApplyTransform(joint.gameObject, m_tag);
        if (!joint.isEndSite)
        {
            foreach (BVHJoint child in joint.children)
            {
                TransformJoint(child, m_tag); //todo: check if it what we needed to do
            }
        }
    }

    // Returns the frame nunmber of the BVH animation at a given time
    public int GetFrameNumber(float time)
    {
        // Your code here
        int numOfFrames = (int) (time / data.frameLength);
        return numOfFrames % data.numFrames;
    }

    // Returns the proportion of time elapsed between the last frame and the next one, between 0 and 1
    public float GetFrameIntervalTime(float time)
    {
        // Your code here
        var numOfFrames = (int) (time / data.frameLength);
        return (time - numOfFrames * data.frameLength) / data.frameLength;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time * animationSpeed;
        if (animate)
        {
            int currFrame = GetFrameNumber(time);
            // Your code here
            
            t = interpolate ? GetFrameIntervalTime(time) : 0;
            currFrameData = data.keyframes[currFrame];
            if (currFrame != data.numFrames - 1)
            {
                nextFrameData = data.keyframes[currFrame + 1];
            }

            var curHipPos = new Vector3(currFrameData[data.rootJoint.positionChannels.x],
                currFrameData[data.rootJoint.positionChannels.y], currFrameData[data.rootJoint.positionChannels.z]);
            var nextHipPos = new Vector3(nextFrameData[data.rootJoint.positionChannels.x],
                nextFrameData[data.rootJoint.positionChannels.y], nextFrameData[data.rootJoint.positionChannels.z]);
            Vector3 hipPos = Vector3.Lerp(curHipPos, nextHipPos, t);
            TransformJoint(data.rootJoint, MatrixUtils.Translate(hipPos));
            
        }
    }
}
