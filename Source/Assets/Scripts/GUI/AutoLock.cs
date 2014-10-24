using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoLock : MonoBehaviour {
	public Player player;
	public Texture[] texturesSelected, texturesNotSelected;

	void OnGUI () {
		List<Enemy> targets = player.getVisibleTargets ();

		foreach (Enemy enemy in targets) {
			Texture[] textures = enemy == player.getTarget () ? texturesSelected : texturesNotSelected;

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
		
		GUI.DrawTexture (new Rect (min.x, Screen.height-max.y, 10, 10), textures[0]);
		GUI.DrawTexture (new Rect (min.x, Screen.height-min.y, 10, 10), textures[1]);
		GUI.DrawTexture (new Rect (max.x, Screen.height-max.y, 10, 10), textures[2]);
		GUI.DrawTexture (new Rect (max.x, Screen.height-min.y, 10, 10), textures[3]);
	}
}
