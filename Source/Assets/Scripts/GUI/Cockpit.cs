using UnityEngine;
using System.Collections;

public class Cockpit : MonoBehaviour {
	public Texture texture;

	private int textureWidth, textureHeight;

	// Use this for initialization
	void Start () {
		textureWidth = texture.width;
		textureHeight = texture.height;
	}

	void OnGUI () {
		// Calculate height
		float ratio = (float) textureHeight / (float) textureWidth;
		float height = (float)Screen.width * ratio;

		// Calculate position
		float y = Screen.height - height;

		// Draw texture
		GUI.DrawTexture (new Rect (0, y, Screen.width, height), texture);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
