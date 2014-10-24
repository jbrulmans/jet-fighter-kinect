using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoLock : MonoBehaviour {
	public Player player;
	public Texture notSelected, selected;

	void OnGUI () {
		List<Enemy> targets = player.getVisibleTargets ();

		foreach (Enemy enemy in targets) {
			Texture texture = enemy == player.getTarget () ? selected : notSelected;

			Vector3 pos = Camera.main.WorldToScreenPoint (enemy.transform.position);
			GUI.DrawTexture (new Rect (pos.x-15, Screen.height-pos.y-15, 30, 30), texture);
		}
	}
}
