using UnityEngine;
using System.Collections;

public class SelectState : MonoBehaviour {
	public bool isSelected;

	public void Awake() {
		if (EventManager.Instance != null) {
			EventManager.Instance.OnLeftClickUp += SelectState_OnLeftClickUp;
			EventManager.Instance.OnLeftClickDown += SelectState_OnLeftClickDown;
			EventManager.Instance.OnLeftClickHold += SelectState_OnLeftClickHold;
		}
	}

	public void OnDisable() {
		if (EventManager.Instance != null) {
			EventManager.Instance.OnLeftClickUp -= SelectState_OnLeftClickUp;
			EventManager.Instance.OnLeftClickDown -= SelectState_OnLeftClickDown;
			EventManager.Instance.OnLeftClickHold -= SelectState_OnLeftClickHold;
		}
	}

	public void SelectState_OnLeftClickUp(GameObject go) {
		if (go != null && go.Equals(this.gameObject)) {
			//Do action after releasing mouse button.
			Debug.Log("On click up");
			this.isSelected = true;
		}
	}

	public void SelectState_OnLeftClickDown(GameObject go) {
		if (go != null && go.Equals(this.gameObject)) {
			//Do action after clicking mouse button.
			Debug.Log("On click down");
			this.isSelected = true;
		}
		else {
			this.isSelected = false;
		}
	}

	public void SelectState_OnLeftClickHold(GameObject go) {
		if (go != null && go.Equals(this.gameObject)) {
			//Do action after holding mouse button.
			Debug.Log("On click hold");
			this.isSelected = true;
		}
	}

	public void OnGUI() {
		Vector3 camPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
		camPos.y = Screen.height - camPos.y;
		GUI.Label(new Rect(20f, 60f, 300f, 30f), camPos.ToString());
		GUI.Label(new Rect(20f, 90f, 300f, 30f), Selection.selectionArea.ToString());
	}

	public void Update() {
		Vector3 camPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
		camPos.y = Screen.height - camPos.y;
		bool flag = Selection.selectionArea.Contains(camPos);
		Renderer renderer = this.GetComponent<Renderer>();
		if (renderer != null) {
			if (this.isSelected || flag) {
				renderer.material.color = Color.blue;
			}
			else {
				renderer.material.color = Color.white;
			}
		}
	}
}
