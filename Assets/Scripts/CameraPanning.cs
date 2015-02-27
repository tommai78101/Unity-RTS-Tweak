using UnityEngine;
using UnityEditor;
using System.Collections;

public class CameraPanning : MonoBehaviour {

	public float distanceFromScreenBorders;
	public bool useDebugSceneCamBorder;
	public float camSpeed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
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
		Vector2 screen = useDebugSceneCamBorder ? Handles.GetMainGameViewSize () : new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

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
