using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public Player player;

	private bool visible;

	void Awake () {
		visible = false;
	}

	void FixedUpdate () {
		bool _visible = isVisible ();

		if (_visible != visible) {
			visible = _visible;
			player.enemyIsVisible (this, visible);
		}
	}

	private bool isVisible () {
		Vector3 posViewport = Camera.main.WorldToViewportPoint (transform.position);

		return posViewport.x > 0 && posViewport.x < 1 
			&& posViewport.y > 0 && posViewport.y < 1 
			&& posViewport.z > 0;
	}
}
