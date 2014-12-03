using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointingObjective : MonoBehaviour {
	public Player player;

	private bool ready = false;
	private List<Enemy> objectives = new List<Enemy>();
	
	void Update () {
		if (!ready && player.areEnemiesReady ()) {
			getObjectives ();
			ready = true;
		}

		if (ready) {
			if (Input.GetKeyUp (KeyCode.KeypadEnter))
				selectNextObjective ();
		}
	}

	private void getObjectives () {
		List<Enemy> enemies = player.getVisibleTargets ();
	 	int size = Mathf.Min (3, enemies.Count);

		for (int i=0; i<size; i++) {
			bool found = false;

			while (!found) {
				Enemy e = enemies[Random.Range (0, enemies.Count)];
				found = true;

				if (objectives.Contains (e))
					found = false;
				else
					objectives.Add (e);
			}
		}

		foreach (Enemy e in objectives)
			Debug.Log(e);

		selectNextObjective ();
	}

	private void selectNextObjective () {
		if (objectives.Count > 0) {
			player.objectiveEnemy = objectives[0];
			objectives.Remove (player.objectiveEnemy);

		} else
			player.objectiveEnemy = null;
	}
}
