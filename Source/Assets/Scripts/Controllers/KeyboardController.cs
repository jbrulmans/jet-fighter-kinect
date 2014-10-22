using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {
	private Player player;
	
	void Start () {
		player = this.GetComponent<Player> ();
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