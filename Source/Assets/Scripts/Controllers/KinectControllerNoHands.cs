using UnityEngine;
using System.Collections;

public class KinectControllerNoHands : MonoBehaviour, GestureListener {
	private Player player;

	void Start () {
		GestureDetector.addListener (this);
		player = this.GetComponent<Player> ();
	}
	
	void Update () {
	}

	public void leanGesture(float aLeftRight, float aFrontBack) {

	}

	
	public void armGesture(float angleLeft, float angleRight) {

	}

}
