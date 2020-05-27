using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float lifeTime = 3.0F;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void OnCollisionEnter () {
		Destroy (gameObject);
	}
}
