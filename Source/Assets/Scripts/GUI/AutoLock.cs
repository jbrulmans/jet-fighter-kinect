using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoLock : MonoBehaviour {
	public Player player;
	public Texture[] texturesSelected, texturesSelected2, texturesNotSelected;

	void OnGUI () {
		List<Enemy> targets = player.getVisibleTargets ();

		// TODO: Remove later (usefull for debugging)
		/*if (player.test != null)
			GUI.DrawTexture (
				new Rect (player.test.x, Screen.height-player.test.y, 10, 10), 
				texturesSelected[0]);*/

		foreach (Enemy enemy in targets) {
			Texture[] textures = texturesNotSelected;
			if (enemy == player.getTarget ()) {
				if (player.getTarget () && player.targetIsLocked ())
					textures = texturesSelected;
				else
					textures = texturesSelected2;
			}

			// Draw textures
			drawLockIcon (textures, enemy);

			// Draw texture
			//Texture texture = enemy == player.getTarget () ? selected : notSelected;
			/*Vector3 pos = Camera.main.WorldToScreenPoint (enemy.transform.position);
			GUI.DrawTexture (new Rect (pos.x-15, Screen.height-pos.y-15, 30, 30), texture);*/
		}
	}

	void drawLockIcon (Texture[] textures, Enemy enemy) {
		Renderer renderer = enemy.GetComponentInChildren<Renderer> ();
		Vector3 max = Camera.main.WorldToScreenPoint (enemy.collider.bounds.max);
		Vector3 min = Camera.main.WorldToScreenPoint (enemy.collider.bounds.min);
		
		if (max.x < min.x) { float temp = max.x; max.x = min.x; min.x = temp;}
		if (max.y < min.y) { float temp = max.y; max.y = min.y; min.y = temp;}
		
		GUI.DrawTexture (new Rect (min.x, Screen.height-max.y, 20, 20), textures[0]);
		GUI.DrawTexture (new Rect (min.x, Screen.height-min.y-20, 20, 20), textures[1]);
		GUI.DrawTexture (new Rect (max.x-20, Screen.height-max.y, 20, 20), textures[2]);
		GUI.DrawTexture (new Rect (max.x-20, Screen.height-min.y-20, 20, 20), textures[3]);
	}
}
