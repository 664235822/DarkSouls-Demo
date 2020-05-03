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

    public float walkSpeed = 1.5f;
    public float runSpeed = 3.0f;
    public float jumpVelocity = 4.0f;
    public float rollVelocity = 3.0f;

    private Vector3 planarVector;
    private bool planarLock;

    private Vector3 thrustVector;

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward",
            playerInput.directionMagnitude * Mathf.Lerp(anim.GetFloat("forward"), playerInput.run ? 2.0f : 1.0f, 0.3f));
        if (rigid.velocity.magnitude > 5.0f)
        {
            anim.SetTrigger("roll");
        }
        if (playerInput.jump)
        {
            anim.SetTrigger("jump");    
        }
        
        if (playerInput.directionMagnitude > 0.1f)
        {
            model.forward = Vector3.Slerp(model.forward, playerInput.directionVector, 0.3f);
        }

        if (!planarLock)
        {
            planarVector = playerInput.directionMagnitude * model.forward * walkSpeed * (playerInput.run ? runSpeed : 1.0f);
        }
        
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector3(planarVector.x, rigid.velocity.y, planarVector.z) + thrustVector;
        thrustVector = Vector3.zero;
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
}
