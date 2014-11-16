using UnityEngine;
using System.Collections;

public class Kinect_Controller_Hands_Angle : MonoBehaviour, GestureListener {
	private Player player;

	// Use this for initialization
	void Start () {
		GestureDetector.addListener (this);
		player = this.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void leanGesture(float aLeftRight, float aFrontBack) {
		float angleLR = -((115 - aLeftRight) * 3.6f + (-90));
		float angleUD = -((120 - aFrontBack) * 3.0f + (-90));
		player.setXZAxisAngles (angleLR, angleUD);
		//player.setZAxisAngle(aLeftRight);
		//player.setXAxisAngle(aFrontBack);
		//Debug.Log ("Angle: " + aFrontBack); 
	}

	public void armGesture(float angleLeft, float angleRight) {
		;
	}

	public void pointGesture(float xMovement, float yMovement) {
		//Debug.Log (xMovement + " " + yMovement);
	}
	
	public void machineGunGesture() {
		player.fireMachineGun();
	}
}