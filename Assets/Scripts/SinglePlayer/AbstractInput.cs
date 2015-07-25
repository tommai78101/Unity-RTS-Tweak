using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbstractInput : Selection {
	//Selection box
	//public Rect selectionArea = new Rect();
	//public Material borderMaterial; //OpenGL shader material

	//Input
	//Mouse
	public bool leftMouseButtonActive;
	public bool rightMouseButtonActive;

	//Keyboard
	public bool AttackKeyActive;
	public bool SplitKeyActive;
	public bool MergeKeyActive;

	private void Start() {
		this.initialClick = this.startingVertex = this.endingVertex = -Vector3.one;
		this.borderColor = new Color(1f, 128f / 255f, 1f, 1f); //Dark green
	}

	public override void Update() {
		HandleLeftMouseButton();
		HandleRightMouseButton();

		//Pressed
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A)) {
			this.AttackKeyActive = true;
		}
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S)) {
			this.SplitKeyActive = true;
		}
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D)) {
			this.MergeKeyActive = true;
		}

		//Released
		if (Input.GetKeyUp(KeyCode.A)){
			this.AttackKeyActive = false;
		}
		if (Input.GetKeyUp(KeyCode.S)){
			this.SplitKeyActive = false;
		}
		if (Input.GetKeyUp(KeyCode.D)){
			this.MergeKeyActive = false;
		}

		this.startingVertex = getStartingVertex();
		this.endingVertex = getEndingVertex();
	}

	private void HandleRightMouseButton() {
		if (Input.GetMouseButtonDown(1)) {
			this.rightMouseButtonActive = true;
		}
		if (Input.GetMouseButtonUp(1)) {
			this.rightMouseButtonActive = false;
		}
	}

	private void HandleLeftMouseButton() {
		if (Input.GetMouseButtonDown(0)) {
			this.leftMouseButtonActive = true;
			this.initialClick = Input.mousePosition;
		}
		if (Input.GetMouseButton(0)) {
			this.leftMouseButtonActive = true;
			float newX = Input.mousePosition.x - this.initialClick.x;
			float newY = (Screen.height - Input.mousePosition.y) - (Screen.height - this.initialClick.y);
			AbstractInput.selectionArea.Set(this.initialClick.x, Screen.height - this.initialClick.y, newX, newY);
			if (AbstractInput.selectionArea.width < 0) {
				AbstractInput.selectionArea.x += AbstractInput.selectionArea.width;
				AbstractInput.selectionArea.width *= -1f;
			}
			if (AbstractInput.selectionArea.height < 0) {
				AbstractInput.selectionArea.y += AbstractInput.selectionArea.height;
				AbstractInput.selectionArea.height *= -1f;
			}
		}
		if (Input.GetMouseButtonUp(0)) {
			this.leftMouseButtonActive = false;
		}
	}

	private Vector3 getStartingVertex() {
		if (this.initialClick == -Vector3.one) {
			return -Vector3.one;
		}
		float y = Input.mousePosition.y < this.initialClick.y ? Input.mousePosition.y : this.initialClick.y;
		float x = Input.mousePosition.x < this.initialClick.x ? Input.mousePosition.x : this.initialClick.x;
		x /= Screen.width;
		y /= Screen.height;
		return new Vector3(x, y, 0f);
	}

	private Vector3 getEndingVertex() {
		if (this.initialClick == -Vector3.one) {
			return -Vector3.one;
		}
		float x = Input.mousePosition.x < this.initialClick.x ? this.initialClick.x : Input.mousePosition.x;
		float y = Input.mousePosition.y < this.initialClick.y ? this.initialClick.y : Input.mousePosition.y;
		x /= Screen.width;
		y /= Screen.height;
		return new Vector3(x, y, 0f);
	}

	private void OnPostRender() {
		if (!borderMaterial) {
			Debug.LogError("Might need a material here.");
			return;
		}
		if (this.startingVertex == -Vector3.one || this.endingVertex == -Vector3.one) {
			return;
		}
		if (Input.GetMouseButton(0)) {
			GL.PushMatrix();
			if (borderMaterial.SetPass(0)) {
				GL.LoadOrtho();
				GL.Begin(GL.LINES);
				//Vertices uses normalized coordinates, and not absolute values.
				GL.Color(this.borderColor);
				GL.Vertex(this.startingVertex);
				GL.Vertex(new Vector3(this.endingVertex.x, this.startingVertex.y, 0f));
				//--------
				GL.Vertex(new Vector3(this.endingVertex.x, this.startingVertex.y, 0f));
				GL.Vertex(this.endingVertex);
				//--------
				GL.Vertex(this.endingVertex);
				GL.Vertex(new Vector3(this.startingVertex.x, this.endingVertex.y, 0f));
				//--------
				GL.Vertex(new Vector3(this.startingVertex.x, this.endingVertex.y, 0f));
				GL.Vertex(this.startingVertex);
				GL.End();
			}
			GL.PopMatrix();
		}
	}
}
