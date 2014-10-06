using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {
	private static string objectContainerTag = "Objects";
	private static float translateSpeed = 3;

	public GameObject _camera;

	private Vector3 crosshairPos;
	private bool rotating = false;

	private GameObject selectedObject = null;
	private float objectDistance = 0;
	private Quaternion objectRotation;

	
	void Start () {
		crosshairPos = new Vector3 (
			Screen.width / 2, Screen.height / 2);
	}

	void Update () {
		// Left mouse (Selecting objects)
		if (Input.GetMouseButtonDown (0))
			leftMouseIsDown();

		// Right mouse (Rotating objects)
		if (Input.GetKeyDown(KeyCode.LeftControl))
			rotateIsDown();
		else if (Input.GetKeyUp(KeyCode.LeftControl))
			rotateIsUp ();

		// Mouse wheel (Translating objects)
		float scrolled = Input.GetAxis ("Mouse ScrollWheel");
		if (scrolled != 0)
			mouseIsScrolled (scrolled);

		if (rotating) {
			float xDiff = Input.GetAxis("Mouse X")*10F;
			float yDiff = Input.GetAxis("Mouse Y")*10F;

			if (xDiff != 0)
				selectedObject.transform.Rotate(Vector3.up * xDiff/2);
			if (yDiff != 0)
				selectedObject.transform.Rotate(Vector3.left * yDiff/2);

			if (xDiff != 0 || yDiff != 0)
				objectRotation = selectedObject.transform.rotation;
		}

		updatePositionSelectedObject ();
	}

	void leftMouseIsDown () {
		// An object is already selected, release it
		if (selectedObject != null) {
			releaseObject();
			return;
		}

		// Check if an object is clicked
		Ray ray = _camera.camera.ScreenPointToRay(crosshairPos);
		
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)){
			GameObject parent = GameObject.FindGameObjectWithTag(objectContainerTag);
			
			// Is clicked object child of "Objects" ?
			if (hit.transform.IsChildOf(parent.transform))
				selectObject(hit.transform.gameObject);
		}
	}

	void rotateIsDown () {
		// Freeze camera rotation if an object is selected
		freezeCamera (selectedObject != null);

		if (selectedObject != null) {
			rotating = true;
		}
	}

	void rotateIsUp () {
		freezeCamera(false);
		rotating = false;
	}

	void freezeCamera (bool freeze) {
		// Freeze script on camera (up and down)
		MouseLook mouseLook = _camera.GetComponent<MouseLook> ();
		mouseLook.freeze = freeze;

		// Freeze script on first person controller (left and right)
		mouseLook = _camera.transform.parent.gameObject.GetComponent<MouseLook> ();
		mouseLook.freeze = freeze;
	}

	// Move object closer or further away from player
	void mouseIsScrolled (float delta) {
		if (selectedObject == null)
			return;

		// Move object
		objectDistance += delta * translateSpeed;

		// Object must be in front of player
		if (objectDistance < 1)
			objectDistance = 1;
	}

	// Select object
	void selectObject (GameObject obj) {
		selectedObject = obj;
		objectDistance = Vector3.Distance(obj.transform.position, this.transform.position);
		objectRotation = obj.transform.rotation;
	}

	// Release selected object
	void releaseObject () {
		selectedObject.rigidbody.velocity = Vector3.zero;
		selectedObject = null;
	}

	void updatePositionSelectedObject () {
		if (selectedObject == null)
			return;

		// Calculate target position
		Ray ray = _camera.camera.ScreenPointToRay(crosshairPos);
		Vector3 targetPos = ray.origin + (ray.direction * objectDistance);

		// Update position and rotation
		selectedObject.transform.position = targetPos;
		selectedObject.transform.rotation = objectRotation;
	}
}
