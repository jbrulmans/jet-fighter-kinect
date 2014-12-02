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
		if (aLeftRight < 110 && aLeftRight > 70) {
				//reset the relative rotation
				player.moveSideways (0.0f);
				//make the absolute rotation
				float angleLR = -((110 - aLeftRight) * 4.5f - 90);
				player.setZAxisAngle (angleLR);
				//player.setYZAxisAngle (angleLR);
		} else {
				//make the relative rotation
				player.moveSideways ((aLeftRight < 70) ? 1.0f : -1.0f);
				//adjust the previous angle to the current angle of the player
				player.setPreviousZAxisAngle ();
		}


		if (aFrontBack < 110 && aFrontBack > 70) {
				//reset the relative rotation
				player.moveUpOrDown (0.0f);
				//make the absolute rotation
				float angleUD = -((110 - aFrontBack) * 4.5f - 90);
				player.setXAxisAngle (angleUD);
		} else {
				//make the relative rotation
				player.moveUpOrDown ((aFrontBack < 70) ? 1.0f : -1.0f);
				//adjust the previous angle to the current angle of the player
				player.setPreviousXAxisAngle ();
		}
	}

	public void armGesture(float angleLeft, float angleRight) {
		if(!player.fixedSpeed)
			player.speed = angleRight+40;
	}


	public void pointGesture(float xMovement, float yMovement, bool selecting, string debug) {
		//constrain the movement 
		xMovement = Mathf.Clamp (xMovement*2, -1f, 1f);
		yMovement = Mathf.Clamp (yMovement*2, -1f, 1f);

		if (selecting) {
			player.selectTarget (yMovement, xMovement);
		} else {
			player.stopSelectingTargets ();
		}
	}
	
	public void machineGunGesture() {
		player.fireMachineGun();
	}

	public void missileGesture() {
		player.fireMissile ();
	}
}