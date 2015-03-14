using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	public static List<Selectable> selectedObjects = new List<Selectable>();
	public bool isSelected = false;
	public Color initialColor = Color.white;
	public Color selectedColor;

	private bool isBoxedSelected = false;

	private void Update() {
		Renderer renderer = this.GetComponentInChildren<Renderer>();
		if (renderer.enabled && Input.GetMouseButton(0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(this.transform.position);
			camPos.y = Screen.height - camPos.y;
			isBoxedSelected = Selection.selectionArea.Contains(camPos);
		}

		if (this.isBoxedSelected) {
			if (this.selectedColor.Equals(null)) {
				Debug.Log("[Selectable] Color is null.");
				return;
			}
			renderer.material.color = this.selectedColor;
			if (Input.GetMouseButtonUp(0)) {
				this.isSelected = true;
				if (!Selectable.selectedObjects.Contains(this)) {
					Selectable.selectedObjects.Add(this);
				}
			}
		}
		else {
			if (Input.GetMouseButtonUp(0)) {
				this.isSelected = false;
			}
			renderer.material.color = this.initialColor;
		}

		while (!this.isSelected && Selectable.selectedObjects.Contains(this)) {
			Selectable.selectedObjects.Remove(this);
		}
	}
}