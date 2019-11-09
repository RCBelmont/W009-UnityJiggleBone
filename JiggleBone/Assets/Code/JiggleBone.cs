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
    public BoneParticle(GameObject obj, bool isRoot, BoneParticle parentBone = null)
    {
        GO = obj;
        IsRoot = isRoot;
        BoneTransform = obj.transform;
        CurrPos = TargetPos = BoneTransform.position;
    }
}


public class JiggleBone : MonoBehaviour
{
    private BoneParticle _rootBone;
    private List<BoneParticle> _boneList = new List<BoneParticle>();
    private void Awake()
    {
       _rootBone = new BoneParticle(this.gameObject, true);
       _boneList.Add(_rootBone);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void CrateChildBoneParticle(BoneParticle root)
    {
        int childCount = root.BoneTransform.GetChildCount();
        if (childCount > 0)
        {
            Transform child = root.BoneTransform.GetChild(0);
            _boneList.Add(new BoneParticle(child.gameObject, false, root));
        }
    }

}

