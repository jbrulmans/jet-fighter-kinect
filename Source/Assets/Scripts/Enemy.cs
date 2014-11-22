﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	public Player player;
	public int life = 100;
	public GameObject explosion;
	public float speed = 40;
	public float RotationSpeed = 60;

	private bool visible;
	private bool ready = false;
	public enum Behaviour : int { Random, Patrol, Pursue }
	public Behaviour behaviour = Behaviour.Random;

	private Vector3 currentTarget = new Vector3(0.0f,0.0f,0.0f);
	public List<Vector3> patrolPositions;
	private Quaternion lookRotation;
	private int patrolCounter = 0;

	void Awake () {
		//Debug.Log ("" + this.transform.position.x + ", " +  this.transform.position.y + ", " + this.transform.position.z);
		visible = false;
	}

	void Start() {
		player.registerEnemy (this);
		newTargetPosition(behaviour);
	}

	void FixedUpdate () {
		bool _visible = isVisible ();

		if (_visible != visible) {
			visible = _visible;
			player.enemyIsVisible (this, visible);
		}

		var step = speed * Time.deltaTime;
		
		// Move our position a step closer to the target.
		transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);
		if (transform.position == currentTarget || behaviour == Behaviour.Pursue)
			newTargetPosition (behaviour);

		ready = true;
	}

	public void newTargetPosition(Behaviour behaviour) {
		//Makes the plane fly to a random location in a radius of up to 2000 meters away from the player
		if (behaviour == Behaviour.Random) {
			currentTarget = player.transform.position + (Random.onUnitSphere * 500);
			if (currentTarget.y < 0)
					currentTarget.y = 50;
		} else if (behaviour == Behaviour.Patrol) {
			currentTarget = patrolPositions[patrolCounter];
			patrolCounter++;
			if (patrolCounter >= patrolPositions.Count)
				patrolCounter = 0;
		} else if (behaviour == Behaviour.Pursue) {
			currentTarget = player.transform.position;
		}
		transform.LookAt(currentTarget);
		transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
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
		player.removeEnemy (this);
		Destroy (gameObject);
	}

	public bool isReady () {
		return ready;
	}

	private bool isVisible () {
		Vector3 posViewport = Camera.main.WorldToViewportPoint (transform.position);

		return posViewport.x > 0 && posViewport.x < 1 
			&& posViewport.y > 0 && posViewport.y < 1 
			&& posViewport.z > 0;
	}
}
