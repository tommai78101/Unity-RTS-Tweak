﻿#if UNITY_EDITOR
	using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class CameraPanning : MonoBehaviour {

	public float distanceFromScreenBorders;
	public bool useDebugSceneCamBorder;
	public float camSpeed;
	public bool mouseInFocus;

	// Use this for initialization
	void Start () {
		this.mouseInFocus = false;
	}

	public void OnApplicationFocus(bool focus) {
		this.mouseInFocus = focus;
	}

	// Update is called once per frame
	void Update () {
		if (!this.mouseInFocus) {
			return;
		}
		
		//Debug.Log ("Mouse Pos: " + Input.mousePosition.x + "  " + Input.mousePosition.y);
		//Debug.Log ("Screen size: " + Screen.currentResolution.width + " " + Screen.currentResolution.height);
		//Left 25
		//Bottom 25
		//Current Screen size: 1920x1080	




		//  Screen.SetResolution(640, 480, true);

		/*
		Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions) {
            print(res.width + "x" + res.height);
        }
        Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
		 */


		float aspectRatio = Screen.currentResolution.width / Screen.currentResolution.height;
		float distance = distanceFromScreenBorders * aspectRatio;
		Vector2 mousePos = Input.mousePosition;
#if UNITY_EDITOR
		Vector2 screen = useDebugSceneCamBorder ? Handles.GetMainGameViewSize () : new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
#else
		Vector2 screen = new Vector2(Screen.width, Screen.height);
#endif

		

		if (mousePos.x > 0 && mousePos.x < distance){
			//Moving left
			Vector3 camPos = this.transform.position;
			camPos.x -= camSpeed;
			this.transform.position = camPos;
		}
		if (mousePos.y > 0 && mousePos.y < distance){
			//Moving backward
			Vector3 camPos = this.transform.position;
			camPos.z -= camSpeed;
			this.transform.position = camPos;
		}
		if (mousePos.x > (screen.x - distance) && mousePos.x <= screen.x){
			//Moving right
			Vector3 camPos = this.transform.position;
			camPos.x += camSpeed;
			this.transform.position = camPos;
		}
		if (mousePos.y > (screen.y - distance) && mousePos.y <= screen.y){
			//Moving forward
			Vector3 camPos = this.transform.position;
			camPos.z += camSpeed;
			this.transform.position = camPos;
		}
	}
}
