using UnityEngine;
using System.Collections;

public class TutorialSelectable : MonoBehaviour {
	public Color selectionColor;
	public bool isSelected;

	private Color initialColor;

	private void Start() {
		Renderer renderer = this.GetComponent<Renderer>();
		this.initialColor = renderer.material.color;
	}

	public void SetSelect() {
		this.isSelected = true;
		Renderer renderer = this.GetComponent<Renderer>();
		renderer.material.color = this.selectionColor;
	}

	public void SetDeselect() {
		this.isSelected = false;
		Renderer renderer = this.GetComponent<Renderer>();
		renderer.material.color = this.initialColor;
	}
}
