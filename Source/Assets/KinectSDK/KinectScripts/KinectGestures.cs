using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectGestures
{
	static float minHoek = 360;
	static float maxHoek = 0;
		public interface GestureListenerInterface
		{
				// Invoked when a new user is detected and tracking starts
				// Here you can start gesture detection with KinectManager.DetectGesture()
				void UserDetected (uint userId, int userIndex);
		
				// Invoked when a user is lost
				// Gestures for this user are cleared automatically, but you can free the used resources
				void UserLost (uint userId, int userIndex);
		
				// Invoked when a gesture is in progress 
				void GestureInProgress (uint userId, int userIndex, Gestures gesture, float progress, 
			KinectWrapper.SkeletonJoint joint, Vector3 screenPos);

				// Invoked if a gesture is completed.
				// Returns true, if the gesture detection must be restarted, false otherwise
				bool GestureCompleted (uint userId, int userIndex, Gestures gesture,
			KinectWrapper.SkeletonJoint joint, Vector3 screenPos);

				// Invoked if a gesture is cancelled.
				// Returns true, if the gesture detection must be retarted, false otherwise
				bool GestureCancelled (uint userId, int userIndex, Gestures gesture, 
			KinectWrapper.SkeletonJoint joint);
		}
	
	
		public enum Gestures
		{
				None = 0,
				Tilt
		}
	
	
		public struct GestureData
		{
				public uint userId;
				public Gestures gesture;
				public int state;
				public float timestamp;
				public int joint;
				public Vector3 jointPos;
				public Vector3 screenPos;
				public float tagFloat;
				public Vector3 tagVector;
				public Vector3 tagVector2;
				public float progress;
				public bool complete;
				public bool cancelled;
				public List<Gestures> checkForGestures;
				public float startTrackingAtTime;
		}
	

	
		// Gesture related constants, variables and functions
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
	
	
		private static void SetGestureJoint (ref GestureData gestureData, float timestamp, int joint, Vector3 jointPos)
		{
				gestureData.joint = joint;
				gestureData.jointPos = jointPos;
				gestureData.timestamp = timestamp;
				gestureData.state++;
		}
	
		private static void SetGestureCancelled (ref GestureData gestureData)
		{
				gestureData.state = 0;
				gestureData.progress = 0f;
				gestureData.cancelled = true;
		}
	
		private static void CheckPoseComplete (ref GestureData gestureData, float timestamp, Vector3 jointPos, bool isInPose, float durationToComplete)
		{
				if (isInPose) {
						float timeLeft = timestamp - gestureData.timestamp;
						gestureData.progress = durationToComplete > 0f ? Mathf.Clamp01 (timeLeft / durationToComplete) : 1.0f;
	
						if (timeLeft >= durationToComplete) {
								gestureData.timestamp = timestamp;
								gestureData.jointPos = jointPos;
								gestureData.state++;
								gestureData.complete = true;
								Debug.Log (gestureData.gesture + "NOG IETS BIJTYPEN WAT IK WIL");
						}
				} else {
						SetGestureCancelled (ref gestureData);
				}
		}
	
		private static void SetScreenPos (uint userId, ref GestureData gestureData, ref Vector3[] jointsPos, ref bool[] jointsTracked)
		{
				Vector3 handPos = jointsPos [rightHandIndex];
//		Vector3 elbowPos = jointsPos[rightElbowIndex];
//		Vector3 shoulderPos = jointsPos[rightShoulderIndex];
				bool calculateCoords = false;
		
				if (gestureData.joint == rightHandIndex) {
						if (jointsTracked [rightHandIndex] /**&& jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex]*/) {
								calculateCoords = true;
						}
				} else if (gestureData.joint == leftHandIndex) {
						if (jointsTracked [leftHandIndex] /**&& jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex]*/) {
								handPos = jointsPos [leftHandIndex];
//				elbowPos = jointsPos[leftElbowIndex];
//				shoulderPos = jointsPos[leftShoulderIndex];
				
								calculateCoords = true;
						}
				}
		
				if (calculateCoords) {
//			if(gestureData.tagFloat == 0f || gestureData.userId != userId)
//			{
//				// get length from shoulder to hand (screen range)
//				Vector3 shoulderToElbow = elbowPos - shoulderPos;
//				Vector3 elbowToHand = handPos - elbowPos;
//				gestureData.tagFloat = (shoulderToElbow.magnitude + elbowToHand.magnitude);
//			}
			
						if (jointsTracked [hipCenterIndex] && jointsTracked [shoulderCenterIndex] && 
								jointsTracked [leftShoulderIndex] && jointsTracked [rightShoulderIndex]) {
								Vector3 neckToHips = jointsPos [shoulderCenterIndex] - jointsPos [hipCenterIndex];
								Vector3 rightToLeft = jointsPos [rightShoulderIndex] - jointsPos [leftShoulderIndex];
				
								gestureData.tagVector2.x = rightToLeft.x; // * 1.2f;
								gestureData.tagVector2.y = neckToHips.y; // * 1.2f;
				
								if (gestureData.joint == rightHandIndex) {
										gestureData.tagVector.x = jointsPos [rightShoulderIndex].x - gestureData.tagVector2.x / 2;
										gestureData.tagVector.y = jointsPos [hipCenterIndex].y;
								} else {
										gestureData.tagVector.x = jointsPos [leftShoulderIndex].x - gestureData.tagVector2.x / 2;
										gestureData.tagVector.y = jointsPos [hipCenterIndex].y;
								}
						}
	
//			Vector3 shoulderToHand = handPos - shoulderPos;
//			gestureData.screenPos.x = Mathf.Clamp01((gestureData.tagFloat / 2 + shoulderToHand.x) / gestureData.tagFloat);
//			gestureData.screenPos.y = Mathf.Clamp01((gestureData.tagFloat / 2 + shoulderToHand.y) / gestureData.tagFloat);
			
						if (gestureData.tagVector2.x != 0 && gestureData.tagVector2.y != 0) {
								Vector3 relHandPos = handPos - gestureData.tagVector;
								gestureData.screenPos.x = Mathf.Clamp01 (relHandPos.x / gestureData.tagVector2.x);
								gestureData.screenPos.y = Mathf.Clamp01 (relHandPos.y / gestureData.tagVector2.y);
						}
			
						//Debug.Log(string.Format("{0} - S: {1}, H: {2}, SH: {3}, L : {4}", gestureData.gesture, shoulderPos, handPos, shoulderToHand, gestureData.tagFloat));
				}
		}
	
		private static void SetZoomFactor (uint userId, ref GestureData gestureData, float initialZoom, ref Vector3[] jointsPos, ref bool[] jointsTracked)
		{
				Vector3 vectorZooming = jointsPos [rightHandIndex] - jointsPos [leftHandIndex];
		
				if (gestureData.tagFloat == 0f || gestureData.userId != userId) {
						gestureData.tagFloat = 0.5f; // this is 100%
				}

				float distZooming = vectorZooming.magnitude;
				gestureData.screenPos.z = initialZoom + (distZooming / gestureData.tagFloat);
		}
	
		private static void SetWheelRotation (uint userId, ref GestureData gestureData, Vector3 initialPos, Vector3 currentPos)
		{
				float angle = Vector3.Angle (initialPos, currentPos) * Mathf.Sign (currentPos.y - initialPos.y);
				gestureData.screenPos.z = angle;
		}
	
		// estimate the next state and completeness of the gesture
		public static void CheckForGesture (uint userId, ref GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked, Player player)
		{
				if (gestureData.complete)
						return;
		
				switch (gestureData.gesture) {
				// check for Tilt
				case Gestures.Tilt:
						float arms_threshold = 0.1f;
						switch (gestureData.state) {
						case 0:  // gesture detection - phase 1
								if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked [leftElbowIndex] && jointsTracked [rightElbowIndex] &&
										Mathf.Abs (jointsPos [rightHandIndex].y - jointsPos [rightElbowIndex].y) < arms_threshold && 
										Mathf.Abs (jointsPos [leftHandIndex].y - jointsPos [leftElbowIndex].y) < arms_threshold) {
										SetGestureJoint (ref gestureData, timestamp, rightHandIndex, jointsPos [rightHandIndex]);
										gestureData.progress = 0.5f;
										Debug.Log ("CASE 1");
								}
								break;
						case 1:  // gesture phase 2 = tilting
								if (jointsTracked [rightHandIndex] && jointsTracked [leftHandIndex] && jointsTracked [leftElbowIndex] && jointsTracked [rightElbowIndex]) {
										Debug.Log ("CASE 2");
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

														if (aFrontBack < minHoek)
															minHoek = aLeftRight;
														if (aFrontBack > maxHoek)
															maxHoek = aLeftRight;
														


														if (aLeftRight < 86)
															if (aLeftRight <= 55)
																	player.moveSideways(0.99f);
															else {
																player.moveSideways(1 - (aLeftRight - 55)/31);
															}
														else if (aLeftRight > 94) {
															if (aLeftRight >= 125)
																	player.moveSideways(-0.99f);
															else {
																player.moveSideways(-1 * ((aLeftRight-94)/31.0f));
															}
														}

														if (aFrontBack < 85)
																if (aFrontBack <= 70)
																	player.moveUpOrDown (0.99f);
																else {
																	player.moveUpOrDown(1 - (aFrontBack - 70)/15);
																}
														else if (aFrontBack > 95) {
																if (aFrontBack >= 115)
																	player.moveUpOrDown(-0.99f);
																else {
																	player.moveUpOrDown(-1 * ((aFrontBack-95)/20.0f));
																}
														}

														Debug.Log ("min: " + minHoek + ", max: " + maxHoek);
														//Debug.Log ("tilting :D " + aLeftRight.ToString ());
												}
										} else {
												Debug.Log ("tilting cancelled");
												SetGestureCancelled (ref gestureData);
										}
								}
								break;
						}
						break;
				// here come more gesture-cases
				}
		}

}
