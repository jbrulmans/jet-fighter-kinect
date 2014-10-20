using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float speed = 10;
	public float horizontalRotationSpeed = 60;
	public float verticalRotationSpeed = 45;

	private Vector3 movement;
	private Vector3 realRotation;
	
	void Start () {
		movement = Vector3.zero;
		realRotation = Vector3.zero;
	}

	void FixedUpdate () {
		movement.z = 1;

		// Rotate plane
		rotateAroundZ ();
		rotateAroundX ();

		/*if (movement.x == 0 && movement.y == 0) {
			//transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);

			float deltaX = getDiffToHorizontal(realRotation.x);
			float deltaZ = getDiffToHorizontal(realRotation.z);

			float angleX = verticalRotationSpeed * Time.deltaTime * (deltaX > 0 ? 1 : -1);
			float angleZ = horizontalRotationSpeed * Time.deltaTime * (deltaZ > 0 ? 1 : -1);

			angleX = -Mathf.Clamp(angleX, Mathf.Min(0, deltaX), Mathf.Max(0, deltaX));
			angleZ = -Mathf.Clamp(angleZ, Mathf.Min(0, deltaZ), Mathf.Max(0, deltaZ));

			//rotate (angleX, 0, angleZ);
			//Debug.Log(realRotation.x + " " + transform.eulerAngles.x + ", " + realRotation.z + " " + transform.eulerAngles.z);

			/*Vector3 dir = Vector3.RotateTowards (transform.position, new Vector3(0, transform.eulerAngles.y, 0), 
			                       verticalRotationSpeed * Time.deltaTime * Mathf.Deg2Rad, 1);
			transform.rotation = Quaternion.LookRotation(dir);*/

			/*transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0)), 
			                       verticalRotationSpeed * Time.deltaTime);*/

			//Debug.Log(realRotation.x + " " +realRotation.y + " " +realRotation.z);
		//}

		// Move plane
		//rigidbody.AddForce(movement * speed);
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

	public void fireMachineGun () {
		
	}

	private void rotateAroundZ () {
		float speed = horizontalRotationSpeed;
		float angle = -movement.x * speed * Time.deltaTime;
		float angle2 = 0;

		/*if (angle != 0) {
			angle2 = -Mathf.Abs(angle/2);
		}*/

		// Player is not going left or right, rotate back to horizontal
		/*if (angle == 0) {
			float delta = getDiffToHorizontal(realRotation.z);
			angle = speed * Time.deltaTime * (delta > 0 ? 1 : -1);
			angle = -Mathf.Clamp(angle, Mathf.Min(0, delta), Mathf.Max(0, delta));
		}*/

		// Rotate player
		rotate (angle2, 0, angle);
	}

	private void rotateAroundX () {
		float speed = verticalRotationSpeed;
		float angle = -movement.y * speed * Time.deltaTime;

		// Player is not going up or down, rotate back to horizontal
		/*if (angle == 0) {
			float delta = getDiffToHorizontal(realRotation.x);
			angle = speed * Time.deltaTime * (delta > 0 ? 1 : -1);
			angle = -Mathf.Clamp(angle, Mathf.Min(0, delta), Mathf.Max(0, delta));
		}*/

		// Rotate player
		rotate (angle, 0, 0);
	}

	// Returns an angle (in degrees) between -90 and 90. 
	// This is the difference of rotation with zero (z = zero is horizontal).
	private float getDiffToHorizontal (float rotation) {
		float output = 0;
		
		// Angle must be between 0 and 360
		rotation = rotation % 360;
		if (rotation < 0)
			rotation += 360;
		
		// Calculate angle
		if (rotation >= 0 && rotation <= 90)
			output = -rotation;
		else if (rotation > 90 && rotation <= 180)
			output = 180 - rotation;
		else if (rotation > 180 && rotation < 270)
			output = 180 - rotation;
		else 
			output = 360 - rotation;
		
		return -output;
	}

	private void rotate (float x, float y, float z) {
		realRotation += new Vector3(x, y, z);
		transform.Rotate (x, y, z);
		//transform.eulerAngles = realRotation;
	}
}
