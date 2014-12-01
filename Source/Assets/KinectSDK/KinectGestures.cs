using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectGestures
{
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
			LEANING,
			ARM,
			MACHINEGUN,
			POINTING,
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
		// estimate the next state and completeness of the gesture
		public static void CheckForGesture (uint userId, ref GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked)
		{
			GestureDetector.CheckForGesture(userId, ref gestureData, timestamp, ref jointsPos, ref jointsTracked);
		}
}
