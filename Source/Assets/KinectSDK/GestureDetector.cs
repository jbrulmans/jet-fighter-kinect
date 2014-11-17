using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureDetector {
	private static List<GestureListener> listeners = new List<GestureListener> ();

	private const int leftHandIndex = (int)KinectWrapper.SkeletonJoint.LEFT_HAND;
	private const int rightHandIndex = (int)KinectWrapper.SkeletonJoint.RIGHT_HAND;
	private const int leftElbowIndex = (int)KinectWrapper.SkeletonJoint.LEFT_ELBOW;
	private const int rightElbowIndex = (int)KinectWrapper.SkeletonJoint.RIGHT_ELBOW;
	private const int leftShoulderIndex = (int)KinectWrapper.SkeletonJoint.LEFT_SHOULDER;
	private const int rightShoulderIndex = (int)KinectWrapper.SkeletonJoint.RIGHT_SHOULDER;
	private const int hipCenterIndex = (int)KinectWrapper.SkeletonJoint.HIPS;
	private const int shoulderCenterIndex = (int)KinectWrapper.SkeletonJoint.NECK;
	private const int leftHipIndex = (int)KinectWrapper.SkeletonJoint.LEFT_HIP;
	private const int rightHipIndex = (int)KinectWrapper.SkeletonJoint.RIGHT_HIP;
	private const int leftKneeIndex = (int)KinectWrapper.SkeletonJoint.LEFT_KNEE;
	private const int rightKneeIndex = (int)KinectWrapper.SkeletonJoint.RIGHT_KNEE;
	private const int headIndex = (int)KinectWrapper.SkeletonJoint.HEAD;

	// Helper function
	private static void SetGestureJoint (ref KinectGestures.GestureData gestureData, float timestamp, int joint, Vector3 jointPos) {
		gestureData.joint = joint;
		gestureData.jointPos = jointPos;
		gestureData.timestamp = timestamp;
		gestureData.state++;
	}

	// Helper function
	private static void SetGestureCancelled (ref KinectGestures.GestureData gestureData) {
		gestureData.state = 0;
		gestureData.progress = 0f;
		gestureData.cancelled = true;
	}

	public static void addListener (GestureListener listener) {
		listeners.Add (listener);
	}

	// Check what gesture and call GestureListeners
	public static void CheckForGesture (uint userId, ref KinectGestures.GestureData gestureData, 
		float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked) {

		if (gestureData.complete)
			return;
		detectLeaning (ref gestureData, timestamp, ref jointsPos, ref jointsTracked);
		detectArm (ref gestureData, timestamp, ref jointsPos, ref jointsTracked);
		detectPointing (ref gestureData, timestamp, ref jointsPos, ref jointsTracked);
		detectMachineGun (ref gestureData, timestamp, ref jointsPos, ref jointsTracked);
	}

	private static void detectLeaning (ref KinectGestures.GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked) {
		switch (gestureData.state) {
		case 0: 
			if (jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex]) { 
				Vector3 mid_shoulders = jointsPos [shoulderCenterIndex];
				Vector3 mid_hips = jointsPos [hipCenterIndex];
				Vector3 ground_far_x = mid_hips;
				ground_far_x.Set ((mid_hips.x + 20), mid_hips.y, mid_hips.z);
				
				Vector3 ground_far_z = mid_hips;
				ground_far_z.Set (mid_hips.x, mid_hips.y, (mid_hips.z +	 20));
				
				Vector3 vectorLeftRight1 = ground_far_x - mid_hips;
				Vector3 vectorLeftRight2 = mid_shoulders - mid_hips;
				float aLeftRight = Vector3.Angle (vectorLeftRight1, vectorLeftRight2);
				
				Vector3 vectorFrontBack1 = ground_far_z - mid_hips;
				Vector3 vectorFrontBack2 = mid_shoulders - mid_hips;
				
				float aFrontBack = Vector3.Angle (vectorFrontBack1, vectorFrontBack2);

				sendLeanGesture (aLeftRight, aFrontBack);
				SetGestureCancelled (ref gestureData);
			}
			break;
		}
	}

	private static void detectArm (ref KinectGestures.GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked) {
		switch (gestureData.state) {
		case 0: 
			if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex]) {
				Vector3 mid_hips = jointsPos [hipCenterIndex];
				Vector3 mid_shoulders = jointsPos [shoulderCenterIndex];
				Vector3 right_hand = jointsPos [rightHandIndex];
				Vector3 left_hand = jointsPos [leftHandIndex];
				
				Vector3 vectorRight = right_hand - mid_shoulders;
				Vector3 vectorLeft = left_hand - mid_shoulders;
				Vector3 vectorMid = mid_hips - mid_shoulders;

				float angleRight = Vector3.Angle (vectorRight, vectorMid);
				float angleLeft = Vector3.Angle (vectorLeft, vectorMid);

				sendArmGesture (angleLeft, angleRight);
				SetGestureCancelled (ref gestureData);

			}
			break;
		}
	}

	
	private static void detectPointing (ref KinectGestures.GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked) {
		float threshold = 0.10f;
		switch (gestureData.state) {
		case 0: 
			if (jointsTracked [rightHandIndex] && jointsTracked[rightShoulderIndex]) {
				Vector3 right_shoulder = jointsPos [rightShoulderIndex];
				Vector3 right_hand = jointsPos [rightHandIndex];
				

				//x position of the right hand same as right shoulder
				bool rightHandXisRightHandShoulderX = Mathf.Abs(right_hand.x - right_shoulder.x) < threshold;
				//y position of the right hand same as right shoulder
				bool rightHandYisRightHandShoulderY = Mathf.Abs(right_hand.y - right_shoulder.y) < threshold;
				//z position of the hand lower than the z position of the shoulder 
				bool righthandInFrontOhShoulder = right_hand.z < right_shoulder.z;
				if(rightHandXisRightHandShoulderX && rightHandYisRightHandShoulderY && rightHandXisRightHandShoulderX){
					SetGestureJoint(ref gestureData, timestamp, rightHandIndex, right_hand);
				}
			}
			break;
		case 1:
			if(jointsTracked [rightHandIndex] && jointsTracked[rightShoulderIndex] && gestureData.joint == rightHandIndex ) {
				Vector3 cur_right_hand = jointsPos [rightHandIndex];
				Vector3 prev_right_hand = gestureData.jointPos;
				Vector3 right_shoulder = jointsPos [rightShoulderIndex];

				bool inPose = Mathf.Abs(cur_right_hand.z - prev_right_hand.z) < threshold;
				if(inPose) {
					float xMovement = cur_right_hand.x-prev_right_hand.x;
					float yMovement = cur_right_hand.y-prev_right_hand.y;
					sendPointGesture (xMovement, yMovement, false);
				}
				else {
					//cancel gesture, so we can do it again from the start if needed!
					SetGestureCancelled (ref gestureData);
					sendPointGesture (0, 0, false);
				}
			}
			break;
		}
	}

	private static void detectMachineGun (ref KinectGestures.GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked) {
		switch (gestureData.state) {
		case 0: 
			if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[leftShoulderIndex]) {

				Vector3 right_elbow = jointsPos [rightElbowIndex];
				Vector3 left_elbow = jointsPos [leftElbowIndex];
				Vector3 right_hand = jointsPos [rightHandIndex];
				Vector3 left_hand = jointsPos [leftHandIndex];
				Vector3 right_shoulder = jointsPos [rightShoulderIndex];
				Vector3 left_shoulder = jointsPos [leftShoulderIndex];

				/*
				float threshold = 0.20f;
				bool rightHandInFrontOfElbow = Mathf.Abs(right_hand.x - right_elbow.x) < threshold && Mathf.Abs(right_hand.y - right_elbow.y) < threshold  && right_hand.z < right_elbow.z;
				bool leftHandInFrontOfElbow = Mathf.Abs(left_hand.x - left_elbow.x) < threshold && Mathf.Abs(left_hand.y - left_elbow.y) < threshold && left_hand.z < left_elbow.z;
				bool rightElbowUnderShoulder = Mathf.Abs(right_elbow.x - right_shoulder.x) < threshold && Mathf.Abs(right_elbow.z - right_shoulder.z) < threshold && right_elbow.y < right_shoulder.y;
				bool leftElbowUnderShoulder = Mathf.Abs(left_elbow.x - left_shoulder.x) < threshold && Mathf.Abs(left_elbow.z - left_shoulder.z) < threshold && left_elbow.y < left_shoulder.y;

				if(rightHandInFrontOfElbow && leftHandInFrontOfElbow && rightElbowUnderShoulder && leftElbowUnderShoulder) {
					sendMachineGunGesture();
					SetGestureCancelled (ref gestureData);
				}
				*/



				Vector3 vectorRightElbowHand = right_hand - right_elbow;
				Vector3 vectorRightElbowShoulder = right_shoulder - right_elbow;
				Vector3 vectorLeftElbowHand = left_hand - left_elbow;
				Vector3 vectorLeftElbowShoulder = left_shoulder - left_elbow;
				
				float right = Vector3.Angle (vectorRightElbowHand, vectorRightElbowShoulder);
				float left = Vector3.Angle (vectorLeftElbowHand, vectorLeftElbowShoulder);


				//Debug.Log("right: " + right + "left: " + left); 
				int threshold = 20;
				int mid = 90;
				if(right < mid+threshold && right > mid-threshold  && left < mid+threshold && left > mid-threshold){
					sendMachineGunGesture();
					SetGestureCancelled (ref gestureData);
				}



			}
			break;
		}
	}

	
	private static void sendLeanGesture (float angleLeftRight, float angleFrontBack) {
		foreach (GestureListener listener in listeners) {
			listener.leanGesture (angleLeftRight, angleFrontBack);
		}
	}
	
	private static void sendArmGesture (float angleLeft, float angleRight) {
		foreach (GestureListener listener in listeners) {
			listener.armGesture (angleLeft, angleRight);
		}
	}
	
	private static void sendPointGesture (float xMovement, float yMovement, bool select) {
		foreach (GestureListener listener in listeners) {
			listener.pointGesture (xMovement, yMovement, select);
		}
	}

	private static void sendMachineGunGesture () {
		foreach (GestureListener listener in listeners) {
			listener.machineGunGesture();
		}
	}
}
