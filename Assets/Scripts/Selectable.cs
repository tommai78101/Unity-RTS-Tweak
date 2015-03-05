using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {
	public bool isSelected = false;
	public Color initialColor = Color.white;
	public Color selectedColor;

	private bool isBoxedSelected = false;

	private void Update() {
		Renderer renderer = this.GetComponent<Renderer>();
		if (renderer.enabled && Input.GetMouseButton(0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(this.transform.position);
			camPos.y = Screen.height - camPos.y;
			isBoxedSelected = SelectionManager.selectionArea.Contains(camPos);
		}

		if (this.isBoxedSelected) {
			if (this.selectedColor == null) {
				renderer.material.color = Color.blue;
			}
			else {
				renderer.material.color = this.selectedColor;
			}
			if (Input.GetMouseButtonUp(0)) {
				this.isSelected = true;
			}
		}
		else {
			if (Input.GetMouseButtonDown(0)) {
				this.isSelected = false;
			}
			renderer.material.color = this.initialColor;
		}
	}
}