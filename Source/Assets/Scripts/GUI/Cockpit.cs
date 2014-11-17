using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Code from: http://answers.unity3d.com/questions/11022/how-to-rotate-gui-textures.html (+ own modified code)
public class Cockpit : MonoBehaviour {
	public Texture2D texture;
	public Texture2D attitudeIndicator;
	public Texture2D headingIndicator;
	public Texture2D radar;
	
	private float width = 1.0f;
	private Color color = Color.black;

	private float textureWidth, textureHeight;


	public float angle = 0;
	public Player player = null;

	public Vector2 size = new Vector2(128, 128);
	private float attitudeX = 0.0f;
	private float attitudeY = 0.0f;
	private float attitudeHeight = 0.0f;
	public Vector2 headingSize = new Vector2(128, 128);
	private float headingX = 0.0f;
	private float headingY = 0.0f;
	private float headingHeight = 0.0f;
	public Vector2 radarSize = new Vector2(128, 128);
	private float radarX = 0.0f;
	private float radarY = 0.0f;
	private float radarHeight = 0.0f;
	
	//this will overwrite the items position
	public AlignmentScreenpoint ScreenpointToAlign = AlignmentScreenpoint.TopLeft;
	private Vector2 relativePosition = new Vector2(0, 0);
	private Vector2 headingRelativePosition = new Vector2(0, 0);
	private Vector2 radarRelativePosition = new Vector2(0, 0);
	
	Vector2 pos = new Vector2(0, 0);
	Vector2 headingPos = new Vector2(0, 0);
	Vector2 radarPos = new Vector2(0, 0);

	Rect rect;
	Rect headingRect;
	Rect radarRect;
	Vector2 pivot;
	Vector2 headingPivot;
	Vector2 radarPivot;


	void Start () {
		textureWidth = texture.width;
		textureHeight = texture.height;
		// Calculate height 685 * 600
		attitudeX = ((float)textureWidth * (0.379f)) * ((float)(Screen.width)/(float)(textureWidth));
		attitudeY = (textureHeight * -0.33f) * ((float)(Screen.width)/(float)(textureWidth));
		float attitudeWidth = (attitudeIndicator.width * 1.46f) * ((float)(Screen.width)/(float)(textureWidth));
		attitudeHeight = attitudeIndicator.height * 0.2f * ((float)(Screen.width)/(float)(textureWidth));

		headingX = ((float)textureWidth * (0.5026f)) * ((float)(Screen.width)/(float)(textureWidth));
		headingY = (textureHeight * -0.395f) * ((float)(Screen.width)/(float)(textureWidth));
		float headingWidth = (headingIndicator.width * 0.20f) * ((float)(Screen.width)/(float)(textureWidth));
		headingHeight = headingIndicator.height * 0.20f * ((float)(Screen.width)/(float)(textureWidth));

		radarX = ((float)textureWidth * (0.623f)) * ((float)(Screen.width)/(float)(textureWidth));
		radarY = Screen.height - (textureHeight * 0.356f) * ((float)(Screen.width)/(float)(textureWidth));
		float radarWidth = (radar.width * 0.30f) * ((float)(Screen.width)/(float)(textureWidth));
		radarHeight = radar.height * 0.30f * ((float)(Screen.width)/(float)(textureWidth));
		
		size = new Vector2 (attitudeWidth, attitudeHeight);
		relativePosition = new Vector2(attitudeX, attitudeY);
		headingSize = new Vector2 (headingWidth, headingHeight);
		headingRelativePosition = new Vector2(headingX, headingY);
		radarSize = new Vector2 (radarWidth, radarHeight);
		radarRelativePosition = new Vector2(radarX, radarY);
		UpdateSettings();
	}

	/// <summary>
	/// Updates the settings.
	/// </summary>
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

		headingPos = cornerPos + headingRelativePosition;
		headingRect = new Rect(headingPos.x - headingSize.x * 0.5f, headingPos.y - headingSize.y * 0.5f, headingSize.x, headingSize.y);
		headingPivot = new Vector2(headingRect.xMin + headingRect.width * 0.5f, headingRect.yMin + headingRect.height * 0.5f);

		radarPos = cornerPos + radarRelativePosition;
		radarRect = new Rect(radarPos.x - radarSize.x * 0.5f, radarPos.y - radarSize.y * 0.5f, radarSize.x, radarSize.y);
		radarPivot = new Vector2(radarRect.xMin + radarRect.width * 0.5f, radarRect.yMin + radarRect.height * 0.5f);
	}
	          
	//Part of code based on: http://www.dastardlybanana.com/radarPage.htm
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


		GUI.DrawTextureWithTexCoords(rect, attitudeIndicator, new Rect (0.0f, 0.4f + 0.1f*getAttitudeYOffset(), 1.0f, 0.20f), true);
		GUI.matrix = matrixBackup;

	
		// Draw texture
		//GUI.DrawTexture (new Rect (attitudeX, attitudeY, attitudeWidth, attitudeHeight), attitudeIndicator);
		GUI.DrawTexture (new Rect (0, y, Screen.width, height), texture);

		List<Enemy> enemies = player.getEnemies ();

		foreach (Enemy e in enemies) {
			float enemyX = e.transform.position.x;
			float enemyZ = e.transform.position.z;

			float playerX = player.transform.position.x;
			float playerZ = player.transform.position.z;

			Vector3 mousePos = Input.mousePosition;
			Vector3 enemyDirection = new Vector3 (enemyX - playerX, enemyZ - playerZ, 0.0f);

			Vector3 enemyVec = new Vector3 (e.transform.position.x, e.transform.position.y, e.transform.position.z);
			Vector3 playerVec = player.transform.position;

			float dist = Vector3.Distance (enemyVec, playerVec);

			float enemyDirectionAngle = 0.0f;

			enemyDirectionAngle = Mathf.Atan2 ((enemyX - playerX), (enemyZ - playerZ)) * Mathf.Rad2Deg - 90.0f - player.transform.eulerAngles.y;
			enemyDirection.x = (float)Math.Cos ((double)enemyDirectionAngle * Mathf.Deg2Rad);
			enemyDirection.y = (float)Math.Sin ((double)enemyDirectionAngle * Mathf.Deg2Rad);

			Debug.Log(Screen.width);
			//For width 823 is radarMarkDistFromCenter best at 37.0f
			float radarMarkDistFromCenter = (38.0f/823.0f) * (float)Screen.width;
			float radarOffset = 200.0f;
			if (dist >= radarOffset) {
				radarMarkDistFromCenter = (38.0f/823.0f) * (float)Screen.width;
			}
			else {
				radarMarkDistFromCenter = (dist/radarOffset) * (38.0f/823.0f) * (float)Screen.width;
			}

			Rect newRadarRect = new Rect (radarX + enemyDirection.x*radarMarkDistFromCenter, radarY + enemyDirection.y*radarMarkDistFromCenter, radarRect.width, radarRect.height);
			GUI.DrawTexture (newRadarRect, radar);
		}

		angle = 360.0f - player.transform.rotation.eulerAngles.y;
		GUIUtility.RotateAroundPivot(angle, headingPivot);
		GUI.DrawTexture (headingRect, headingIndicator);

	}

	/// <summary>
	/// Gets the attitude Y offset.
	/// </summary>
	/// <returns>The attitude Y offset.</returns>
	float getAttitudeYOffset() {
		float newYOffset = 0.0f;

		if (player.transform.rotation.eulerAngles.x < 90.0f)
			newYOffset = -1 * ((Screen.height/attitudeHeight)/2.0f*(player.transform.rotation.eulerAngles.x/90.0f));
		else if (player.transform.rotation.eulerAngles.x > 90.0f)
			newYOffset = ((Screen.height/attitudeHeight)/2.0f*(1-((player.transform.rotation.eulerAngles.x - 270.0f
			                                        )/90.0f)));

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
