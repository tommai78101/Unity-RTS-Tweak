#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using Extension;

public class CameraPanning : MonoBehaviour {

	public float distanceFromScreenBorders;
	public bool useDebugSceneCamBorder;
	public float camSpeed;
	public bool mouseInFocus;
	public float distanceFromEdge;
	public bool enableZoom;
	public int zoomLevel;

	// Use this for initialization
	void Start() {
		this.distanceFromEdge = 20f;
		this.zoomLevel = 3;
		this.mouseInFocus = true;
		this.camSpeed = 0.05f;
		this.enableZoom = true;

		SetCameraPosition(float.NaN, (float) this.zoomLevel, float.NaN);
	}

	public void OnApplicationFocus(bool focus) {
		this.mouseInFocus = focus;
	}

	// Update is called once per frame
	public virtual void Update() {
		if (!this.mouseInFocus) {
			return;
		}

		//Debug.Log ("Mouse Pos: " + Input.mousePosition.x + "  " + Input.mousePosition.y);
		//Debug.Log ("Screen size: " + Screen.currentResolution.width + " " + Screen.currentResolution.height);
		//Left 25
		//Bottom 25
		//Current Screen size: 1920x1080	

		CameraZoom();


		//  Screen.SetResolution(640, 480, true);

		/*
		Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions) {
            print(res.width + "x" + res.height);
        }
        Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
		 */


		float aspectRatio = (float) Screen.currentResolution.width / (float) Screen.currentResolution.height;
		this.distanceFromEdge = distanceFromScreenBorders * aspectRatio;
		Vector2 mousePos = Input.mousePosition;
#if UNITY_EDITOR
		this.useDebugSceneCamBorder = true;
		Vector2 screen = useDebugSceneCamBorder ? Handles.GetMainGameViewSize() : new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
#else
		Vector2 screen = new Vector2(Screen.width, Screen.height);
#endif



		if (mousePos.x > 0 && mousePos.x < this.distanceFromEdge) {
			//Moving left
			Vector3 camPos = this.transform.position;
			camPos.x -= camSpeed;
			this.transform.position = camPos;
		}
		if (mousePos.y > 0 && mousePos.y < this.distanceFromEdge) {
			//Moving backward
			Vector3 camPos = this.transform.position;
			camPos.z -= camSpeed;
			this.transform.position = camPos;
		}
		if (mousePos.x > (screen.x - this.distanceFromEdge) && mousePos.x <= screen.x) {
			//Moving right
			Vector3 camPos = this.transform.position;
			camPos.x += camSpeed;
			this.transform.position = camPos;
		}
		if (mousePos.y > (screen.y - this.distanceFromEdge) && mousePos.y <= screen.y) {
			//Moving forward
			Vector3 camPos = this.transform.position;
			camPos.z += camSpeed;
			this.transform.position = camPos;
		}
		//Debug.Log("Distance From Edges: " + this.distanceFromEdge.ToString() + " Mouse From Edges: {" + Input.mousePosition.x + ", " + (screen.y - Input.mousePosition.y) + ", " + (screen.x - Input.mousePosition.x) + ", " + Input.mousePosition.y + "}");
	}

	public void SetCameraPosition(float xDiff, float yDiff, float zDiff) {
		Vector3 camPos = Camera.main.transform.position;
		if (!float.IsNaN(xDiff)) {
			camPos.x = xDiff;
		}
		if (!float.IsNaN(yDiff)) {
			camPos.y = yDiff;
		}
		if (!float.IsNaN(zDiff)) {
			camPos.z = zDiff;
		}
		Camera.main.transform.position = camPos;
	}

	public void CameraZoom() {
		if (this.enableZoom) {
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (delta > 0f) {
				//Scroll up
				this.zoomLevel++;
				if (this.zoomLevel > 20) {
					this.zoomLevel = 20;
				}
				SetCameraPosition(float.NaN, (float) this.zoomLevel, float.NaN);
			}
			else if (delta < 0f) {
				//Scroll down
				this.zoomLevel--;
				if (this.zoomLevel < 3) {
					this.zoomLevel = 3;
				}
				SetCameraPosition(float.NaN, (float) this.zoomLevel, float.NaN);
			}
			this.camSpeed = 0.05f + (this.zoomLevel - 3) * 0.015f;
		}
	}
}
