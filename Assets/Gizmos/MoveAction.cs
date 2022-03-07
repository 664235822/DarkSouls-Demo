using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3;
using UnityEngine;

public class MoveAction : Action
{
    public SharedFloat maxDistance;
    public SharedFloat minDistance;
    public SharedFloat speed;
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.Value.position);
        if (distance <= maxDistance.Value && distance > minDistance.Value)
        {
            transform.position += (target.Value.position - transform.position).normalized * speed.Value * Time.deltaTime;
            transform.LookAt(target.Value);
            GetComponent<ActorController>().anim.SetFloat("forward", 1);
            return TaskStatus.Running;
        }
        else
        {
            GetComponent<ActorController>().anim.SetFloat("forward", 0);
            return TaskStatus.Failure;
        }
    }
}