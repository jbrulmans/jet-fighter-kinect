using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public Player player;
	public int life = 100;
	public GameObject explosion;

	private bool visible;

	void Awake () {
		//Debug.Log ("" + this.transform.position.x + ", " +  this.transform.position.y + ", " + this.transform.position.z);
		visible = false;
	}

	void Start() {

		player.registerEnemy (this);
	}

	void FixedUpdate () {
		bool _visible = isVisible ();

		if (_visible != visible) {
			visible = _visible;
			player.enemyIsVisible (this, visible);
		}
	}

	public void hit (int damage) {
		life -= damage;

		if (life <= 0)
			destroy ();
	}

	public void destroy () {
		life = 0;
		//Instantiate (explosion, transform.position, Quaternion.identity);

		player.enemyIsVisible (this, false);
		Destroy (gameObject);
	}

	private bool isVisible () {
		Vector3 posViewport = Camera.main.WorldToViewportPoint (transform.position);

		return posViewport.x > 0 && posViewport.x < 1 
			&& posViewport.y > 0 && posViewport.y < 1 
			&& posViewport.z > 0;
	}
}
