﻿using UnityEngine;
using System.Collections;

//Code from: http://answers.unity3d.com/questions/11022/how-to-rotate-gui-textures.html (+ own modified code)
public class Cockpit : MonoBehaviour {
	public Texture texture;
	public Texture2D attitudeIndicator;

	private int textureWidth, textureHeight;


	public float angle = 0;
	public Vector2 size = new Vector2(128, 128);
	public Player player = null;
	private float attitudeX = 0.0f;
	private float attitudeY = 0.0f;
	private float attitudeHeight = 0.0f;
	
	//this will overwrite the items position
	public AlignmentScreenpoint ScreenpointToAlign = AlignmentScreenpoint.TopLeft;
	public Vector2 relativePosition = new Vector2(0, 0);
	
	Vector2 pos = new Vector2(0, 0);
	
	Rect rect;
	Vector2 pivot;

	// Use this for initialization
	void Start () {
		textureWidth = texture.width;
		textureHeight = texture.height;

		// Calculate height
		attitudeX = ((float)textureWidth * (0.379f)) * ((float)(Screen.width)/(float)(textureWidth));
		attitudeY = (textureHeight * -0.33f) * ((float)(Screen.width)/(float)(textureWidth));
		float attitudeWidth = (attitudeIndicator.width * 0.797f) * ((float)(Screen.width)/(float)(textureWidth));
		attitudeHeight = attitudeIndicator.height * 0.2f * ((float)(Screen.width)/(float)(textureWidth));
		
		size = new Vector2 (attitudeWidth, attitudeHeight);
		relativePosition = new Vector2(attitudeX, attitudeY);
		UpdateSettings();
	}

	void UpdateSettings()
	{
		Vector2 cornerPos = new Vector2(0, 0);
		
		//overwrite the items position
		switch (ScreenpointToAlign)
		{
		case AlignmentScreenpoint.TopLeft:
			cornerPos =new Vector2(0, 0);
			break;
		case AlignmentScreenpoint.TopMiddle:
			cornerPos =new Vector2(Screen.width/2, 0);
			break;
		case AlignmentScreenpoint.TopRight:
			cornerPos = new Vector2(Screen.width, 0);
			break;
		case AlignmentScreenpoint.LeftMiddle:
			cornerPos = new Vector2(0, Screen.height / 2);
			break;
		case AlignmentScreenpoint.RightMiddle:
			cornerPos = new Vector2(Screen.width, Screen.height / 2);
			break;
		case AlignmentScreenpoint.BottomLeft:
			cornerPos = new Vector2(0, Screen.height);
			break;
		case AlignmentScreenpoint.BottomMiddle:
			cornerPos = new Vector2(Screen.width/2, Screen.height);
			break;
		case AlignmentScreenpoint.BottomRight:
			cornerPos = new Vector2(Screen.width, Screen.height);
			break;
		default:
			break;
		}
		
		pos = cornerPos + relativePosition;
		rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
		pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
	}

	void OnGUI () {
		// Calculate position
		float ratio = (float) Screen.width / (float) textureWidth;
		float height = (float)textureHeight * ratio;
		float y = Screen.height - height;

		if (Application.isEditor)
		{
			UpdateSettings();
		}
		Matrix4x4 matrixBackup = GUI.matrix;
		angle = player.transform.rotation.eulerAngles.z;

		GUIUtility.RotateAroundPivot(angle, pivot);

		GUI.DrawTextureWithTexCoords(rect, attitudeIndicator, new Rect (0.0f, 0.4f + 0.01f*getAttitudeYOffset(), 1.0f, 0.20f), true);
		GUI.matrix = matrixBackup;
	
		// Draw texture
		//GUI.DrawTexture (new Rect (attitudeX, attitudeY, attitudeWidth, attitudeHeight), attitudeIndicator);
		GUI.DrawTexture (new Rect (0, y, Screen.width, height), texture);

	}

	float getAttitudeYOffset() {
		float newYOffset = 0.0f;

		if (player.transform.rotation.eulerAngles.x < 90.0f)
			newYOffset = -1 * (attitudeHeight/2.0f*(player.transform.rotation.eulerAngles.x/90.0f));
		else if (player.transform.rotation.eulerAngles.x > 90.0f)
			newYOffset = (attitudeHeight/2.0f*(1-((player.transform.rotation.eulerAngles.x - 270.0f)/90.0f)));

		return newYOffset;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public enum AlignmentScreenpoint
	{
		TopLeft, TopMiddle, TopRight,
		LeftMiddle, RightMiddle,
		BottomLeft, BottomMiddle, BottomRight
	}
}
