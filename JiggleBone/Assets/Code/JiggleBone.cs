using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class BoneParticle
{
    public GameObject GO;
    public bool IsRoot = false;
    public Transform BoneTransform;
    public Vector3 CurrPos, TargetPos;
    public BoneParticle ParentBone;
    public Vector3 DefaultLocalPos;
    public int BoneIdx = 0;
    public float DefaultDisToParent = 0.0f;
    public Quaternion DefaultRotate;
    public Vector3 Velocity = Vector3.zero;
    public BoneParticle(GameObject obj, bool isRoot, int boneIdx = 0, BoneParticle parentBone = null)
    {
        GO = obj;
        IsRoot = isRoot;
        BoneTransform = obj.transform;
        CurrPos = TargetPos = BoneTransform.position;
        DefaultLocalPos = BoneTransform.localPosition;
        BoneIdx = boneIdx;
        ParentBone = parentBone;
        
        if (parentBone != null)
        {
            DefaultDisToParent = Vector3.Distance(BoneTransform.position, parentBone.BoneTransform.position);
        }

        DefaultRotate = BoneTransform.localRotation;

    }
}


public class JiggleBone : MonoBehaviour
{
    public float Stiff = 2.0f;

    private BoneParticle _rootBone;
    private List<BoneParticle> _boneList = new List<BoneParticle>();

    private void Awake()
    {
        _rootBone = new BoneParticle(this.gameObject, true, 0);
        _boneList.Add(_rootBone);
        CrateChildBoneParticle(_rootBone);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        SetBoneToDefaultPos();
        for (int i = 0; i < _boneList.Count; i++)
        {
            BoneParticle bp = _boneList[i];
            if (bp.IsRoot)
            {
                bp.CurrPos = bp.BoneTransform.position;
                continue;
            }

            Vector3 X = bp.BoneTransform.right;
            Vector3 forceDir = bp.TargetPos - bp.CurrPos;
            float distanceToOPos = forceDir.magnitude;
            
            forceDir = forceDir.normalized;
            float sprint = 50.0f;
            float forceStr = sprint / bp.BoneIdx * distanceToOPos; 
            float mass = 3.0f;
            
            Vector3 parentToThisDir = (bp.CurrPos - bp.ParentBone.CurrPos).normalized;
            //parentToThisDir = (bp.CurrPos - bp.ParentBone.CurrPos).normalized;
            //bp.CurrPos = bp.ParentBone.CurrPos + parentToThisDir * bp.DefaultDisToParent;
            forceDir = Vector3.Cross(parentToThisDir, forceDir).normalized;
            forceDir = Vector3.Cross(forceDir, parentToThisDir).normalized;
            Vector3 accelerate = (forceStr / mass) * forceDir;

            bp.Velocity += accelerate * Time.deltaTime;
            bp.Velocity = Vector3.Dot(bp.Velocity, forceDir) * 0.99f * forceDir;
            
            Vector3 dynamicPos = bp.CurrPos + bp.Velocity * Time.deltaTime;
            parentToThisDir = (dynamicPos - bp.ParentBone.CurrPos).normalized;
            bp.CurrPos = bp.ParentBone.CurrPos + parentToThisDir * bp.DefaultDisToParent;
            
         
            bp.BoneTransform.position = bp.CurrPos; //bp.CurrPos;
            //bp.BoneTransform.LookAt(bp.ParentBone.CurrPos, _rootBone.BoneTransform.forward);
            Vector3 lookDir = (bp.CurrPos - bp.ParentBone.CurrPos).normalized;
            //lookDir = Vector3.Cross(bp.BoneTransform.right, lookDir);
            
            bp.BoneTransform.rotation = QuaternionLookRotation(lookDir, X);
        }
    }

    private Quaternion QuaternionLookRotation(Vector3 forward, Vector3 up)
    {
        forward.Normalize();

        Vector3 vector = Vector3.Normalize(forward);
        Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
        Vector3 vector3 = Vector3.Cross(vector, vector2);
        var m00 = vector3.x;
        var m01 = vector3.y;
        var m02 = vector3.z;
        
        var m10 = vector.x;
        var m11 = vector.y;
        var m12 = vector.z;
        
        var m20 = vector2.x;
        var m21 = vector2.y;
        var m22 = vector2.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float) Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }

        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float) Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }

        if (m11 > m22)
        {
            var num6 = (float) Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }

        var num5 = (float) Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    private void SetBoneToDefaultPos()
    {
        for (int i = 0; i < _boneList.Count; i++)
        {
            BoneParticle bp = _boneList[i];
            if (bp.IsRoot)
            {
                bp.TargetPos = bp.BoneTransform.position;
                continue;
            }

            bp.BoneTransform.localRotation = bp.DefaultRotate;
            bp.BoneTransform.localPosition = bp.DefaultLocalPos;
            bp.TargetPos = bp.BoneTransform.position;
        }
    }

    private void CrateChildBoneParticle(BoneParticle root)
    {
        int childCount = root.BoneTransform.childCount;
        if (childCount > 0)
        {
          
            Transform child = root.BoneTransform.GetChild(0);
            BoneParticle newParticle = new BoneParticle(child.gameObject, false, _boneList.Count, root);
            _boneList.Add(newParticle);
            CrateChildBoneParticle(newParticle);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var boneParticle in _boneList)
        {
            Gizmos.DrawWireSphere(boneParticle.TargetPos,  1.5f);
            Gizmos.DrawLine(boneParticle.BoneTransform.position, boneParticle.BoneTransform.position +boneParticle.Velocity);
        }
    }
}