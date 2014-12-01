using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	// Speed
	public float speed = 10;
	public float horizontalRotationSpeed = 60;
	public float verticalRotationSpeed = 45;
	public float reverseSpeed = 180;

	// Weapons
	public GameObject machineGun, missile;
	public float timeBetweenBullets = 0.15f, timeBetweenMissiles = 3f;
	public float range = 400f;

	// Position/control plane variables
	private Vector3 movement;
	private bool balanceHorizontal = false;
	private bool balanceVertical = false;
	private bool reversing = true;
	private Vector3 reverseTarget;
	private Action reverseCallback;
	private Vector3 crosshairPos;
	private bool autopilot = false;

	// Machine gun variables
	private float bulletTimer, missileTimer = 3f;
	private ParticleSystem gunParticles;
	private LineRenderer gunLine;
	private Light gunLight;
	private AudioSource gunShot;

	// Enemies
	private bool enemiesReady;
	private List<Enemy> targetsVisible;
	private List<Enemy> enemiesList;
	private Enemy target = null, targetStart = null;

	// Objectives
	private int count;
	private int countObjectives;
	public GUIText countText;
	public GUIText objectiveDoneText;
	public GUIText ringsCollectedText;
	public GameObject objectivesGameObject;

	// Explosions
	public Detonator detonator;
	private AudioSource explosionSound;

	//previous angles
	float prev_left_right = 0;
	float prev_front_back = 0;

	void Awake () {
		movement = Vector3.zero;
		reversing = false;
		targetsVisible = new List<Enemy> ();
		enemiesList = new List<Enemy> ();
	
		// Machine gun components
		gunParticles = machineGun.GetComponent<ParticleSystem> ();
		gunLine = machineGun.GetComponent <LineRenderer> ();
		gunLight = machineGun.GetComponent<Light> ();
	}

	void Start () {
		crosshairPos = new Vector3 (Screen.width / 2, Screen.height / 2);
		// Objectives
		if (objectivesGameObject.active) {
			count = 0;
			objectiveDoneText.text = "";
			ringsCollectedText.text = "";
			countObjectives = countTotalObjectives ();
			setCountText ();
		}
		// Explosions
		detonator =	GetComponent("Detonator") as Detonator;
		// Sounds
		AudioSource[] aSources = this.GetComponents<AudioSource>();
		gunShot = aSources [0];
		explosionSound = aSources [1];
	}

	void Update () {
		bulletTimer += Time.deltaTime;
		missileTimer += Time.deltaTime;

		// Show bullet effects only short period of time
		if(bulletTimer >= timeBetweenBullets * 0.2f)
			disableGunEffects ();

		// Check if enemies are ready
		if (!enemiesReady) {
			if (areEnemiesReady()) {
				enemiesReady = true;
				enemyIsVisible(null, false);
			}
		}
	}

	void FixedUpdate () {
		movement.z = 1;

		if (autopilot)
			followTarget ();

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

	// Collision Detector
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Ring") {
			other.gameObject.SetActive (false);
			count++;
			setCountText();
		}

		if (other.gameObject.tag == "Finish") {
			objectiveDoneText.text = "Course Finished!";
			ringsCollectedText.text = "Collected: " + count.ToString() + "/" + countObjectives.ToString();
		}

		if (other.gameObject.tag == "Terrain") {
			detonator.Explode();
			explosionSound.Play();
			//this.gameObject.SetActive(false);
			Destroy(this);
		}
	}

	public void setCountText() {
		countText.text = "Rings collected: " + count.ToString () + "/" + countObjectives.ToString ();
	}

	private int countTotalObjectives() {
		return GameObject.FindGameObjectsWithTag("Ring").Length;
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

		RaycastHit shootHit = new RaycastHit ();
		int shootableMask = LayerMask.GetMask ("Enemy");
		bulletTimer = 0f;

		gunShot.Play ();
		enableGunEffects ();

		// Set position gunLine
		gunLine.SetPosition (0, machineGun.transform.position);

		// Aim point machinegun
		Ray ray = Camera.main.ScreenPointToRay (crosshairPos);
		Vector3 target = ray.GetPoint (range);

		// Shootable object is hit, stop line at object
		if(Physics.Raycast (ray, out shootHit, range, shootableMask)) {
			gunLine.SetPosition (1, shootHit.point);
			Enemy enemy = shootHit.transform.gameObject.GetComponent<Enemy>();
			enemy.hit (10);
		
		// Nothing is hit
		} else {
			gunLine.SetPosition (1, target);
		}
	}

	public void fireMissile () {
		if (missileTimer < timeBetweenMissiles || target == null)
			return;

		missileTimer = 0f;
		GameObject m = Instantiate (this.missile, transform.position, transform.rotation) as GameObject;
		Missile missile = m.GetComponent<Missile> ();
		missile.setTarget (target);
	}


	/// <summary>
	/// Plane will turn back, plane can't be controlled untill plane is completely reversed.
	/// This is meant for when the user reaches the end of the world.
	/// </summary>
	/// <param name="callback">Function to call, when plane is finished reversing.</param>
	public void reverse (Action callback) {
		if (!reversing) {
			reversing = true;
			reverseTarget = new Vector3 (transform.eulerAngles.x + 180f, transform.eulerAngles.y, 0);
			reverseCallback = callback;
		}
	}

	/// <summary>
	/// Selects a target plane (if visible).
	/// </summary>
	/// <param name="downUp">Value between -1 and 1. -1 Means choose lowest target.</param>
	/// <param name="leftRight">Value between -1 and 1. -1 Means most left target.</param>
	public void selectTarget (float downUp, float leftRight) {
		int minX = int.MaxValue, minY = int.MaxValue;
		int maxX = int.MinValue, maxY = int.MinValue;

		// Check if there are enemies visible
		if (targetsVisible.Count == 0)
			return;

		// Get bounding box around all visible enemies
		foreach (Enemy e in targetsVisible) {
			int x = (int) Camera.main.WorldToScreenPoint (e.transform.position).x;
			int y = (int) Camera.main.WorldToScreenPoint (e.transform.position).y;

			minX = Mathf.Min (minX, x);
			maxX = Mathf.Max (maxX, x);

			minY = Mathf.Min (minY, y);
			maxY = Mathf.Max (maxY, y);
		}

		// Find point
		Vector3 pos = Camera.main.WorldToScreenPoint (targetStart.transform.position);
		float xDistance = Math.Abs (pos.x - maxX);
		float yDistance = Math.Abs (pos.y - maxY);

		if (leftRight < 0)
			xDistance =  Math.Abs (pos.x - minX);
		if (downUp < 0)
			yDistance = Math.Abs (pos.y - minX);

		target = getClosestTarget (new Vector3 (
			pos.x + (xDistance * leftRight),
			pos.y + (yDistance * downUp)), true);
	}

	public void startAutoPilot () {
		autopilot = true;
	}

	public void stopAutoPilot () {
		autopilot = true;
	}

	public void stopSelectingTargets () {
		targetStart = target;
	}

	public bool targetIsLocked () {
		return targetStart == target;
	}

	// Call this function when an enemy enters or leaves screen
	public void enemyIsVisible (Enemy enemy, bool visible) {
		// Remove target from list if no longer visible
		if (!visible)
			targetsVisible.Remove (enemy);

		// Add target to list if visible
		else if (!targetsVisible.Contains (enemy))
			targetsVisible.Add (enemy);

		if (enemiesReady) {
			// Remove target if no longer visible
			if (!visible && target == enemy)
					target = null;

			// Select target if none selected
			if (target == null && targetsVisible.Count > 0) {
				target = getMiddleTarget ();
				targetStart = target;
			}
		}
	}

	/// <summary>
	/// Registers the enemy.
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	public void registerEnemy (Enemy enemy) {
		enemiesList.Add(enemy);
	}

	public void removeEnemy (Enemy enemy) {
		enemiesList.Remove (enemy);
	}

	public List<Enemy> getEnemies () {
		return enemiesList;
	}

	public List<GameObject> getRings() {
		List<GameObject> rings = new List<GameObject>();
		GameObject[] gos = GameObject.FindGameObjectsWithTag ("Ring");
		for(int i = 0; i < gos.Length; i++) {
			rings.Add(gos[i]);
		}
		return rings;
	}

	public void setZAxisAngle(float aLeftRight) {
		transform.Rotate (0, 0, aLeftRight-prev_left_right);
		prev_left_right = aLeftRight;
	}

	public void setYAxisAngle(float aLeftRight) {
		transform.Rotate (0, aLeftRight - prev_left_right, 0);
		prev_left_right = aLeftRight;
	}
	
	public void setXAxisAngle(float aFrontBack) {
		transform.Rotate (aFrontBack-prev_front_back, 0, 0);
		prev_front_back = aFrontBack;
	}

	// Tryout
	public void setYZAxisAngle (float aLeftRight) {
		transform.Rotate (0, (-1.3f) * (aLeftRight-prev_left_right), aLeftRight-prev_left_right);
		prev_left_right = aLeftRight;
	}

	public void setPreviousZAxisAngle() {
		prev_left_right = transform.eulerAngles.z;
	}

	public void setPreviousXAxisAngle() {
		prev_front_back = transform.eulerAngles.x;
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

		if (transform.rotation == Quaternion.Euler (reverseTarget)) {
			reversing = false;
			reverseCallback ();
		}
	}

	private void disableGunEffects () {
		gunLine.enabled = false;
		gunLight.enabled = false;
	}

	private void enableGunEffects () {
		gunLight.enabled = true;
		gunLine.enabled = true;

		//gunParticles.Stop ();
		gunParticles.Emit (500);
		//gunParticles.Play ();
	}

	private Enemy getMiddleTarget () {
		Vector3 middle = Camera.main.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f, 0f));

		return getClosestTarget (middle, false);
	}

	private void followTarget () {
		if (target == null)
			return;
	
		// Calculate target rotation, and slowly turn to that rotation
		Vector3 targetPosition = target.transform.position;
		Quaternion targetRotation = Quaternion.LookRotation (targetPosition - transform.position);
		transform.rotation = Quaternion.Slerp (
			transform.rotation, targetRotation, Time.deltaTime * horizontalRotationSpeed / 10f);
	}

	private Enemy getClosestTarget (Vector3 screenTarget, bool ignoreZ) {
		// TODO: Remove later (usefull for debugging)
		// test = screenTarget;
		float minDistance = float.MaxValue;
		Enemy minEnemy = null;
		
		foreach (Enemy e in targetsVisible) {
			if (e == targetStart)
				;//continue;

			Vector3 screenPos = Camera.main.WorldToScreenPoint (e.transform.position);
			if (ignoreZ)
				screenPos.z = 0;

			float distance = Vector3.Distance (screenTarget, screenPos);
			if (distance < minDistance) {
				minDistance = distance;
				minEnemy = e;
			}
		}
		
		return minEnemy;
	}

	private bool areEnemiesReady () {
		bool ready = true;
		
		GameObject container = GameObject.FindGameObjectWithTag ("EnemyContainer");
		if (container == null)
			return false;
		
		Enemy[] enemies = container.GetComponents<Enemy> ();
		if (enemies == null)
			return false;
		
		foreach (Enemy e in enemies) {
			if (!e.isReady()) 
				ready = false;
		}
		
		return ready;
	}

	public List<Enemy> getVisibleTargets () { return targetsVisible; }
	public Enemy getTarget () { return target; }
}
