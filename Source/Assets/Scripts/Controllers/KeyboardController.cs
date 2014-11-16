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

		// TODO: Remove later (usefull for debugging)
		/*if (Input.GetKey(KeyCode.Keypad0))
			player.selectTarget (1f, 0f);
		if (Input.GetKey(KeyCode.Keypad1))
			player.selectTarget (-1f, -1f);
		if (Input.GetKey(KeyCode.Keypad2))
			player.stopSelectingTargets();*/
	}
}