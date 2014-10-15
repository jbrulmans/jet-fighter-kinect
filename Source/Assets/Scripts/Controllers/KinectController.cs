using UnityEngine;
using System.Collections;

public class KinectController : MonoBehaviour {
	public Player player;
	
	void Start () {
		
	}
	
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");
		player.moveSideways (horizontal);
		
		float vertical = Input.GetAxis ("Vertical");
		player.moveUpOrDown (vertical);
	}
}
