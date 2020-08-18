using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public Transform model;
    public Animator anim;
    public PlayerInputInterface playerInput;
    public Rigidbody rigid;
    public CameraController cameraController;

    [Header("Movement Setting")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 3.0f;
    public float jumpVelocity = 4.0f;
    public float rollVelocity = 3.0f;

    [Header("Friction Setting")] 
    public CapsuleCollider collider;
    public PhysicMaterial FrictionOne;
    public PhysicMaterial FrictionZero;
    
    private Vector3 planarVector;
    private bool planarLock;
    private bool directionTrack;
    private Vector3 thrustVector;
    private bool canAttack = true;
    private Vector3 deltaPosition;

    public bool leftIsShield = true;

    // Update is called once per frame
    void Update()
    {

        if (!cameraController.lockState)
        {
            anim.SetFloat("forward",
                playerInput.directionMagnitude * Mathf.Lerp(anim.GetFloat("forward"), playerInput.run ? 2.0f : 1.0f, 0.3f));
            anim.SetFloat("right",0);
        }
        else
        {
            Vector3 localDVec = transform.InverseTransformVector(playerInput.directionVector);
            anim.SetFloat("forward", localDVec.z * (playerInput.run ? 2.0f : 1.0f));
            anim.SetFloat("right", localDVec.x * (playerInput.run ? 2.0f : 1.0f));
        }

        if (rigid.velocity.magnitude > 5.0f)
        {
            anim.SetTrigger("roll");
        }

        if (playerInput.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if ((playerInput.attackRight || playerInput.attackLeft) &&
            (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)
        {
            anim.SetBool("mirror", playerInput.attackLeft && !leftIsShield);
            anim.SetTrigger("attack");
        }

        if ((playerInput.counterBack) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) &&
            canAttack)
        {
            if (!leftIsShield)
            {
                anim.SetTrigger("counterBack");
            }
        }

        if ((CheckState("ground") || CheckState("blocked")) && leftIsShield)
        {
            if (playerInput.defence)
            {
                anim.SetBool("defence", playerInput.defence);
                anim.SetLayerWeight(anim.GetLayerIndex("Defence"), 1);
            }
            else
            {
                anim.SetBool("defence", playerInput.defence);
                anim.SetLayerWeight(anim.GetLayerIndex("Defence"), 0);
            }
        }
        else
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Defence"), 0);
        }

        if (playerInput.directionMagnitude > 0.1f)
        {
            model.forward = Vector3.Slerp(model.forward, playerInput.directionVector, 0.3f);
        }

        if (playerInput.lockTarget)
        {
            cameraController.LockTarget();
        }

        if (!cameraController.lockState)
        {
            if (!planarLock)
            {
                planarVector = playerInput.directionMagnitude * model.forward * walkSpeed *
                               (playerInput.run ? runSpeed : 1.0f);
            }
        }
        else
        {
            if (!directionTrack)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planarVector.normalized;
            }
            
            if (!planarLock)
            {
                planarVector = playerInput.directionVector * walkSpeed *
                               (playerInput.run ? runSpeed : 1.0f);
            }
        }


    }

    private void FixedUpdate()
    {
        rigid.position += deltaPosition;
        rigid.velocity = new Vector3(planarVector.x, rigid.velocity.y, planarVector.z) + thrustVector;
        thrustVector = Vector3.zero;
        deltaPosition = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }
    
    public bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsTag(tagName);
    }

    public void OnJumpEnter()
    {
        playerInput.inputEnabled = false;
        planarLock = true;
        directionTrack = true;
        thrustVector = new Vector3(0, jumpVelocity, 0);
    }

    public void OnGroundEnter()
    {
        playerInput.inputEnabled = true;
        planarLock = false;
        directionTrack = false;
        canAttack = true;
        
        collider.material = FrictionOne;
    }

    public void OnGroundExit()
    {
        collider.material = FrictionZero;
    }

    public void OnFailEnter()
    {
        playerInput.inputEnabled = false;
        planarLock = true;
    }

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }

    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnRollEnter()
    {
        playerInput.inputEnabled = false;
        planarLock = true;
        directionTrack = true;
        thrustVector = new Vector3(0, rollVelocity, 0);
    }

    public void OnJabUpdate()
    {
        playerInput.inputEnabled = false;
        planarLock = true;
        thrustVector = model.transform.forward * anim.GetFloat("jabVelocity");
    }

    public void OnAttack1hAEnter()
    {
        playerInput.inputEnabled = false;
    }

    public void OnAttack1hAUpdate()
    {
        thrustVector = model.transform.forward * anim.GetFloat("attack1hAVelocity");
    }

    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnUpdateRootMotion(object deltaPosition)
    {
        if (CheckState("attack1hC"))
        {
            this.deltaPosition += (Vector3) deltaPosition;
        }
    }

    public void OnHitEnter()
    {
        playerInput.inputEnabled = false;
        planarVector = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnBlockedEnter()
    {
        playerInput.inputEnabled = false;
    }

    public void OnDieEnter()
    {
        playerInput.inputEnabled = false;
        planarVector = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnStunnedEnter()
    {
        playerInput.inputEnabled = false;
        planarVector = Vector3.zero;
    }

    public void OnCounterBackEnter()
    {
        playerInput.inputEnabled = false;
        planarVector = Vector3.zero;
    }
    
    public void IssueTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void SetBool(string animName, bool value)
    {
        anim.SetBool(animName, value);
    }
}
