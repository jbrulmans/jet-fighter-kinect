using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {
	public Player player;
	
	void Start () {

	}

	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");
		player.moveSideways (horizontal);

		float vertical = Input.GetAxis ("Vertical");
		player.moveUpOrDown (vertical);

		if (Input.GetKey ("space")) {
			//player.reverse ();
			player.fireMachineGun ();
		}
	}
}