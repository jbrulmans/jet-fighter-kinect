using UnityEngine;
using System.Collections;

public class KinectControllerTwoHandsAutoPilot : MonoBehaviour, GestureListener {
	private Player player;
	
	//range for arm
	private float arm_mid = 70.0f;
	private float arm_mid_range = 10.0f;
	private float autoPilotThreshold = 30.0f;

	//range for leaning
	private float lean_mid = 90.0f;
	private float lean_mid_range = 10.0f;


	//speeds
	float rollSpeed = 0.01f;
	float rollVal = 0.0f;
	float rollReturnSpeed = 0.02f;
	float pitchSpeed = 0.01f;
	float pitchVal = 0.0f;
	float pitchReturnSpeed = 0.02f;
	
	//store angles from the functions to use in the other function
	private float aLeftRightLean = 1.0f;
	private float angleArms = 1.0f;

	private bool autoPilot = false;

	void Start () {
		GestureDetector.addListener (this);
		player = this.GetComponent<Player> ();
	}
	
	void Update () {
	}

	public void leanGesture(float aLeftRight, float aFrontBack) {
		aLeftRightLean = aLeftRight;

		if (autoPilot) {
			//autopilot code for going left and right
			player.startAutoPilot ();
			return;
		} else {
			player.stopAutoPilot();
		}

		if (aLeftRight > lean_mid + lean_mid_range || aLeftRight < lean_mid - lean_mid_range) {
			if (aLeftRight > lean_mid + lean_mid_range) {
				rollVal -= rollSpeed;
				rollVal = Mathf.Max (rollVal, -1.0f);
			} else {
				rollVal += rollSpeed;
				rollVal = Mathf.Min (rollVal, 1.0f);
			}
		}
		else {
			float toZero = rollVal>0?-1:+1;
			rollVal += toZero * rollReturnSpeed;
		}
		player.moveSideways(rollVal);

	}
	
	
	public void armGesture(float angleLeft, float angleRight) {
		angleArms = angleLeft;
		autoPilot = this.angleArms < autoPilotThreshold;

		if(autoPilot){
			//autopilot code for going up and down
			player.startAutoPilot();
			return;
		} else {
			player.stopAutoPilot();
		}

		if (angleArms > arm_mid + arm_mid_range || angleArms < arm_mid - arm_mid_range) {
			if (angleArms > arm_mid + arm_mid_range) {
				pitchVal -= pitchSpeed;
				pitchVal = Mathf.Max (pitchVal, -1.0f);
			} else {
				pitchVal += pitchSpeed;
				pitchVal = Mathf.Min (pitchVal, 1.0f);
			}
		}
		else {
			float toZero = pitchVal>0?-1:+1;
			pitchVal += toZero * pitchSpeed * 2;
		}
		player.moveUpOrDown(pitchVal);
	}

	public void pointGesture(float xMovement, float yMovement, bool select) {
		if (!autoPilot)
			return;
		
	}
	
	public void machineGunGesture() {
		if (!autoPilot)
			return;
		player.fireMachineGun();
	}

	public void missileGesture() {
		
	}
}
