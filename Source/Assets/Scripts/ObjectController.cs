using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {
	private static string objectContainerTag = "Objects";

	public GameObject _camera;

	private Vector3 crosshairPos;
	private GameObject selectedObject = null;
	private float selectedObjectDistance = 0;
	
	void Start () {
		crosshairPos = new Vector3 (
			Screen.width / 2, Screen.height / 2);
	}

	void Update () {
		// If user clicked, find clicked object
		if (Input.GetMouseButtonDown (0)) {
			mouseIsDown();
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

	// Select object
	void selectObject (GameObject obj) {
		selectedObject = obj;
		selectedObjectDistance = Vector3.Distance(obj.transform.position, this.transform.position);

		;//Destroy(obj);
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
		Vector3 targetPos = ray.origin + (ray.direction * selectedObjectDistance);


		selectedObject.transform.position = targetPos;
	}
}
