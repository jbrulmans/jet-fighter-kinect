using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// Speed
	public float speed = 10;
	public float horizontalRotationSpeed = 60;
	public float verticalRotationSpeed = 45;
	public float reverseSpeed = 180;

	// Machine gun
	public GameObject machineGun;
	public float timeBetweenBullets = 0.15f;
	public float range = 100f;

	// Position/control plane variables
	private Vector3 movement;
	private bool balanceHorizontal = false;
	private bool balanceVertical = false;
	private bool reversing = true;
	private Vector3 reverseTarget;

	// Machine gun variables
	private float bulletTimer;
	private ParticleSystem gunParticles;
	private LineRenderer gunLine;
	private Light gunLight;

	void Awake () {
		movement = Vector3.zero;
		reversing = false;
	
		// Machine gun components
		gunParticles = machineGun.GetComponent<ParticleSystem> ();
		gunLine = machineGun.GetComponent <LineRenderer> ();
		gunLight = machineGun.GetComponent<Light> ();
	}

	void Update () {
		bulletTimer += Time.deltaTime;

		// Show bullet effects only short period of time
		if(bulletTimer >= timeBetweenBullets * 0.2f)
			disableGunEffects ();
	}

	void FixedUpdate () {
		movement.z = 1;

		// Don't rotate or balance when reversing
		if (reversing)
			doReverse ();
		
		else {
			// Rotate plane
			rotateAroundZ ();
			rotateAroundX ();
			
			// Balance plane
			balance ();
		}

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
		// To soon to fire new bullet
		if (bulletTimer < timeBetweenBullets)
			return;

		bulletTimer = 0f;
		Ray shootRay = new Ray ();
		RaycastHit shootHit = new RaycastHit ();
		int shootableMask = LayerMask.GetMask ("Shootable");

		// Enable gun effects
		enableGunEffects ();

		// Set position gunLine
		gunLine.SetPosition (0, machineGun.transform.position);
		shootRay.origin = machineGun.transform.position;
		shootRay.direction = machineGun.transform.forward;

		// Shootable object is hit, stop line at object
		if(Physics.Raycast (shootRay, out shootHit, range, shootableMask)) {
			gunLine.SetPosition (1, shootHit.point);
		
		} else {
			gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
		}
	}

	// Plane will turn back, plane can't be controlled untill plane is completely reversed
	// This is meant for when the user reaches the end of the world
	public void reverse () {
		if (!reversing) {
			reversing = true;
			reverseTarget = new Vector3 (transform.eulerAngles.x + 180f, transform.eulerAngles.y, 0);
		}
	}

	// Rotate left/right
	private void rotateAroundZ () {
		float speed = horizontalRotationSpeed;
		float angle = -movement.x * speed * Time.deltaTime;
		angle *= 1.2f / 0.9f;

		// Rotate player
		transform.Rotate (0, 0, angle);
	}

	// Rotate up/down
	private void rotateAroundX () {
		float speed = verticalRotationSpeed;
		float angle = -movement.y * speed * Time.deltaTime;

		// If near 270 or 90 degrees, move at least 1.2 degrees
		if (Mathf.Abs (transform.eulerAngles.x - 270) < 5
		    || Mathf.Abs (transform.eulerAngles.x - 90) < 5) {
			if (Mathf.Abs (angle) < 1.2)
				angle = angle < 0 ? -1.2f : 1.2f;
		}

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

	// TODO: Doesn t work properly yet
	private void doReverse () {
		transform.rotation = Quaternion.RotateTowards (
			transform.rotation, 
			Quaternion.Euler(reverseTarget),
			reverseSpeed * Time.deltaTime
		);

		if (transform.rotation == Quaternion.Euler (reverseTarget))
			reversing = false;
	}

	private void disableGunEffects () {
		gunLine.enabled = false;
		gunLight.enabled = false;
	}

	private void enableGunEffects () {
		gunLight.enabled = true;
		gunLine.enabled = true;

		gunParticles.Stop ();
		gunParticles.Play ();
	}
}
