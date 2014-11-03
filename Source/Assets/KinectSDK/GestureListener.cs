using UnityEngine;
using System.Collections;

public interface GestureListener {
	void leanGesture(float angleLeftRight, float angleFrontBack);
	void armGesture(float angleLeft, float angleRight);
	void pointGesture(float xMovement, float yMovement);
}
