using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	public Texture2D crosshair;

	private Rect crosshairPos;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;

		crosshairPos = new Rect (
			(Screen.width - crosshair.width) / 2,
			(Screen.height - crosshair.height) / 2,
			crosshair.width,
			crosshair.height
		);
	}

	void Update () {
		
	}

	void OnGUI () {
		GUI.DrawTexture (crosshairPos, crosshair);
	}
}
