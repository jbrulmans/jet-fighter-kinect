using UnityEngine;
using System.Collections;
using System;

public class GestureInfo : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	// GUI Text to display the gesture messages.
	public GUIText text;
	
	// private bool to track if progress message has been displayed
	private bool progressDisplayed;
	
	public void UserDetected(uint userId, int userIndex) {
		DisplayText ("Detected a user");
	}
	
	public void UserLost(uint userId, int userIndex) {
		DisplayText (String.Empty);
	}

	public void GestureInProgress(uint userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectWrapper.SkeletonJoint joint, Vector3 screenPos)
	{
		if(progress > 0.3f) {
			DisplayText(string.Format ("{0} {1:F1}% complete", gesture, progress * 100));
			
			progressDisplayed = true;
		}		
	}

	public bool GestureCompleted (uint userId, int userIndex, KinectGestures.Gestures gesture, 
		KinectWrapper.SkeletonJoint joint, Vector3 screenPos)
	{
		DisplayText (gesture + " detected");
		progressDisplayed = false;
		
		return true;
	}

	public bool GestureCancelled (uint userId, int userIndex, KinectGestures.Gestures gesture, 
		KinectWrapper.SkeletonJoint joint)
	{
		if(progressDisplayed) {
			DisplayText (String.Empty);
			progressDisplayed = false;
		}
		
		return true;
	}

	private void DisplayText (String message) {
		if (text != null)
			text.guiText.text = message;
	}
}