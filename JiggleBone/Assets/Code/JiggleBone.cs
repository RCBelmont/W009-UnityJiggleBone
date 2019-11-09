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

            Vector3 moveDir = bp.TargetPos - bp.CurrPos;
            float totalDistance = moveDir.magnitude;

            Vector3 dynamicPos = bp.CurrPos + moveDir.normalized * (totalDistance / (Stiff * bp.BoneIdx));
            Vector3 parentToThisDir = (dynamicPos - bp.ParentBone.CurrPos).normalized;
            bp.CurrPos = bp.ParentBone.CurrPos + parentToThisDir * bp.DefaultDisToParent;
            
           
            bp.BoneTransform.position = bp.CurrPos; //bp.CurrPos;
            bp.BoneTransform.LookAt(bp.ParentBone.CurrPos);
        }
    }

    private void SetBoneToDefaultPos()
    {
        for (int i = 0; i < _boneList.Count; i++)
        {
            BoneParticle bp = _boneList[i];
            if (bp.IsRoot)
            {
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
}