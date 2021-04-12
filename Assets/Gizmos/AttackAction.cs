using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AttackAction : Action
{
	public SharedBool enable;
	
	public override void OnStart()
	{
		GetComponent<DummyInput>().attackRight = enable.Value;
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}