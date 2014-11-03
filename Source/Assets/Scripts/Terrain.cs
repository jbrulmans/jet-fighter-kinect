using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {
	public Player player;
	public GameObject fog, debug;
	public int radius = 200;

	private bool reversing = false;

	void Start () {
		// Instantiate and hide fog
		fog = Instantiate (fog) as GameObject;
		enableFog (false);

		// Debug: Show border terrain using spheres
	 	int n = 20;
	 	for (int i=0; i<n; i++) {
			float degr = (360 / n) * i;
			float x = (radius) * Mathf.Sin (Mathf.Deg2Rad * degr);
			float z = (radius) * Mathf.Cos (Mathf.Deg2Rad * degr);

			Instantiate (debug, transform.position + new Vector3 (x, 0, z), Quaternion.Euler (0, degr, 0));
		}
	}

	void Update () {
		float distance = distanceFromCenter (player.transform.position);

		// Enable fog if close enough
		enableFog (wallIsClose ());
		updateFogPos ();

		// If to far away from center terrain, reverse
		if (distance > radius && !reversing)
			player.reverse (doneReversing);
	}

	/// <summary>
	/// Updates the fog position if the fog is enabled.
	/// </summary>
	private void updateFogPos () {
		// Don t update position if fog is disabled
		if (!fog.renderer.enabled)
			return;

		// Update position and direction
		fog.transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 30));
		fog.transform.LookAt (Camera.main.transform.forward);
	}

	/// <summary>
	/// Returns if the wall is close.
	/// </summary>
	/// <returns><c>true</c>, if is close to wall, <c>false</c> otherwise.</returns>
	private bool wallIsClose () {
		Vector3 p = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 60));

		return distanceFromCenter (p) >= radius;
	}

	/// <summary>
	/// Returns the distance from point to center of terrain
	/// </summary>
	private float distanceFromCenter (Vector3 p) {
		Vector3 p1 = new Vector3 (p.x, 0, p.z);
		Vector3 p2 = new Vector3 (transform.position.x, 0, transform.position.z);

		return Vector3.Distance (p1, p2);
	}

	private void enableFog (bool enabled) {
		fog.renderer.enabled = enabled;
	}

	/// <summary>
	/// Call this when the player is done reversing his plane.
	/// </summary>
	public void doneReversing () {
		reversing = false;
	}
}
