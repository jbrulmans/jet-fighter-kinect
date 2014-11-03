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
		//if (aLeftRight % 2 == 0)
			player.setRotationZ_Axis (aLeftRight-90);
	}

	public void armGesture(float angleLeft, float angleRight) {
		;
	}

	public void pointGesture(float xMovement, float yMovement) {
		
	}
}