using UnityEngine;
using System.Collections;

public class KinectControllerBen : MonoBehaviour, GestureListener {
	private Player player;

	
	float rollVal = 0.0f;
	float rollSpeed = 0.20f;
	float rollReturnSpeed = 0.40f;
	float pitchVal = 0.0f;
	float pitchSpeed = 0.05f;
	float pitchReturnSpeed = 0.10f;

	//calibrate
	float defaultLeftRightAngle = 0.0f;
	float defaultFrontBackAngle = 0.0f;
	//range for leaning
	private float lean_frontback_range = 5.0f;
	private float lean_leftright_range = 5.0f;
	//autopilot
	private bool autoPilot = false;
	private float autoPilotThreshold = 45.0f;
	private bool calibrate = true;

	void Start () {
		GestureDetector.addListener (this);
		player = this.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void leanGesture(float aLeftRight, float aFrontBack) {
		if(autoPilot)
		{
			Debug.Log ("autoPilot");
			//autopilot code for going up and down
			player.startAutoPilot();
			return;
		}
		if(calibrate) //if we just switched from autopilot to non autopilot.
		{
			Debug.Log ("calibrate");
			//calibrate
			defaultLeftRightAngle = aLeftRight;
			defaultFrontBackAngle = aFrontBack;
			//stop autopilot
			player.stopAutoPilot();
			calibrate = false;
		}

		float aLR = defaultLeftRightAngle - aLeftRight;
		if (Mathf.Abs(aLR) > lean_leftright_range) {
			if (aLR < 0) {
				//rollVal -= rollSpeed;
				//rollVal = Mathf.Max (rollVal, -1.0f);
				rollVal = -1.0f;
			} else {
				//rollVal += rollSpeed;
				//rollVal = Mathf.Min (rollVal, 1.0f);
				rollVal = 1.0f;
			}
		}
		else {
			//float toZero = rollVal>0?-1:+1;
			//rollVal += toZero * rollReturnSpeed;
			rollVal = 0.0f;
		}
		player.moveSideways(rollVal);

		float aFB = defaultFrontBackAngle - aFrontBack;
		if (Mathf.Abs(aFB) > lean_frontback_range) {
			if (aFB < 0) {
				//pitchVal -= pitchSpeed;
				//pitchVal = Mathf.Max (pitchVal, -1.0f);
				pitchVal = -1.0f;
			} else {
				//pitchVal += pitchSpeed;
				//pitchVal = Mathf.Min (pitchVal, 1.0f);
				pitchVal = 1.0f;
			}
		}
		else {
			//float toZero = pitchVal>0?-1:+1;
			//pitchVal += toZero * pitchSpeed * 2;
			pitchVal = 0.0f;
		}
		player.moveUpOrDown(pitchVal);


		/*
		if (aLeftRight < 110 && aLeftRight > 70) {
			//reset the relative rotation
			player.moveSideways (0.0f);
			//make the absolute rotation
			float angleLR = -((110 - aLeftRight) * 4.5f - 90);
			player.setZAxisAngle (angleLR);
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
		*/

	}

	public void armGesture(float angleLeft, float angleRight) {
		bool prevAutoPilot = autoPilot;
		autoPilot = angleLeft < autoPilotThreshold;
		calibrate = prevAutoPilot && !autoPilot;
	}

	public void pointGesture(float xMovement, float yMovement, bool select) {
	
	}

	public void machineGunGesture() {
	
	}
}
