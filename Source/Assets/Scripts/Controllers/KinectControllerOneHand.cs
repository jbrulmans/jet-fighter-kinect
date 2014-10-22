using UnityEngine;
using System.Collections;

public class KinectControllerOneHand : MonoBehaviour, GestureListener {
	private Player player;

	//range for arm
	float arm_mid = 70.0f;
	float arm_mid_range = 10.0f;

	//range for leaning
	float linksStart = 95.0f;
	float linksStop = 120.0f;
	float rechtsStart = 85.0f;
	float rechtsStop = 60.0f;
	
	//store angles from the functions to use in the other function
	float aLeftRightLean = 1.0f;
	float angleLeftArm = 1.0f;


	void Start () {
		GestureDetector.addListener (this);
		player = this.GetComponent<Player> ();
	}
	
	void Update () {
	}
	
	public void leanGesture(float aLeftRight, float aFrontBack) {
		this.aLeftRightLean = aLeftRight;

		if (aLeftRight < rechtsStart) {
			player.autoBalancePlaneHorizontal(false);
			player.autoBalancePlaneVertical(false);
			if (aLeftRight <= rechtsStop) 
				player.moveSideways(0.99f);
			else {
				player.moveSideways(1 - (aLeftRight - rechtsStop)/(rechtsStart - rechtsStop));
			}
		}
		else if (aLeftRight > linksStart) {
			player.autoBalancePlaneHorizontal(false);
			player.autoBalancePlaneVertical(false);
			if (aLeftRight >= linksStop)
				player.moveSideways(-0.99f);
			else {
				player.moveSideways(-1 * ((aLeftRight-linksStart)/(linksStop-linksStart)));
			}
		}
		else if (angleLeftArm < arm_mid - arm_mid_range && angleLeftArm > arm_mid + arm_mid_range){
			player.autoBalancePlaneHorizontal(true);
			player.autoBalancePlaneVertical(true);
		}

	}
	
	
	public void armGesture(float angleLeft, float angleRight) {
		this.angleLeftArm = angleLeft;

		if (angleLeft < arm_mid - arm_mid_range) {
			player.autoBalancePlaneHorizontal(false);
			player.autoBalancePlaneVertical(false);
			player.moveUpOrDown (-1.0f);
		} else if (angleLeft > arm_mid + arm_mid_range) {
			player.autoBalancePlaneHorizontal(false);
			player.autoBalancePlaneVertical(false);
			player.moveUpOrDown (1.0f);
		} else if (aLeftRightLean < linksStart && aLeftRightLean > rechtsStart) {
			player.autoBalancePlaneHorizontal(true);
			player.autoBalancePlaneVertical(true);
		}
	}

}
