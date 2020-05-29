using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public Transform model;
    public Animator anim;
    public PlayerInput playerInput;
    public Rigidbody rigid;

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
    private Vector3 thrustVector;
    private bool canAttack = true;
    private float lerpTarget;
    private Vector3 deltaPosition;

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward",
            playerInput.directionMagnitude * Mathf.Lerp(anim.GetFloat("forward"), playerInput.run ? 2.0f : 1.0f, 0.3f));

        anim.SetBool("defense", playerInput.defense);
        
        if (rigid.velocity.magnitude > 5.0f)
        {
            anim.SetTrigger("roll");
        }

        if (playerInput.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if (playerInput.attack && CheckState("ground") && canAttack)
        {
            anim.SetTrigger("attack");
        }

        if (playerInput.directionMagnitude > 0.1f)
        {
            model.forward = Vector3.Slerp(model.forward, playerInput.directionVector, 0.3f);
        }

        if (!planarLock)
        {
            planarVector = playerInput.directionMagnitude * model.forward * walkSpeed *
                           (playerInput.run ? runSpeed : 1.0f);
        }

    }

    private void FixedUpdate()
    {
        rigid.position += deltaPosition;
        rigid.velocity = new Vector3(planarVector.x, rigid.velocity.y, planarVector.z) + thrustVector;
        thrustVector = Vector3.zero;
        deltaPosition = Vector3.zero;
    }

    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    public void OnJumpEnter()
    {
        playerInput.inputEnabled = false;
        planarLock = true;
        thrustVector = new Vector3(0, jumpVelocity, 0);
    }

    public void OnGroundEnter()
    {
        playerInput.inputEnabled = true;
        planarLock = false;
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
        lerpTarget = 1.0f;
    }

    public void OnAttack1hAUpdate()
    {
        thrustVector = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        float currentWeight = Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), currentWeight);
    }
    
    public void OnAttackIdleEnter()
    {
        playerInput.inputEnabled = true;
        lerpTarget = 0;
    }

    public void OnAttackIdleUpdate()
    {
        thrustVector = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        float currentWeight = Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), currentWeight);
    }

    public void OnUpdateRootMotion(object deltaPosition)
    {
        if (CheckState("attack1hC", "Attack"))
        {
            this.deltaPosition += (Vector3) deltaPosition;
        }
    }
}
