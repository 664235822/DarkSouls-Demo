using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float ZOffset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Vector3 targetpos = new Vector3 (target.position.x, transform.position.y, target.position.z + ZOffset);

		transform.position = Vector3.Lerp (transform.position, targetpos, 0.1F);
	}
}
