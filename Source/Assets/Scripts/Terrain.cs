using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {
	public Player player;
	public int limit = 200;

	private bool reversing = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 p1 = new Vector3 (player.transform.position.x, 0, player.transform.position.z);
		Vector3 p2 = new Vector3 (transform.position.x, 0, transform.position.z);

		if (Vector3.Distance (p1, p2) > limit) {
			if (!reversing) {
				player.reverse (doneReversing);
			}
		}
	}

	public void doneReversing () {
		reversing = false;
	}
}
