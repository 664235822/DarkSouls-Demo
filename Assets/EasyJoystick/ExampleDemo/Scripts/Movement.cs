








using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour 
{
	public float speed = 5;
	public EasyJoystick moveJoy, rotateJoy;
	public Transform Gun, bulletSpawn;
	public Rigidbody bulletPrefab;
	public float attck_rate;
	private float delay;

	void FixedUpdate (){
		GetComponent<Rigidbody>().velocity = moveJoy.MoveInput () * speed;			//Move rigidbody;
		moveJoy.Rotate (transform, 15.0F);							//Rotate rigidbody;
		rotateJoy.Rotate (Gun, 15.0F);								//Rotate gun;

		if(rotateJoy.IsPressed())									//Shooting;
			Shoot();
	}

	void Shoot(){
		if(Time.time > delay){
			Rigidbody bullet = (Rigidbody)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.localRotation);
			bullet.AddForce(bulletSpawn.forward * 1000);
			delay = Time.time + attck_rate;
		}
	}
}
