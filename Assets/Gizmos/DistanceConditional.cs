using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class DistanceConditional : Conditional
{
	public SharedFloat distance;
	public SharedTransform target;
	
	public override TaskStatus OnUpdate()
	{
		return Vector3.Distance(transform.position, target.Value.position) <= distance.Value
			? TaskStatus.Success
			: TaskStatus.Failure;
	}
}