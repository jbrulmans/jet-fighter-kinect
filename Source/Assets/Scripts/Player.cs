using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float speed = 10;
	public float horizontalRotationSpeed = 60;
	public float verticalRotationSpeed = 45;

	private Vector3 movement;
	private bool balanceHorizontal = false;
	private bool balanceVertical = false;
	
	void Start () {
		movement = Vector3.zero;
	}

	void FixedUpdate () {
		movement.z = 1;

		// Rotate plane
		rotateAroundZ ();
		rotateAroundX ();

		// Balance plane
		balance ();

		// Move plane
		transform.Translate (0, 0, speed * Time.deltaTime);
	}

	// Input must have a value between -1 and 1
	public void moveSideways (float input) {
		movement.x = input;
	}

	// Input must have a value between -1 and 1
	public void moveUpOrDown (float input) {
		movement.y = input;
	}

	// 
	public void autoBalancePlaneHorizontal (bool turnOn) {
		balanceHorizontal = turnOn;
	}

	public void autoBalancePlaneVertical (bool turnOn) {
		balanceVertical = turnOn;
	}

	public void fireMachineGun () {
		
	}

	private void rotateAroundZ () {
		float speed = horizontalRotationSpeed;
		float angle = -movement.x * speed * Time.deltaTime;
		float angle2 = 0;

		// Rotate player
		transform.Rotate (angle2, 0, angle);
	}

	private void rotateAroundX () {
		float speed = verticalRotationSpeed;
		float angle = -movement.y * speed * Time.deltaTime;

		// Rotate player
		transform.Rotate (angle, 0, 0);
	}

	private void balance () {
		float x = transform.eulerAngles.x;
		float z = transform.eulerAngles.z;

		if (balanceHorizontal) 
			z = 0;
		if (balanceVertical)
			x = 0;

		transform.rotation = Quaternion.RotateTowards (
			transform.rotation, 
			Quaternion.Euler(new Vector3(x, transform.eulerAngles.y, z)), 
			verticalRotationSpeed * Time.deltaTime
		);
	}
}
