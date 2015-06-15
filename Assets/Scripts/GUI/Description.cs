using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Description : MonoBehaviour {
	public Text text;

	public void Start() {
		//Assuming that "text" variable is already initialized.
		if (this.text == null) {
			Debug.LogException(new System.NullReferenceException("Text (UI) is not set in the inspector."));
		}
	}

	public void OnGUI() {
		if (Network.isClient || Network.isServer) {
			//Connected.
			if (Selectable.selectedObjects.Count > 0) {
				if (Input.GetKeyDown(KeyCode.A)) {
					text.text = "Right click anywhere to set a location to initiate attack.";
				}
				else if (Input.GetKeyUp(KeyCode.S)) {
					text.text = "Wait until your units have fully split itself.";
				}
				else if (Input.GetKeyUp(KeyCode.D)) {
					text.text = "Wait until your units have fully merged.";
				}
				else {
					text.text = "A: Attack. S: Split. D: Merge. Right Click: Move to Position.";
				}
			}
			else {
				text.text = "Click left mouse button to select a unit. Hold down the left mouse button to drag a selection box.";
			}
		}
		else {
			text.text = "Connect to a server, or host a server. Game requires an online connection.";
		}
	}
}

