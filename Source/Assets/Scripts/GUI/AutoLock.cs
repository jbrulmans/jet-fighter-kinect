using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoLock : MonoBehaviour {
	public Player player;
	public Texture[] texturesSelected, texturesSelected2, texturesSelected3, texturesNotSelected;

	void OnGUI () {
		List<Enemy> targets = player.getVisibleTargets ();

		if (player.objectiveEnemy != null
		    && targets.Contains(player.objectiveEnemy)
		    && player.getTarget () != player.objectiveEnemy)
			drawLockIcon (texturesSelected3, player.objectiveEnemy);

		foreach (Enemy enemy in targets) {
			if (enemy == player.objectiveEnemy && enemy != player.getTarget())
				continue;

			Texture[] textures = texturesNotSelected;
			if (enemy == player.getTarget ()) {
				if (player.getTarget () && player.targetIsLocked ())
					textures = texturesSelected;
				else {
					textures = texturesSelected2; //2
				}
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
		// Get 8 points of collision box
		Bounds bounds = enemy.collider.bounds;
		Vector3[] points = new Vector3[] {
			bounds.min, bounds.max,
			new Vector3 (bounds.min.x, bounds.min.y, bounds.max.z),
			new Vector3 (bounds.min.x, bounds.max.y, bounds.min.z),
			new Vector3 (bounds.max.x, bounds.min.y, bounds.min.z),
			new Vector3 (bounds.min.x, bounds.max.y, bounds.max.z),
			new Vector3 (bounds.max.x, bounds.min.y, bounds.max.z),
			new Vector3 (bounds.max.x, bounds.max.y, bounds.min.z)
		}; 

		// Calculate top-left and, bottom-right corners (in screen coordinates)
		Vector3 max = new Vector3 (float.MinValue, float.MinValue);
		Vector3 min = new Vector3 (float.MaxValue, float.MaxValue);
		for (int i=0; i<points.Length; i++) {
			Vector3 p = Camera.main.WorldToScreenPoint (points[i]);
			max.x = Mathf.Max (p.x, max.x);
			max.y = Mathf.Max (p.y, max.y);
			min.x = Mathf.Min (p.x, min.x);
			min.y = Mathf.Min (p.y, min.y);
		}

		// Calculate size of icon
		float height = max.y - min.y;
		float width = max.x - min.x;
		float s = Mathf.Min (height, width);
		s = s * (2f / 3f);
		s = Mathf.Min (20, s);

		GUI.DrawTexture (new Rect (min.x, Screen.height-max.y, s, s), textures[0]);
		GUI.DrawTexture (new Rect (min.x, Screen.height-min.y-s, s, s), textures[1]);
		GUI.DrawTexture (new Rect (max.x-s, Screen.height-max.y, s, s), textures[2]);
		GUI.DrawTexture (new Rect (max.x-s, Screen.height-min.y-s, s, s), textures[3]);
	}
}
