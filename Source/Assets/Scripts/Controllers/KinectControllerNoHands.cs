﻿using UnityEngine;
using System.Collections;

public class KinectControllerNoHands : MonoBehaviour, GestureListener {
	private Player player;

	void Start () {
		GestureDetector.addListener (this);
		player = this.GetComponent<Player> ();
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2)){
			player.stopSelectingTargets();
		}
	}

	public void leanGesture(float aLeftRight, float aFrontBack) {
		float achteroverStart = 82.0f;
		float achteroverStop = 70.0f;
		float vooroverStart = 92.0f;
		float vooroverStop = 100.0f;													
		
		float linksStart = 95.0f;
		float linksStop = 121.0f;
		float rechtsStart = 85.0f;
		float rechtsStop = 60.0f;
		
		//getallekes waren: 86, 55 en 94, 125
		
		if (aLeftRight < rechtsStart) {
			player.autoBalancePlaneVertical(false);
			player.autoBalancePlaneHorizontal(false);
			if (aLeftRight <= rechtsStop) 
				player.moveSideways(0.99f);
			else {
				player.moveSideways(1 - (aLeftRight - rechtsStop)/(rechtsStart - rechtsStop));
			}
		}
		else if (aLeftRight > linksStart) {
			player.autoBalancePlaneVertical(false);
			player.autoBalancePlaneHorizontal(false);
			if (aLeftRight >= linksStop)
				player.moveSideways(-0.99f);
			else {
				player.moveSideways(-1 * ((aLeftRight-linksStart)/(linksStop-linksStart)));
			}
		}
		else if (aFrontBack <= vooroverStart && aFrontBack >= achteroverStart){
			player.autoBalancePlaneHorizontal(true);
			player.autoBalancePlaneVertical(true);
		}
		
		
		// Achterover
		if (aFrontBack < achteroverStart) {
			player.autoBalancePlaneVertical(false);
			player.autoBalancePlaneHorizontal(false);
			if (aFrontBack <= achteroverStop)
				player.moveUpOrDown (0.99f);
			else {
				player.moveUpOrDown(1 - (aFrontBack - achteroverStop)/(achteroverStart - achteroverStop));
			}
		}
		
		// Voorover
		else if (aFrontBack > vooroverStart) {
			player.autoBalancePlaneVertical(false);
			player.autoBalancePlaneHorizontal(false);
			if (aFrontBack >= vooroverStop)
				player.moveUpOrDown (-0.99f);
			else {
				player.moveUpOrDown(-1 * ((aFrontBack-vooroverStart)/(vooroverStop-vooroverStart)));
			}
		}
		else if (aLeftRight >= rechtsStart && aLeftRight <= linksStart){
			player.autoBalancePlaneHorizontal(true);
			player.autoBalancePlaneVertical(true);
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
		} /*else {
			player.stopSelectingTargets ();
		}*/
	}

	public void machineGunGesture() {
		player.fireMachineGun();
	}

	public void missileGesture() {
		player.fireMissile ();
	}
}
