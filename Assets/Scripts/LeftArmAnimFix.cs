using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    private Animator anim;
    public ActorController actorController;
    public Vector3 leftLowerArmEuler;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (actorController.leftIsShield)
        {
            if (!anim.GetBool("defence"))
            {
                Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                leftLowerArm.localEulerAngles += leftLowerArmEuler;
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
            }
        }
    }
}
