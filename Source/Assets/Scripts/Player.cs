using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float speed = 10;
	public float horizontalRotationSpeed = 1;
	public float verticalRotationSpeed = 0.5f;

	private Vector3 movement;
	
	void Start () {
		movement = Vector3.zero;
	}

	void FixedUpdate () {
		movement.z = 1;

		// Rotate plane
		rotateAroundZ ();
		rotateAroundX ();

		// Move plane
		rigidbody.AddForce(movement * speed);
	}

	public void moveSideways (float input) {
		movement.x = input;
	}

	public void moveUpOrDown (float input) {
		movement.y = input;
	}

	private void rotateAroundZ () {
		// If going left or right, rotate
		float zRotation = movement.x * -horizontalRotationSpeed;

		// Player is not going left or right, rotate back to horizontal
		if (zRotation == 0) {
			float diffToHorizontal = getDiffToHorizontal(transform.eulerAngles.z);
			zRotation = horizontalRotationSpeed * (diffToHorizontal > 0 ? 1 : -1);
			zRotation = -Mathf.Clamp(zRotation, Mathf.Min(0, diffToHorizontal), Mathf.Max(0, diffToHorizontal));
		}

		// Rotate player
		Quaternion rotation = transform.rotation;
		rotation *= Quaternion.Euler (0, 0, zRotation);
		transform.rotation = rotation;
	}

	private void rotateAroundX () {
		// If going up or down rotate
		float xRotation = movement.y * -verticalRotationSpeed;

		// Player is not going up or down, rotate back to horizontal
		if (xRotation == 0) {
			float diffToHorizontal = getDiffToHorizontal(transform.eulerAngles.x);
			xRotation = verticalRotationSpeed * (diffToHorizontal > 0 ? 1 : -1);
			xRotation = -Mathf.Clamp(xRotation, Mathf.Min(0, diffToHorizontal), Mathf.Max(0, diffToHorizontal));
		}

		// Rotate player
		Quaternion rotation = transform.rotation;
		rotation *= Quaternion.Euler (xRotation, 0, 0);
		transform.rotation = rotation;
	}

	// Returns an angle (in degrees) between -180 and 180. 
	// This is the difference of rotation with zero (z = zero is horizontal).
	private float getDiffToHorizontal (float rotation) {
		rotation = rotation % 360;

		if (rotation > 180)
			rotation -= 360;

		return rotation;
	}
}
