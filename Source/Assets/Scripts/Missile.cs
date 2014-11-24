using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {
	public float speed = 200;

	private Enemy target = null;

	void Update () {
		if (target == null)
			return;

		// Move towards target
		Vector3 targetPosition = target.transform.position;
		Quaternion targetRotation = Quaternion.LookRotation (targetPosition - transform.position);
		transform.rotation = Quaternion.Slerp (
			transform.rotation, targetRotation, Time.deltaTime * 60 / 10f);

		// Distance to target
		if (Vector3.Distance (target.transform.position, this.transform.position) < 8f) {
			target.destroy();
			Destroy (gameObject);
		}
	}

	void FixedUpdate () {
		transform.Translate (0, 0, speed * Time.deltaTime);
	}

	public void setTarget (Enemy enemy) {
		this.target = enemy;
	}
}
