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

    private Vector3 movingVector;

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward",
            playerInput.directionMagnitude * Mathf.Lerp(anim.GetFloat("forward"), playerInput.run ? 2.0f : 1.0f, 0.3f));
        if (playerInput.directionMagnitude > 0.1f)
        {
            model.forward = Vector3.Slerp(model.forward, playerInput.directionVector, 0.3f);
        }

        movingVector = playerInput.directionMagnitude * model.forward * walkSpeed * (playerInput.run ? runSpeed : 1.0f);
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector3(movingVector.x, rigid.velocity.y, movingVector.z);
    }
}
