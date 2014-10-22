﻿using UnityEngine;
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
	}

	private static void detectLeaning (ref KinectGestures.GestureData gestureData, float timestamp, 
		ref Vector3[] jointsPos, ref bool[] jointsTracked) {

		float arms_threshold = 0.15f;
		switch (gestureData.state) {
		case 0:  // gesture detection - phase 1
			if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked [leftElbowIndex] && jointsTracked [rightElbowIndex] &&
			    Mathf.Abs (jointsPos [rightHandIndex].y - jointsPos [rightElbowIndex].y) < arms_threshold && 
			    Mathf.Abs (jointsPos [leftHandIndex].y - jointsPos [leftElbowIndex].y) < arms_threshold) {
				SetGestureJoint (ref gestureData, timestamp, rightHandIndex, jointsPos [rightHandIndex]);
				gestureData.progress = 0.5f;
			}
			break;

		case 1:  // gesture phase 2 = tilting
			if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked [leftElbowIndex] && jointsTracked [rightElbowIndex]) {
				
				bool isInPose = !(Mathf.Abs(jointsPos [rightHandIndex].x - jointsPos [rightElbowIndex].x) < arms_threshold && 
				                  Mathf.Abs(jointsPos [leftHandIndex].x - jointsPos [leftElbowIndex].x) < arms_threshold);
				
				//if we're still in the pose, detect the angle
				if (isInPose) {
					if (jointsTracked [rightShoulderIndex] && jointsTracked [leftShoulderIndex]) { 
						Vector3 mid_shoulders = jointsPos [shoulderCenterIndex];
						Vector3 mid_hips = jointsPos [hipCenterIndex];
						Vector3 ground_far_x = mid_hips;
						ground_far_x.Set ((mid_hips.x + 20), mid_hips.y, mid_hips.z);
						
						Vector3 ground_far_z = mid_hips;
						ground_far_z.Set (mid_hips.x, mid_hips.y, (mid_hips.z + 20));
						
						Vector3 vectorLeftRight1 = ground_far_x - mid_hips;
						Vector3 vectorLeftRight2 = mid_shoulders - mid_hips;
						float aLeftRight = Vector3.Angle (vectorLeftRight1, vectorLeftRight2);
						
						Vector3 vectorFrontBack1 = ground_far_z - mid_hips;
						Vector3 vectorFrontBack2 = mid_shoulders - mid_hips;
						
						float aFrontBack = Vector3.Angle (vectorFrontBack1, vectorFrontBack2);

						sendLeanGesture (aLeftRight, aFrontBack);
					}
				} else {
					Debug.Log ("tilting cancelled");
					SetGestureCancelled (ref gestureData);
				}
			}
			break;
		}
	}

	private static void detectArm (ref KinectGestures.GestureData gestureData, float timestamp, 
	                                   ref Vector3[] jointsPos, ref bool[] jointsTracked) {

		switch (gestureData.state) {
		case 0: 
			if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked [leftElbowIndex] && jointsTracked [rightElbowIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex]) {
				
				Vector3 mid_hips = jointsPos [hipCenterIndex];
				Vector3 mid_shoulders = jointsPos [shoulderCenterIndex];
				Vector3 right_hand = jointsPos [rightHandIndex];
				Vector3 left_hand = jointsPos [leftHandIndex];
				
				Vector3 vectorRight1 = right_hand - mid_shoulders;
				Vector3 vectorRight2 = mid_hips - mid_shoulders;

				Vector3 vectorLeft1 = left_hand - mid_shoulders;
				Vector3 vectorleft2 = mid_hips - mid_shoulders;

				float angleRight = Vector3.Angle (vectorRight1, vectorRight2);
				float angleLeft = Vector3.Angle (vectorLeft1, vectorleft2);
				
				SetGestureJoint (ref gestureData, timestamp, rightHandIndex, jointsPos [rightHandIndex]);
				gestureData.progress = 0.5f;
				
				sendArmGesture (angleLeft, angleRight);
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
			listener.leanGesture (angleLeft, angleRight);
		}
	}
}
