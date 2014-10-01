using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {
	private static string objectContainerTag = "Objects";
	private static float translateSpeed = 3;

	public GameObject _camera;

	private Vector3 crosshairPos;
	private GameObject selectedObject = null;
	private float objectDistance = 0;
	private Quaternion objectRotation;

	
	void Start () {
		crosshairPos = new Vector3 (
			Screen.width / 2, Screen.height / 2);
	}

	void Update () {
		// If user clicked, find clicked object
		if (Input.GetMouseButtonDown (0)) {
			mouseIsDown();
		}

		// Mouse wheel is used
		float scrolled = Input.GetAxis ("Mouse ScrollWheel");
		if (scrolled != 0) {
			mouseIsScrolled (scrolled);
		}

		updatePositionSelectedObject ();
	}

	void mouseIsDown () {
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
